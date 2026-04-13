using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Entities;
using FresherMisa2026.Entities.Enums;
using FresherMisa2026.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FresherMisa2026.Application.Services
{
    /// <summary>
    /// Service dùng chung cho các entity. Bao gồm các nghiệp vụ chung như Get/Insert/Update/Delete và validate cơ bản.
    /// Mỗi entity-specific service có thể kế thừa BaseService để tái sử dụng logic chung.
    /// </summary>
    /// <typeparam name="TEntity">Loại thực thể</typeparam>
    /// CREATED BY: DVHAI (11/07/2026)
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseModel
    {
        #region Declare
        IBaseRepository<TEntity> _baseRepository;
        protected ServiceResponse _serviceResult = null;
        public Type _modelType = null;
        protected string _tableName = string.Empty;
        #endregion

        #region Constructer
        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            _modelType = typeof(TEntity);
            // Lưu tên bảng (lowercase) để dùng trong log/kiểm tra nếu cần
            _tableName = _modelType.GetTableName().ToLowerInvariant();
            _serviceResult = new ServiceResponse()
            {
                IsSuccess = true,
                Code = (int)ResponseCode.Success,
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy tất cả bản ghi. Gọi repository để lấy dữ liệu và cast về TEntity.
        /// </summary>
        /// <returns>Danh sách bản ghi</returns>
        /// CREATED BY: DVHAI 11/07/2026
        public async Task<IEnumerable<TEntity>> GetEntities()
        {
            var entities = await _baseRepository.GetEntities();
            return entities.Cast<TEntity>();
        }

        /// <summary>
        /// Lấy bản ghi theo Id, gọi repository tương ứng.
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <returns>Bản ghi duy nhất</returns>
        /// CREATED BY: DVHAI (11/07/2026)
        public async Task<TEntity> GetEntityByID(Guid entityId)
        {
            var entity = await _baseRepository.GetEntityByID(entityId);
            return entity;
        }

        /// <summary>
        /// Xóa bản ghi theo Id. Nếu xóa thành công sẽ gọi AfterDelete() để service có thể xử lý thêm.
        /// Trả về true nếu có ít nhất 1 bản ghi bị ảnh hưởng.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (07/07/2026)
        public async Task<bool> DeleteByID(Guid entityId)
        {
            //1. Validate xóa
            bool canDelete = await ValidateBeforeDelete(entityId);
            if (!canDelete)
                return false;
            
            //2. Thực hiện xóa
            int rowAffects = await _baseRepository.Delete(entityId);
            if(rowAffects > 0)
                //3. Xóa thành công thì làm gì
                AfterDelete();

            return rowAffects > 0;
        }

        /// <summary>
        /// Validate toàn bộ entity dựa trên các attribute (ví dụ IRequired) và validate tuỳ chỉnh (ValidateCustom override).
        /// Trả về true nếu pass tất cả kiểm tra.
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool Validate(TEntity entity)
        {
            var isValid = true;

            //1. Đọc các property
            var properties = entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                //1.1 Kiểm tra xem  có attribute cần phải validate không
                if (isValid && property.IsDefined(typeof(IRequired), false))
                {
                    //1.1.1 Check bắt buộc nhập
                    isValid = ValidateRequired(entity, property);
                }
            }

            //2. Validate tùy chỉnh từng màn hình
            if (isValid)
            {
                isValid = ValidateCustom(entity);
            }

            return isValid;
        }

        /// <summary>
        /// Validate bắt buộc nhập cho một property. Nếu thất bại sẽ ghi thông tin vào _serviceResult để controller có thể trả về.
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="propertyInfo">Thuộc tính của thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool ValidateRequired(TEntity entity, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Giả trị
            var propertyValue = propertyInfo.GetValue(entity);

            //3. Tên hiển thị
            var propertyDisplayName = _modelType.GetColumnDisplayName(propertyName);

            // Kiểm tra null/empty. Lưu ý: nếu propertyValue là null, .ToString() sẽ ném NullReferenceException => kiểm tra trước.
            if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
            {
                isValid = false;

                _serviceResult.Code = (int)ResponseCode.BadRequest;
                // DevMessage và Data chứa thông tin chi tiết để client / developer biết lỗi là gì
                _serviceResult.DevMessage = "Trùng dữ liệu.";
                _serviceResult.Data = string.Format("Trùng dữ liệu {0}", propertyDisplayName);
            }

            return isValid;
        }

        /// <summary>
        /// Validate tùy chỉnh cho từng service (nếu cần override trong service con để kiểm tra business rule đặc thù).
        /// Mặc định trả về true (không có validate tuỳ chỉnh).
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// CREATED BY: DVHAI (07/07/2021)
        protected virtual bool ValidateCustom(TEntity entity)
        {
            return true;
        }


        /// <summary>
        /// Thêm một thực thể. Thiết lập ModelSate.Add, gọi Validate và nếu hợp lệ gọi repository Insert.
        /// Trả về ServiceResponse để controller dễ dàng trả HTTP code phù hợp.
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<ServiceResponse> Insert(TEntity entity)
        {
            entity.State = ModelSate.Add;

            //1. Validate tất cả các trường nếu được gắn thẻ
            var isValid = Validate(entity);

            //2. Xử lí kết quả validate
            if (isValid)
            {
                _serviceResult.Data = await _baseRepository.Insert(entity);
                _serviceResult.Code = (int)ResponseCode.Success;
            }
            else
            {
                _serviceResult.Code = (int)ResponseCode.BadRequest;
                _serviceResult.DevMessage = "Validate thất bại";
            }

            //3. Trả về kết quả
            return _serviceResult;
        }

        /// <summary>
        /// Cập nhập thông tin bản ghi. Thiết lập ModelSate.Update, validate, rồi gọi repository Update.
        /// Trả về ServiceResponse với Data là số bản ghi bị ảnh hưởng.
        /// </summary>
        /// <param name="entityId">Id bản ghi</param>
        /// <param name="entity">Thông tin bản ghi</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public async Task<ServiceResponse> Update(Guid entityId, TEntity entity)
        {
            //1. Trạng thái
            entity.State = ModelSate.Update;

            //2. Validate tất cả các trường nếu được gắn thẻ
            var isValid = Validate(entity);
            if (isValid)
            {
                int rowAffects = await _baseRepository.Update(entityId, entity);
                _serviceResult.Data = rowAffects;
                if (rowAffects > 0)
                {
                    _serviceResult.Code = (int)ResponseCode.Success;
                }
                else
                {
                    _serviceResult.Code = (int)ResponseCode.BadRequest;
                }
            }
            else
            {
                _serviceResult.Code = (int)ResponseCode.Success;
                _serviceResult.DevMessage = "Validate thất bại";
            }
            //3. Trả về kế quả
            return _serviceResult;
        }
        #endregion

        #region Virtual method
        /// <summary>
        /// Hook method được gọi sau khi xóa thành công. Service con có thể override để xử lý thêm.
        /// </summary>
        protected virtual void AfterDelete()
        {
        }

        /// <summary>
        /// Trước khi xóa
        /// </summary>
        protected virtual async Task<bool> ValidateBeforeDelete(Guid entityId)
        {
            return true;
        }
        #endregion
    }
}
