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
    /// Base repository with caching support
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// Created By: dvhai (09/04/2026)
    /// Modified By: Tannn (18/04/2026) - Added caching and connection pooling optimization
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseModel
    {
        //Properties
        private readonly string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        protected string _tableName;
        public Type _modelType = null;

        // Cache keys
        private const string CACHE_KEY_ALL_ENTITIES = "BaseRepository_{0}_All";
        private const string CACHE_KEY_ENTITY_BY_ID = "BaseRepository_{0}_ById_{1}";
        
        // Cache duration in minutes
        private const int CACHE_DURATION_MINUTES = 5;

        //Constructor
        public BaseRepository(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName();
        }


        /// <summary>
        /// Creates a new database connection from the connection pool
        /// </summary>
        /// Created By: Tannn (18/04/2026)
        private IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Mở kết nối database
        /// </summary>
        protected async Task<IDbConnection> OpenConnectionAsync()
        {
            var dbConnection = GetConnection();
            if (dbConnection is MySqlConnection mySqlConnection)
            {
                await mySqlConnection.OpenAsync();
            }
            else
            {
                dbConnection.Open();
            }
            return dbConnection;
        }

        #region Method Get
        /// <summary>
        /// Lấy danh sách entity với caching
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created By: dvhai (09/04/2026)
        /// Modified By: Tannn (18/04/2026) - Added caching
        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            var cacheKey = string.Format(CACHE_KEY_ALL_ENTITIES, _tableName);
            
            // Try to get from cache first
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<TEntity> cachedEntities))
            {
                return cachedEntities;
            }

            // If not in cache, fetch from database
            var entities = await GetEntitiesUsingCommandTextAsync();

            // Store in cache with 5-minute expiration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            _memoryCache.Set(cacheKey, entities, cacheOptions);

            return entities;
        }

        /// <summary>
        /// Lấy tất cả theo command text
        /// </summary>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        /// Modified By: Tannn (18/04/2026)
        private async Task<IEnumerable<TEntity>> GetEntitiesUsingCommandTextAsync()
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            if (_modelType.GetHasDeletedColumn())
            {
                whereCount++;
                query.Append($" where IsDeleted = FALSE");
            }

            using (var dbConnection = await OpenConnectionAsync())
            {
                var entities = await dbConnection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);
                return entities.ToList();
            }
        }

        /// <summary>
        /// Lấy bản ghi theo id với caching
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi tìm thấy hoặc null</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        /// Modified By: Tannn (18/04/2026) - Added caching
        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var cacheKey = string.Format(CACHE_KEY_ENTITY_BY_ID, _tableName, entityId);
            
            // Try to get from cache first
            if (_memoryCache.TryGetValue(cacheKey, out TEntity cachedEntity))
            {
                return cachedEntity;
            }

            // If not in cache, fetch from database
            var entity = await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());

            // Store in cache if entity found
            if (entity != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

                _memoryCache.Set(cacheKey, entity, cacheOptions);
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

            using (var dbConnection = await OpenConnectionAsync())
            {
                var entities = await dbConnection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), new { Id = id }, commandType: CommandType.Text);
                return entities;
            }
        }

        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        /// Modified By: DVHAI (18/04/2026) - Added cache invalidation
        public async Task<int> DeleteAsync(Guid entityId)
        {
            var rowAffects = 0;
            var dbConnection = await OpenConnectionAsync();

            using (dbConnection)
            using (var transaction = dbConnection.BeginTransaction())
            {
                try
                {
                    //1. Lấy tên của khóa chính
                    var keyName = _modelType.GetKeyName();

                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add($"@v_{keyName}", entityId);

                    //2. Kết nối tới CSDL:
                    rowAffects = await dbConnection.ExecuteAsync($"Proc_Delete{_tableName}ById", param: dynamicParams, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // Clear cache after successful delete
                    if (rowAffects > 0)
                    {
                        InvalidateCache(entityId);
                    }
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
        /// Modified By: Tannn (18/04/2026) - Added cache invalidation
        public async Task<int> InsertAsync(TEntity entity)
        {
            var rowAffects = 0;
            var dbConnection = await OpenConnectionAsync();
            
            using (dbConnection)
            using (var transaction = dbConnection.BeginTransaction())
            {
                try
                {
                    //1.Duyệt các thuộc tính trên bản ghi và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2.Thực hiện thêm bản ghi
                    rowAffects = await dbConnection.ExecuteAsync($"Proc_Insert{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // Clear cache after successful insert
                    if (rowAffects > 0)
                    {
                        InvalidateCache(null);
                    }
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
        /// Modified By: Tannn (18/04/2026) - Added cache invalidation
        public async Task<int> UpdateAsync(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;
            var dbConnection = await OpenConnectionAsync();
            
            using (dbConnection)
            using (var transaction = dbConnection.BeginTransaction())
            {
                try
                {
                    //1. Duyệt các thuộc tính trên customer và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2. Ánh xạ giá trị id
                    var keyName = _modelType.GetKeyName();
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    //3. Kết nối tới CSDL:
                    rowAffects = await dbConnection.ExecuteAsync($"Proc_Update{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    // Clear cache after successful update
                    if (rowAffects > 0)
                    {
                        InvalidateCache(entityId);
                    }
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

            var dbConnection = await OpenConnectionAsync();

            using (dbConnection)
            {
                string store = string.Format("Proc_{0}_FilterPaging", _tableName);
                var parameters = new DynamicParameters();
                parameters.Add("@v_pageIndex", pageIndex);
                parameters.Add("@v_pageSize", pageSize);
                parameters.Add("@v_search", search);
                parameters.Add("@v_sort", sort);
                parameters.Add("@v_searchFields", JsonSerializer.Serialize(searchFields));

                using var reader = await dbConnection.QueryMultipleAsync(
                    new CommandDefinition(store, parameters, commandType: CommandType.StoredProcedure));

                data = (await reader.ReadAsync<TEntity>()).ToList();
                total = await reader.ReadFirstAsync<long>();

                return (total, data);
            }
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

        /// <summary>
        /// Invalidate cache entries for this entity type
        /// </summary>
        /// <param name="entityId">Specific entity ID to invalidate, or null to invalidate all entities cache</param>
        /// Created By: Tannn (18/04/2026)
        private void InvalidateCache(Guid? entityId = null)
        {
            try
            {
                // Clear all entities cache
                var allEntitiesCacheKey = string.Format(CACHE_KEY_ALL_ENTITIES, _tableName);
                _memoryCache.Remove(allEntitiesCacheKey);

                // Clear specific entity cache if ID provided
                if (entityId.HasValue)
                {
                    var entityCacheKey = string.Format(CACHE_KEY_ENTITY_BY_ID, _tableName, entityId.Value);
                    _memoryCache.Remove(entityCacheKey);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error invalidating cache: {ex.Message}");
            }
        }

        #endregion
    }
}
