using Dapper;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Department;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
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
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _cacheKeysByTable = new();

        private readonly string _connectionString;
        private readonly IMemoryCache _memoryCache;
        protected string _tableName;
        public Type _modelType = null;


        //Constructor
        public BaseRepository(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName();
        }


        /// <summary>
        /// Giải phóng tài nguyên của repository
        /// </summary>
        /// Created By: dvhai (09/04/2026)
        public void Dispose()
        {
        }

        /// <summary>
        /// Tạo và mở kết nối database mới cho từng thao tác.
        /// </summary>
        protected async Task<MySqlConnection> CreateOpenConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private string GetEntitiesCacheKey() => $"BaseRepository:{_tableName}:entities";

        private string GetEntityCacheKey(Guid entityId) => $"BaseRepository:{_tableName}:entity:{entityId:N}";

        private void TrackCacheKey(string cacheKey)
        {
            var cacheKeys = _cacheKeysByTable.GetOrAdd(_tableName, _ => new ConcurrentDictionary<string, byte>());
            cacheKeys.TryAdd(cacheKey, 0);
        }

        private void SetCache<TValue>(string cacheKey, TValue value)
        {
            _memoryCache.Set(
                cacheKey,
                value,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

            TrackCacheKey(cacheKey);
        }

        private void ClearCache()
        {
            if (_cacheKeysByTable.TryRemove(_tableName, out var cacheKeys))
            {
                foreach (var cacheKey in cacheKeys.Keys)
                {
                    _memoryCache.Remove(cacheKey);
                }
            }
        }

        #region Method Get
        /// <summary>
        /// Lấy danh sách entity
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created By: dvhai (09/04/2026)
        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            var cacheKey = GetEntitiesCacheKey();
            if (_memoryCache.TryGetValue(cacheKey, out List<BaseModel>? cachedEntities) && cachedEntities != null)
            {
                return cachedEntities;
            }

            await using var connection = await CreateOpenConnectionAsync();

            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            if (_modelType.GetHasDeletedColumn())
            {
                whereCount++;
                query.Append($" where IsDeleted = FALSE");
            }

            var entities = await connection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);
            var result = entities.Cast<BaseModel>().ToList();

            SetCache(cacheKey, result);

            return result;
        }

        /// <summary>
        /// Lấy bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi tìm thấy hoặc null</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        /// Updated by: Anhs (20/04/2026) - Thêm cache cho phương thức GetEntityByIDAsync
        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var cacheKey = GetEntityCacheKey(entityId);
            if (_memoryCache.TryGetValue(cacheKey, out TEntity? cachedEntity) && cachedEntity != null)
            {
                return cachedEntity;
            }

            await using var connection = await CreateOpenConnectionAsync();

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

            var entity = await connection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), new { Id = entityId.ToString() }, commandType: CommandType.Text);

            if (entity != null)
            {
                SetCache(cacheKey, entity);
            }

            return entity;
        }

        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Số bản ghi bị xóa</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        /// Updated by: Anhs (20/04/2026) - Thêm xóa cache khi xóa bản ghi
        public async Task<int> DeleteAsync(Guid entityId)
        {
            var rowAffects = 0;
            await using var connection = await CreateOpenConnectionAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    //1. Lấy tên của khóa chính
                    var keyName = _modelType.GetKeyName();

                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add($"@v_{keyName}", entityId);

                    //2. Kết nối tới CSDL:
                    rowAffects = await connection.ExecuteAsync($"Proc_Delete{_tableName}ById", param: dynamicParams, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //3. Trả về số bản ghi bị ảnh hưởng
            if (rowAffects > 0)
            {
                ClearCache();
            }

            return rowAffects;
        }


        /// <summary>
        /// Thêm bản ghi mới
        /// </summary>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi thêm mới</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        /// Updated by: Anhs (20/04/2026) - Thêm xóa cache khi thêm mới bản ghi
        public async Task<int> InsertAsync(TEntity entity)
        {
            var rowAffects = 0;
            await using var connection = await CreateOpenConnectionAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    //1.Duyệt các thuộc tính trên bản ghi và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2.Thực hiện thêm bản ghi
                    rowAffects = await connection.ExecuteAsync($"Proc_Insert{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //3.Trả về số bản ghi thêm mới
            if (rowAffects > 0)
            {
                ClearCache();
            }

            return rowAffects;
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        /// Updated by: Anhs (20/04/2026) - Thêm xóa cache khi cập nhật bản ghi
        public async Task<int> UpdateAsync(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;
            await using var connection = await CreateOpenConnectionAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    //1. Duyệt các thuộc tính trên customer và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2. Ánh xạ giá trị id
                    var keyName = _modelType.GetKeyName();
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    //3. Kết nối tới CSDL:
                    rowAffects = await connection.ExecuteAsync($"Proc_Update{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            //4. Trả về dữ liệu
            if (rowAffects > 0)
            {
                ClearCache();
            }

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
        /// Updated by: Anhs (20/04/2026) - Thêm validate pageSize và pageIndex phải lớn hơn 0
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

            await using var connection = await CreateOpenConnectionAsync();

            string store = string.Format("Proc_{0}_FilterPaging", _tableName);
            var parameters = new DynamicParameters();
            parameters.Add("@v_pageIndex", pageIndex);
            parameters.Add("@v_pageSize", pageSize);
            parameters.Add("@v_search", search);
            parameters.Add("@v_sort", sort);
            parameters.Add("@v_searchFields", JsonSerializer.Serialize(searchFields));

            using var reader = await connection.QueryMultipleAsync(
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
