using Dapper;
using FresherMisa2026.Application.Exceptions;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;
using System.Text;
using System.Text.Json;

namespace FresherMisa2026.Infrastructure.Repositories
{
    /// <summary>
    /// Base repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// Created By: dvhai (09/04/2026)
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseModel
    {
        private const int CacheMinutes = 5;

        private readonly string _connectionString;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;
        protected readonly string _tableName;
        private readonly Type _modelType;
        private readonly string _keyName;

        //Constructor
        public BaseRepository(
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes)
            };

            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName();
            _keyName = _modelType.GetKeyName();
        }

        protected MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private string BuildListCacheKey()
        {
            return $"entity:{_tableName}:all";
        }

        private string BuildEntityCacheKey(Guid entityId)
        {
            return $"entity:{_tableName}:id:{entityId}";
        }

        private void InvalidateCache(Guid? entityId = null)
        {
            _memoryCache.Remove(BuildListCacheKey());

            if (entityId.HasValue && entityId.Value != Guid.Empty)
            {
                _memoryCache.Remove(BuildEntityCacheKey(entityId.Value));
            }
        }

        private Guid? ResolveEntityId(TEntity entity)
        {
            var keyProperty = entity.GetType().GetProperty(_keyName);
            if (keyProperty == null)
            {
                return null;
            }

            var value = keyProperty.GetValue(entity);
            return value switch
            {
                Guid guid when guid != Guid.Empty => guid,
                _ => null
            };
        }

        private static bool IsDuplicateKeyException(MySqlException exception)
        {
            return exception.Number == 1062;
        }

        private static bool IsDeadlockException(MySqlException exception)
        {
            return exception.Number == 1213;
        }

        private string BuildDuplicateMessage()
        {
            if (_tableName.Equals("Employee", StringComparison.OrdinalIgnoreCase))
            {
                return "M\u00E3 nh\u00E2n vi\u00EAn \u0111\u00E3 t\u1ED3n t\u1EA1i";
            }

            return "D\u1EEF li\u1EC7u \u0111\u00E3 t\u1ED3n t\u1EA1i";
        }

        #region Method Get
        /// <summary>
        /// Láº¥y danh sÃ¡ch entity
        /// </summary>
        /// <returns>Danh sÃ¡ch táº¥t cáº£ báº£n ghi</returns>
        /// Created By: dvhai (09/04/2026)
        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            var listCacheKey = BuildListCacheKey();

            if (_memoryCache.TryGetValue(listCacheKey, out List<TEntity>? cachedEntities) && cachedEntities != null)
            {
                return cachedEntities.Cast<BaseModel>().ToList();
            }

            var entities = await GetEntitiesUsingCommandTextAsync();
            _memoryCache.Set(listCacheKey, entities, _cacheOptions);

            return entities.Cast<BaseModel>().ToList();
        }

        /// <summary>
        /// Láº¥y táº¥t cáº£ theo command text
        /// </summary>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        private async Task<List<TEntity>> GetEntitiesUsingCommandTextAsync()
        {
            using var connection = CreateConnection();
            var query = new StringBuilder($"select * from {_tableName}");

            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(" where IsDeleted = FALSE");
            }

            var entities = await connection.QueryAsync<TEntity>(
                query.ToString(),
                commandType: CommandType.Text);

            return entities.ToList();
        }

        /// <summary>
        /// Láº¥y báº£n ghi theo id
        /// </summary>
        /// <param name="entityId">Id cá»§a báº£n ghi</param>
        /// <returns>Báº£n ghi tÃ¬m tháº¥y hoáº·c null</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var entityCacheKey = BuildEntityCacheKey(entityId);
            if (_memoryCache.TryGetValue(entityCacheKey, out TEntity? cachedEntity) && cachedEntity != null)
            {
                return cachedEntity;
            }

            var entity = await GetEntitieByIdUsingCommandTextAsync(entityId.ToString());
            if (entity != null)
            {
                _memoryCache.Set(entityCacheKey, entity, _cacheOptions);
            }

            return entity;
        }

        /// <summary>
        /// Láº¥y báº£n ghi theo id dÃ¹ng command text
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<TEntity> GetEntitieByIdUsingCommandTextAsync(string id)
        {
            using var connection = CreateConnection();
            var query = new StringBuilder($"select * from {_tableName}");

            query.Append($" where {_keyName} = @Id");

            if (_modelType.GetHasDeletedColumn())
            {
                query.Append(" and IsDeleted = FALSE");
            }

            var entity = await connection.QueryFirstOrDefaultAsync<TEntity>(
                query.ToString(),
                new { Id = id },
                commandType: CommandType.Text);

            return entity;
        }

        /// <summary>
        /// XÃ³a báº£n ghi theo id
        /// </summary>
        /// <param name="entityId">Id cá»§a báº£n ghi</param>
        /// <returns>Sá»‘ báº£n ghi bá»‹ xÃ³a</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> DeleteAsync(Guid entityId)
        {
            var rowAffects = 0;

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add($"@v_{_keyName}", entityId);

                rowAffects = await connection.ExecuteAsync(
                    $"Proc_Delete{_tableName}ById",
                    param: dynamicParams,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            if (rowAffects > 0)
            {
                InvalidateCache(entityId);
            }

            return rowAffects;
        }

        /// <summary>
        /// ThÃªm báº£n ghi má»›i
        /// </summary>
        /// <param name="entity">ThÃ´ng tin báº£n ghi</param>
        /// <returns>Sá»‘ báº£n ghi thÃªm má»›i</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> InsertAsync(TEntity entity)
        {
            const int maxRetryCount = 2;

            for (var attempt = 1; attempt <= maxRetryCount; attempt++)
            {
                var rowAffects = 0;

                using var connection = CreateConnection();
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var parameters = MappingDbType(entity);

                    rowAffects = await connection.ExecuteAsync(
                        $"Proc_Insert{_tableName}",
                        param: parameters,
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    if (rowAffects > 0)
                    {
                        InvalidateCache(ResolveEntityId(entity));
                    }

                    return rowAffects;
                }
                catch (MySqlException ex) when (IsDuplicateKeyException(ex))
                {
                    transaction.Rollback();
                    throw new DuplicateDataException(BuildDuplicateMessage(), ex);
                }
                catch (MySqlException ex) when (IsDeadlockException(ex) && attempt < maxRetryCount)
                {
                    transaction.Rollback();
                    await Task.Delay(50);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            throw new InvalidOperationException("Không thể thêm dữ liệu do xung đột giao dịch.");
        }

        /// <summary>
        /// Cáº­p nháº­t thÃ´ng tin báº£n ghi
        /// </summary>
        /// <param name="entityId">Id báº£n ghi</param>
        /// <param name="entity">ThÃ´ng tin báº£n ghi</param>
        /// <returns>Sá»‘ báº£n ghi bá»‹ áº£nh hÆ°á»Ÿng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<int> UpdateAsync(Guid entityId, TEntity entity)
        {
            const int maxRetryCount = 2;

            for (var attempt = 1; attempt <= maxRetryCount; attempt++)
            {
                var rowAffects = 0;

                using var connection = CreateConnection();
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var keyProperty = entity.GetType().GetProperty(_keyName);
                    if (keyProperty != null && keyProperty.CanWrite)
                    {
                        keyProperty.SetValue(entity, entityId);
                    }

                    var parameters = MappingDbType(entity);

                    rowAffects = await connection.ExecuteAsync(
                        $"Proc_Update{_tableName}",
                        param: parameters,
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    if (rowAffects > 0)
                    {
                        InvalidateCache(entityId);
                    }

                    return rowAffects;
                }
                catch (MySqlException ex) when (IsDuplicateKeyException(ex))
                {
                    transaction.Rollback();
                    throw new DuplicateDataException(BuildDuplicateMessage(), ex);
                }
                catch (MySqlException ex) when (IsDeadlockException(ex) && attempt < maxRetryCount)
                {
                    transaction.Rollback();
                    await Task.Delay(50);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            throw new InvalidOperationException("Không thể cập nhật dữ liệu do xung đột giao dịch.");
        }

        /// <summary>
        /// Láº¥y danh sÃ¡ch thá»±c thá»ƒ paging
        /// </summary>
        /// <param name="pageSize">Sá»‘ báº£n ghi má»—i trang</param>
        /// <param name="pageIndex">Chá»‰ sá»‘ trang</param>
        /// <param name="search">Tá»« khÃ³a tÃ¬m kiáº¿m</param>
        /// <param name="searchFields">Danh sÃ¡ch trÆ°á»ng tÃ¬m kiáº¿m</param>
        /// <param name="sort">Sáº¯p xáº¿p theo</param>
        /// <returns>Tá»•ng sá»‘ báº£n ghi vÃ  danh sÃ¡ch dá»¯ liá»‡u</returns>
        /// CREATED BY: DVHAI (07/07/2026)
        public async Task<(long Total, IEnumerable<TEntity> Data)> GetFilterPagingAsync(
            int pageSize,
            int pageIndex,
            string search,
            List<string> searchFields,
            string sort)
        {
            long total;
            IEnumerable<TEntity> data;

            using var connection = CreateConnection();
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
        /// Ãnh xáº¡ cÃ¡c thuá»™c tÃ­nh sang kiá»ƒu dynamic
        /// </summary>
        /// <param name="entity">Thá»±c thá»ƒ</param>
        /// <returns>Dan sÃ¡ch cÃ¡c biáº¿n Ä‘á»™ng</returns>
        private DynamicParameters MappingDbType(TEntity entity)
        {
            var parameters = new DynamicParameters();
            try
            {
                var properties = entity.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(entity);
                    var propertyType = property.PropertyType;

                    if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        parameters.Add($"@v_{propertyName}", propertyValue, DbType.String);
                    }
                    else
                    {
                        parameters.Add($"@v_{propertyName}", propertyValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mapping entity properties: {ex.Message}");
            }

            return parameters;
        }

        #endregion
    }
}
