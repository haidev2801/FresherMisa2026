using Dapper;
using FresherMisa2026.Application.Interfaces;

using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// Created By: dvhai (09/04/2026)
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : BaseModel
    {
        //Properties
        string _connectionString = string.Empty;
        IConfiguration _configuration;
        protected IDbConnection _dbConnection = null;
        protected string _tableName;
        public Type _modelType = null;
        protected IMemoryCache _cache;


        //Constructor
        public BaseRepository(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            // Lấy connection string từ appsettings (key: DefaultConnection)
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            // Khởi tạo connection MySql
            _dbConnection = new MySqlConnection(_connectionString);
            // Lưu type của model để dùng reflection
            _modelType = typeof(TEntity);
            // Lấy tên bảng từ attribute ConfigTable
            _tableName = _modelType.GetTableName();
        }


        /// <summary>
        /// Dispose connection nếu đang mở.
        /// Đảm bảo giải phóng tài nguyên DB khi repository bị dispose.
        /// </summary>
        /// Created By: dvhai (09/04/2026)
        public void Dispose()
        {
            if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
            }
        }

        /// <summary>
        /// Mở kết nối database
        /// </summary>
        private async Task OpenConnectionAsync()
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                if (_dbConnection is MySqlConnection mySqlConnection)
                {
                    await mySqlConnection.OpenAsync();
                }
                else
                {
                    _dbConnection.Open();
                }
            }
        }

        #region Method Get
        /// <summary>
        /// Lấy danh sách entity (gọi hàm private để build command text)
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created By: dvhai (09/04/2026)
        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            var cacheKey = $"{_tableName}_ALL";

            // nếu có cache thì trả luôn
            if (_cache.TryGetValue(cacheKey, out IEnumerable<TEntity> cached))
            {
                Console.WriteLine("GET ALL FROM CACHE");
                return cached;
            }

            Console.WriteLine("GET ALL FROM DB");

            var data = await GetEntitiesUsingCommandTextAsync();

            // lưu cache 5 phút
            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));

            return data;
        }

        /// <summary>
        /// Lấy tất cả theo command text.
        /// Nếu table có cột IsDeleted thì tự động thêm điều kiện lọc IsDeleted = FALSE.
        /// Trả về danh sách TEntity.
        /// </summary>
        /// CREATED BY: DVHAI (11/07/2021)
        private async Task<IEnumerable<TEntity>> GetEntitiesUsingCommandTextAsync()
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            // Nếu cấu hình entity có cột IsDeleted, thêm điều kiện lọc
            if (_modelType.GetHasDeletedColumn())
            {
                whereCount++;
                query.Append($" where IsDeleted = FALSE");
            }

            // Dapper query để map kết quả sang TEntity
            var entities = await _dbConnection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);

            return entities.ToList();
        }

        /// <summary>
        /// Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi tìm thấy hoặc null</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var cacheKey = $"{_tableName}_{entityId}";

            // check cache
            if (_cache.TryGetValue(cacheKey, out TEntity cached))
            {
                Console.WriteLine("GET BY ID FROM CACHE");
                return cached;
            }

            Console.WriteLine("GET BY ID FROM DB");

            var entity = await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());

            if (entity != null)
            {
                _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(5));
            }

            return entity;
        }

        /// <summary>
        /// Lấy bản ghi theo id dùng command text.
        /// Sử dụng reflection để lấy tên khóa chính và thêm điều kiện lọc IsDeleted nếu cần.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<TEntity> GetEntitieByIdUsingCommandTextAsync(string id)
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            // Hàm nội bộ để thêm 'where' lần đầu tiên nếu cần
            Func<StringBuilder, bool> AppendWhere = (query) => { if (whereCount == 0) query.Append(" where "); return true; };

            // Lấy tên khóa chính theo attribute [Key]
            var primaryKey = _modelType.GetKeyName();

            if (primaryKey != null)
            {
                AppendWhere(query);
                query.Append($"{primaryKey} = @Id");
                whereCount++;
            }

            if (_modelType.GetHasDeletedColumn())
            {
                AppendWhere(query);
                query.Append("IsDeleted = FALSE");
                whereCount++;
            }

            var entities = await _dbConnection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), new { Id = id }, commandType: CommandType.Text);

            return entities;
        }

        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> DeleteAsync(Guid entityId)
        {
            var rowAffects = 0;
            await OpenConnectionAsync();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1. Lấy tên của khóa chính
                    var keyName = _modelType.GetKeyName();

                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add($"@v_{keyName}", entityId);

                    //2. Kết nối tới CSDL: gọi stored procedure theo quy ước tên
                    rowAffects = await _dbConnection.ExecuteAsync($"Proc_Delete{_tableName}ById", param: dynamicParams, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // clear cache
                    _cache.Remove($"{_tableName}_ALL");
                    _cache.Remove($"{_tableName}_{entityId}");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //3. Trả về số bản ghi bị ảnh hưởng
            return rowAffects;
        }


        /// <summary>
        /// Thêm bản ghi mới
        /// </summary>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi thêm mới</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> InsertAsync(TEntity entity)
        {
            var rowAffects = 0;
            await OpenConnectionAsync();
            
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1.Duyệt các thuộc tính trên bản ghi và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2.Thực hiện thêm bản ghi
                    rowAffects = await _dbConnection.ExecuteAsync($"Proc_Insert{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // clear cache
                    _cache.Remove($"{_tableName}_ALL");
                }

                catch (MySqlException ex)
                {
                    transaction.Rollback();

                    if (ex.Number == 1062)
                    {
                        Console.WriteLine($"MySQL Duplicate Entry Error: {ex.Message}");
                        if (ex.Message.Contains("uq_employee_code"))
                            throw new DuplicateException("Mã nhân viên", ex.Message);

                        if (ex.Message.Contains("uq_department_code"))
                            throw new DuplicateException("Mã phòng ban", ex.Message);

                        if (ex.Message.Contains("uq_position_code"))
                            throw new DuplicateException("Mã chức vụ", ex.Message);


                        throw new DuplicateException("Dữ liệu", ex.Message);
                    }

                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //3.Trả về số bản ghi thêm mới
            return rowAffects;
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> UpdateAsync(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;
            await OpenConnectionAsync();
            
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1. Duyệt các thuộc tính trên customer và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2. Ánh xạ giá trị id vào property tương ứng (theo primary key)
                    var keyName = _modelType.GetKeyName();
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    //3. Kết nối tới CSDL: gọi stored procedure update
                    rowAffects = await _dbConnection.ExecuteAsync($"Proc_Update{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // clear cache
                    _cache.Remove($"{_tableName}_ALL");
                    _cache.Remove($"{_tableName}_{entityId}");
                }

                catch (MySqlException ex)
                {
                    transaction.Rollback();

                    if (ex.Number == 1062)
                    {
                        if (ex.Message.Contains("EmployeeCode"))
                            throw new DuplicateException("Mã nhân viên", ex.Message);

                        if (ex.Message.Contains("DepartmentCode"))
                            throw new DuplicateException("Mã phòng ban", ex.Message);

                        if (ex.Message.Contains("PositionCode"))
                            throw new DuplicateException("Mã chức vụ", ex.Message);

                        throw new DuplicateException("Dữ liệu", ex.Message);
                    }

                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            //4. Trả về dữ liệu
            return rowAffects;
        }

        /// <summary>
        /// Lấy danh sách thực thể paging
        /// </summary>
        /// <param name="pageSize">Số bản ghi mỗi trang</param>
        /// <param name="pageIndex">Chỉ số trang</param>
        /// <param name="search">Từ khóa tìm kiếm</param>
        /// <param name="searchFields">Danh sách trường tìm kiếm</param>
        /// <param name="sort">Sắp xếp theo</param>
        /// <returns>Tổng số bản ghi và danh sách dữ liệu</returns>
        /// CREATED BY: DVHAI (07/07/2026)
        public async Task<(long Total,
            IEnumerable<TEntity> Data)> GetFilterPagingAsync(
            int pageSize,
            int pageIndex,
            string search,
            List<string> searchFields,
            string sort)
        {
            long total = 0;
            var data = Enumerable.Empty<TEntity>();

            await OpenConnectionAsync();

            string store = string.Format("Proc_{0}_FilterPaging", _tableName);
            var parameters = new DynamicParameters();
            parameters.Add("@v_pageIndex", pageIndex);
            parameters.Add("@v_pageSize", pageSize);
            parameters.Add("@v_search", search);
            parameters.Add("@v_sort", sort);
            parameters.Add("@v_searchFields", JsonSerializer.Serialize(searchFields));

            using var reader = await _dbConnection.QueryMultipleAsync(
                new CommandDefinition(store, parameters, commandType: CommandType.StoredProcedure));

            data = (await reader.ReadAsync<TEntity>()).ToList();
            total = await reader.ReadFirstAsync<long>();

            return (total, data);
        }

        /// <summary>
        /// Ánh xạ các thuộc tính sang kiểu dynamic
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>Dan sách các biến động</returns>
            private DynamicParameters MappingDbType(TEntity entity)
        {
            var parameters = new DynamicParameters();
            try
            {
                //1. Duyệt các thuộc tính trên entity và tạo parameters
                var properties = entity.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(entity);
                    var propertyType = property.PropertyType;

                    // Nếu property là Guid hoặc Guid? thì map kiểu DbType.String (thường dùng trong project lưu Guid dưới dạng chuỗi)
                    if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                        parameters.Add($"@v_{propertyName}", propertyValue, DbType.String);
                    else
                        parameters.Add($"@v_{propertyName}", propertyValue);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with empty parameters
                Console.WriteLine($"Error mapping entity properties: {ex.Message}");
            }
            //2. Trả về danh sách các parameter
            return parameters;
        }

        #endregion
        
    }
}
