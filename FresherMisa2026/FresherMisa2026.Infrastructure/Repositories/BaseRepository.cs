using Dapper;
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
    /// Base repository — mỗi lần gọi DB dùng connection ngắn (mở → dùng → dispose) để trả connection về pool sớm;
    /// đọc danh sách / theo id được cache 5 phút, CUD xóa cache tương ứng.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// Created By: dvhai (09/04/2026)
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseModel
    {
        /// <summary>
        /// Chuỗi kết nối dùng cho mọi <see cref="MySqlConnection"/> mới — pool được bật mặc định theo connection string.
        /// </summary>
        protected readonly string _connectionString;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// Thời sống cache đọc (theo yêu cầu bài): sau 5 phút entry tự hết hạn.
        /// </summary>
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        protected string _tableName;
        public Type _modelType = null!;

        public BaseRepository(IConfiguration configuration, IMemoryCache cache)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _cache = cache;
            _modelType = typeof(TEntity);
            _tableName = _modelType.GetTableName();
        }

        /// <summary>
        /// Key cache cho toàn bộ bản ghi của bảng (một loại entity một key).
        /// </summary>
        private string AllEntitiesCacheKey => $"repo:{_tableName}:all";

        /// <summary>
        /// Key cache cho một id — dùng định dạng "N" của Guid để key ổn định, ngắn.
        /// </summary>
        private string EntityByIdCacheKey(Guid id) => $"repo:{_tableName}:id:{id:N}";

        /// <summary>
        /// Tạo connection mới và mở ngay. Gọi bằng <c>await using</c> để khi hết scope connection đóng và trả về pool.
        /// </summary>
        protected async Task<MySqlConnection> CreateAndOpenConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            // Bảng trong ChangeLogs dùng COLLATE utf8mb4_general_ci; MySQL 8 mặc định session thường là
            // utf8mb4_0900_ai_ci — tham số/kết nối và cột char khác collation → "Illegal mix of collations"
            // trong Proc_Update*/Proc_Delete* (so sánh khóa / mã). Khớp session với DDL bảng.
            using (var init = connection.CreateCommand())
            {
                init.CommandText = "SET NAMES utf8mb4 COLLATE utf8mb4_general_ci;";
                await init.ExecuteNonQueryAsync();
            }

            return connection;
        }

        /// <summary>
        /// Sau Insert: danh sách tổng đã sai → chỉ cần xóa cache list.
        /// </summary>
        private void InvalidateAllEntitiesCache() => _cache.Remove(AllEntitiesCacheKey);

        /// <summary>
        /// Sau Update/Delete: bản ghi đó và có thể cả list đều sai → xóa cả hai.
        /// </summary>
        private void InvalidateEntityAndListCache(Guid entityId)
        {
            _cache.Remove(AllEntitiesCacheKey);
            _cache.Remove(EntityByIdCacheKey(entityId));
        }

        #region Method Get

        public async Task<IEnumerable<BaseModel>> GetEntitiesAsync()
        {
            // GetOrCreateAsync: lần đầu chạy factory (query DB), các lần sau trả bản copy đã lưu trong memory cho đến khi hết hạn hoặc bị Remove.
            var list = await _cache.GetOrCreateAsync(AllEntitiesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                var fromDb = await LoadAllEntitiesFromDbAsync();
                return fromDb.Cast<BaseModel>().ToList();
            });

            return list ?? Enumerable.Empty<BaseModel>();
        }

        public async Task<TEntity> GetEntityByIDAsync(Guid entityId)
        {
            var key = EntityByIdCacheKey(entityId);

            // Đã có trong cache (kể cả sau khi load từ DB trước đó).
            if (_cache.TryGetValue(key, out TEntity? cached))
                return cached!;

            var entity = await LoadEntityByIdFromDbAsync(entityId.ToString());

            // Không cache bản null để tránh nhầm với "chưa có key" khi TryGetValue.
            if (entity is not null)
            {
                _cache.Set(key, entity, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });
            }

            return entity!;
        }

        private async Task<List<TEntity>> LoadAllEntitiesFromDbAsync()
        {
            await using var connection = await CreateAndOpenConnectionAsync();
            return (await QueryEntitiesCommandTextAsync(connection)).ToList();
        }

        private async Task<IEnumerable<TEntity>> QueryEntitiesCommandTextAsync(MySqlConnection connection)
        {
            var query = new StringBuilder($"select * from {_tableName}");

            if (_modelType.GetHasDeletedColumn())
                query.Append(" where IsDeleted = FALSE");

            var entities = await connection.QueryAsync<TEntity>(query.ToString(), commandType: CommandType.Text);
            return entities.ToList();
        }

        private async Task<TEntity?> LoadEntityByIdFromDbAsync(string id)
        {
            await using var connection = await CreateAndOpenConnectionAsync();
            return await QueryEntityByIdCommandTextAsync(connection, id);
        }

        private async Task<TEntity?> QueryEntityByIdCommandTextAsync(MySqlConnection connection, string id)
        {
            var query = new StringBuilder($"select * from {_tableName}");
            int whereCount = 0;

            Func<StringBuilder, bool> AppendWhere = (q) => { if (whereCount == 0) q.Append(" where "); return true; };

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

            return await connection.QueryFirstOrDefaultAsync<TEntity>(query.ToString(), new { Id = id }, commandType: CommandType.Text);
        }

        public async Task<int> DeleteAsync(Guid entityId)
        {
            var rowAffects = 0;

            await using var connection = await CreateAndOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var keyName = _modelType.GetKeyName();
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add($"@v_{keyName}", entityId);

                rowAffects = await connection.ExecuteAsync(
                    $"Proc_Delete{_tableName}ById",
                    param: dynamicParams,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                transaction.Commit();

                // Chỉ xóa cache sau commit — dữ liệu DB đã khớp với thực tế.
                InvalidateEntityAndListCache(entityId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return rowAffects;
        }

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            var rowAffects = 0;

            await using var connection = await CreateAndOpenConnectionAsync();
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

                // Thêm mới làm thay đổi danh sách "get all"; cache theo id bản mới thường chưa tồn tại nên chỉ cần list.
                InvalidateAllEntitiesCache();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return rowAffects;
        }

        /// <summary>
        /// Proc_* dùng SIGNAL SQLSTATE '45000' — map sang exception để tầng service trả 404/400 thay vì 500.
        /// </summary>
        private static Exception MapUserDefinedSignal(MySqlException ex)
        {
            var msg = ex.Message ?? string.Empty;
            if (msg.Contains("không tồn tại", StringComparison.OrdinalIgnoreCase))
                return new KeyNotFoundException(msg);
            if (msg.Contains("đã tồn tại", StringComparison.OrdinalIgnoreCase))
                return new InvalidOperationException(msg);
            return ex;
        }

        public virtual async Task<int> UpdateAsync(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;

            await using var connection = await CreateAndOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var parameters = MappingDbType(entity);

                var keyName = _modelType.GetKeyName();
                entity.GetType().GetProperty(keyName)!.SetValue(entity, entityId);

                rowAffects = await connection.ExecuteAsync(
                    $"Proc_Update{_tableName}",
                    param: parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                transaction.Commit();

                InvalidateEntityAndListCache(entityId);
            }
            catch (MySqlException ex) when (ex.SqlState == "45000")
            {
                transaction.Rollback();
                throw MapUserDefinedSignal(ex);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return rowAffects;
        }

        public async Task<(long Total, IEnumerable<TEntity> Data)> GetFilterPagingAsync(
            int pageSize,
            int pageIndex,
            string search,
            List<string> searchFields,
            string sort)
        {
            long total = 0;
            var data = Enumerable.Empty<TEntity>();

            // Phân trang có tham số động — không cache theo yêu cầu Task 3.1 (chỉ Get all / by id).
            await using var connection = await CreateAndOpenConnectionAsync();

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

        private DynamicParameters MappingDbType(TEntity entity)
        {
            var parameters = new DynamicParameters();
            try
            {
                var properties = entity.GetType().GetProperties();

                foreach (var property in properties)
                {
                    // Proc_Insert* / Proc_Update* chỉ khai báo tham số theo cột bảng; không gồm audit/State từ BaseModel.
                    // Gửi thừa @v_* khiến MySQL/Dapper lỗi "too many arguments" → API 500.
                    if (property.DeclaringType == typeof(BaseModel))
                        continue;

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
                Console.WriteLine($"Error mapping entity properties: {ex.Message}");
            }

            return parameters;
        }

        #endregion
    }
}
