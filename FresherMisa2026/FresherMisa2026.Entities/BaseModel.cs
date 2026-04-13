using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    /// <summary>
    /// Attribute đánh dấu một property là bắt buộc (custom required attribute dùng trong BaseService.Validate).
    /// Gắn lên property trong entity khi muốn kiểm tra bắt buộc nhập.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IRequired : Attribute
    {

    }

    /// <summary>
    /// Lớp model cơ sở (BaseModel) mà tất cả entity sẽ kế thừa.
    /// Chứa các thuộc tính phổ biến như CreatedBy, CreateDate, trạng thái thao tác (State) và cờ IsDeleted.
    /// Các thuộc tính này không bắt buộc phải ánh xạ vào database (tuỳ thiết kế), nhưng được dùng trong business logic chung.
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Người tạo bản ghi (tên hoặc id người dùng). Có thể null nếu không gán.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày tạo bản ghi. Có thể null nếu chưa bao giờ được gán.
        /// </summary>
        public DateTime? CreateDate { get; set; }
        
        /// <summary>
        /// Người sửa bản ghi gần nhất.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Ngày sửa gần nhất.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Trạng thái thao tác (Add/Update/Delete) dùng trong logic khi ánh xạ sang DB.
        /// Không lưu trực tiếp vào database.
        /// </summary>
        public ModelSate State { get; set; }

        /// <summary>
        /// Cờ xóa mềm. Nếu true thì bản ghi được xem là đã bị xóa (IsDeleted = true).
        /// Repository có thể dùng cờ này để lọc kết quả trả về.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
