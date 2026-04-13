using Dapper;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// BaseRepository cung cấp các phương thức CRUD chung sử dụng Dapper và MySqlConnector.
    /// TEntity phải kế thừa BaseModel để có các thuộc tính chung.
    /// Repository dùng reflection (MethodExtensions) để lấy tên bảng, tên primary key và cờ IsDeleted.
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


        //Constructor
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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

        #region Method Get
        /// <summary>
        /// Lấy danh sách entity (gọi hàm private để build command text)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// Created By: dvhai (09/04/2026)
        public async Task<IEnumerable<BaseModel>> GetEntities()
        {
            return await GetEntitiesUsingCommandTextAsync();
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

        // <summary>
        ///  Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi thông tin 1 bản ghi</return
        /// CREATED BY: DVHAI (07/07/2021)
        public async Task<TEntity> GetEntityByID(Guid entityId)
        {
            return await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());
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
                // NOTE: id ở đây được chèn trực tiếp vào query string => chú ý SQL injection nếu giá trị không được kiểm soát.
                // Trong dự án hiện tại dùng Guid và input từ server nên rủi ro thấp, nhưng tốt hơn là dùng parameterized query.
                query.Append($"{primaryKey} = '{id}'");
                whereCount++;
            }

            if (_modelType.GetHasDeletedColumn())
            {
                AppendWhere(query);
                query.Append($"IsDeleted = FALSE");
                whereCount++;
            }

            var entities = await _dbConnection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), commandType: CommandType.Text);

            return entities;
        }

        /// <summary>
        /// Xóa theo mã (gọi stored procedure Proc_Delete{TableName}ById)
        /// Repository mở transaction, gọi SP và commit/rollback.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> Delete(Guid entityId)
        {
            var rowAffects = 0;
            _dbConnection.Open();

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
                }
                catch { transaction.Rollback(); }
            }

            //3. Trả về số bản ghi bị ảnh hưởng
            return rowAffects;
        }


        /// <summary>
        /// Thêm bản ghi (gọi stored procedure Proc_Insert{TableName}).
        /// Thực hiện mapping các thuộc tính entity sang DynamicParameters rồi gọi SP.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> Insert(TEntity entity)
        {
            var rowAffects = 0;
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1.Duyệt các thuộc tính trên bản ghi và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2.Thực hiện thêm bản ghi
                    rowAffects = await _dbConnection.ExecuteAsync($"Proc_Insert{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            //3.Trả về số bản ghi thêm mới
            return rowAffects;
        }

        /// <summary>
        /// Cập nhập bản ghi (gọi stored procedure Proc_Update{TableName}).
        /// Gán giá trị id vào property tương ứng trước khi map parameters.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entity"></param>
        /// <returns>số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> Update(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;
            _dbConnection.Open();
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
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
            //4. Trả về dữ liệu
            return rowAffects;
        }

        /// <summary>
        /// Ánh xạ các thuộc tính sang DynamicParameters của Dapper.
        /// Quy ước tên tham số: @v_{PropertyName}
        /// Guid/Guid? được map sang DbType.String để lưu dưới dạng chuỗi.
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>Danh sách các parameter</returns>
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
            catch { }
            //2. Trả về danh sách các parameter
            return parameters;
        }

        #endregion
    }
}
