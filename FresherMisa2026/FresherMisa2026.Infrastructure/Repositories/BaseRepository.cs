using Dapper;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository với connection pooling và caching
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

        // Cache
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Cache key cho danh sách entities
        /// </summary>
        private string GetEntitiesCacheKey() => $"Entities_{_tableName}_All";

        /// <summary>
        /// Cache key cho entity theo ID
        /// </summary>
        private string GetEntityByIdCacheKey(Guid id) => $"Entity_{_tableName}_{id}";


        //Constructor
        public BaseRepository(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            _dbConnection = new MySqlConnection(_connectionString);
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName();
            _cache = cache;
        }

        // Constructor cho backward compatibility (các repository con không cần sửa nếu chưa dùng cache)
        public BaseRepository(IConfiguration configuration) : this(configuration, new MemoryCache(new MemoryCacheOptions()))
        {
        }


        /// <summary>
        /// Dispose connection
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
        /// Mở kết nối database - sử dụng connection pooling từ MySqlConnector
        /// MySqlConnector tự quản lý connection pooling qua connection string
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

        /// <summary>
        /// Xóa cache liên quan đến entity khi có thay đổi dữ liệu
        /// </summary>
        private void InvalidateCache()
        {
            // Xóa cache danh sách
            _cache.Remove(GetEntitiesCacheKey());
        }

        /// <summary>
        /// Xóa cache cho một entity cụ thể và danh sách
        /// </summary>
        private void InvalidateCache(Guid entityId)
        {
            _cache.Remove(GetEntitiesCacheKey());
            _cache.Remove(GetEntityByIdCacheKey(entityId));
        }

        #region Method Get
        /// <summary>
        /// Lấy danh sách entity (có cache 5 phút)
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created By: dvhai (09/04/2026)
        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            var cacheKey = GetEntitiesCacheKey();

            // Kiểm tra cache trước
            if (_cache.TryGetValue(cacheKey, out IEnumerable<TEntity> cachedEntities))
            {
                return cachedEntities;
            }

            // Nếu không có cache, truy vấn DB
            var entities = await GetEntitiesUsingCommandTextAsync();

            // Lưu vào cache với thời gian 5 phút
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration)
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            _cache.Set(cacheKey, entities, cacheOptions);

            return entities;
        }

        /// <summary>
        /// Lấy tất cả theo command text
        /// </summary>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        private async Task<IEnumerable<TEntity>> GetEntitiesUsingCommandTextAsync()
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            if (_modelType.GetHasDeletedColumn())
            {
                whereCount++;
                query.Append($" where IsDeleted = FALSE");
            }

            var entities = await _dbConnection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);

            return entities.ToList();
        }

        /// <summary>
        /// Lấy bản ghi theo id (có cache 5 phút)
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi tìm thấy hoặc null</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var cacheKey = GetEntityByIdCacheKey(entityId);

            // Kiểm tra cache trước
            if (_cache.TryGetValue(cacheKey, out TEntity cachedEntity))
            {
                return cachedEntity;
            }

            // Nếu không có cache, truy vấn DB
            var entity = await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());

            // Lưu vào cache nếu tìm thấy
            if (entity != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(CacheDuration)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(cacheKey, entity, cacheOptions);
            }

            return entity;
        }

        /// <summary>
        /// Lấy bản ghi theo id dùng command text
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<TEntity> GetEntitieByIdUsingCommandTextAsync(string id)
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            Func<StringBuilder, bool> AppendWhere = (query) => { if (whereCount == 0) query.Append(" where "); return true; };

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
        /// Xóa bản ghi theo id (invalidate cache)
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

                    //2. Kết nối tới CSDL:
                    rowAffects = await _dbConnection.ExecuteAsync($"Proc_Delete{_tableName}ById", param: dynamicParams, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    //3. Xóa cache khi delete thành công
                    InvalidateCache(entityId);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //4. Trả về số bản ghi bị ảnh hưởng
            return rowAffects;
        }


        /// <summary>
        /// Thêm bản ghi mới (invalidate cache)
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

                    //3. Xóa cache danh sách khi insert thành công
                    InvalidateCache();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //4.Trả về số bản ghi thêm mới
            return rowAffects;
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi (invalidate cache)
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

                    //2. Ánh xạ giá trị id
                    var keyName = _modelType.GetKeyName();
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    //3. Kết nối tới CSDL:
                    rowAffects = await _dbConnection.ExecuteAsync($"Proc_Update{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    //4. Xóa cache khi update thành công
                    InvalidateCache(entityId);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            //5. Trả về dữ liệu
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
