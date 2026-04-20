# Update Task 1 
Thống nhất cách xử lý exception: 

- Dữ liệu nhập vào input sai -> 400
- Không tìm thấy dữ liệu -> 404 
- Lỗi hệ thống -> 500 

### 1) BaseController.cs
- Đổi tham số của PUT từ  `string id` sang `Guid id`, bỏ `Guid.Parse(id)`.
-> id sai định dạng sẽ tự động trả 400, không bị đẩy về 500.

### 2) DepartmentService.cs
Trong các hàm `GetDepartmentByCodeAsync`, `GetEmployeesByDepartmentCodeAsync`, `GetEmployeeCountByDepartmentCodeAsync`
- Dùng `ArgumentException` khi sai request/ rỗng
- Dung `KeyNotFoundException` khi không tìm thấy dữ liệu

Sửa logic `ValidateBeforeDeleteAsync` để: 
  - ID không tồn tại -> 404
  - Phòng ban có nhân viên -> không cho xóa (400)

### 3) PositionService.cs 
Trong hàm `GetPositionByCodeAsync`: 
- kiểm tra request/ rỗng -> `ArgumentException`.
- Không tìm thấy -> `KeyNotFoundException`.

### 4) EmployeeService.cs
Trong hàm `GetEmployeeByCodeAsync`: 
- kiểm tra request/ rỗng -> `ArgumentException`.
- Không tìm thấy -> `KeyNotFoundException`.

### 5) GlobalExceptionMiddleware.cs
Cập nhật status code: 
  - `ArgumentException`, `FormatException` -> 400
  - `KeyNotFoundException` -> 404
  - Còn lại -> 500

