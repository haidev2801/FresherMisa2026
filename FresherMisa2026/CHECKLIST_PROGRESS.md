# Checklist tiến độ công việc

## ✅ Đã hoàn thành

- [x] **BaseRepository - Insert (Create):**
  - [x] Thêm logic ánh xạ giá trị ID sau khi sinh `entityId`:
    - `var keyName = _modelType.GetKeyName();`
    - `entity.GetType().GetProperty(keyName).SetValue(entity, entityId);`

- [x] **BaseRepository - Update:**
  - [x] Đổi thứ tự giữa bước 1 và bước 2 theo logic mới.

- [x] **MySQL - Đồng bộ charset/collation:**
  - [x] Tắt tạm kiểm tra khóa ngoại: `SET FOREIGN_KEY_CHECKS = 0;`
  - [x] `ALTER TABLE department CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;`
  - [x] `ALTER TABLE position CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;`
  - [x] `ALTER TABLE employee CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;`
  - [x] Bật lại kiểm tra khóa ngoại: `SET FOREIGN_KEY_CHECKS = 1;`
  Git commit: "fix(repository): map generated id in insert and reorder update steps"

- [x] **Task 1.1 - Hoàn thiện CRUD cho Position:**
  - [x] Tạo `PositionRepository` kế thừa `BaseRepository<Position>`
  - [x] Tạo `PositionService` kế thừa `BaseService<Position>`
  - [x] Tạo `PositionsController` kế thừa `BaseController<Position>`
  - [x] Đăng ký DI trong `ServiceExtensions.cs`
  - [x] Kiểm tra đủ 5 API CRUD: GET all, GET by id, POST, PUT, DELETE
  Git commit: "feat(task-1.1): implement position CRUD"

  - [x] **Task 1.2 - Hoàn thiện CRUD cho Employee:**
  - [x] Tạo `EmployeeRepository` kế thừa `BaseRepository<Employee>`
  - [x] Tạo `EmployeeService` kế thừa `BaseService<Employee>`
  - [x] Tạo `EmployeesController` kế thừa `BaseController<Employee>`
  - [x] Đăng ký DI trong `ServiceExtensions.cs`
  - [x] Kiểm tra đủ 5 API CRUD: GET all, GET by id, POST, PUT, DELETE
  Git commit: "feat(task-1.2): implement employee CRUD"

  - [x] **Task 2.1 - Thêm Validation cho Employee:**
  - [x] Thêm `[IRequired]` attribute cho các trường bắt buộc: `EmployeeCode`, `EmployeeName`, `DepartmentID`, `PositionID`
  - [x] Override method `ValidateCustom()` trong `EmployeeService` để thêm các validation tùy chỉnh:
    - Mã nhân viên không được trùng lặp
    - Email phải đúng định dạng (nếu có)
    - Số điện thoại phải đúng định dạng (nếu có)
    - Ngày sinh phải nhỏ hơn ngày hiện tại
  - [x] Kiểm tra các trường hợp validation: thiếu trường bắt buộc, mã nhân viên trùng lặp, email/số điện thoại sai định dạng, ngày sinh không hợp lệ
  Git commit: "feat(task-2.1): add validation for employee entity"

  - [x] **Task 2.2 - Tạo API Lọc Employee Theo Điều Kiện:**
  - [x] Tạo `FilterEmployeesRequest` để nhận các điều kiện lọc từ client
  - [x] Thêm method `GetEmployeeByFilter` trong `EmployeeService`
  - [x] Thêm endpoint `GET /api/employees/filter` trong `EmployeesController`
  - [x] Kiểm tra các trường hợp lọc: theo phòng ban, vị trí, mức lương, giới tính, ngày tuyển dụng
  Git commit: "feat(task-2.2): implement employee filtering API"

  - [x] **Task 2.3 - Thêm Custom Endpoint Cho Department:**
  - [x] Thêm endpoint `GET /api/departments/{id}/employees` để lấy danh sách nhân viên theo phòng ban
  - [x] Thêm enpoint `GET /api/departments/{id}/employees-count` để lấy số lượng nhân viên theo phòng ban
  Git commit: "feat(task-2.3): add API get employees and employee count by department"

  - [x] **Task 3.1: Refactor BaseRepository - Tối Ưu Performance**
  - Refactor để sử dụng connection pooling hiệu quả hơn - DONE
    Test cho API GetEntitiesUsingCommandTextAsync dùng using tạo connection để hết hàm sẽ tự dipose, không giữ lại như tạo trong constructor
  - Thêm caching cho các truy vấn `GetEntities`, `GetEntityByID` với thời gian cache 5 phút - DONE
    Hiện đang để cache sống 5 phút và gia hạn thêm 2 phút mỗi khi request
    Dùng ImemoryCache cho GetEntities, GetEntityByID trong BaseService
    Thêm RemoveCache để clear cache khi data change
    delete thì truyền thêm id để xóa cache ở GetEntityByID nữa
  - Đánh giá: Sử dụng `IMemoryCache` hoặc `IDistributedCache` - Dùng IMemoryCache 

**Kiểm tra:**
- [x] Review code để đảm bảo không có breaking changes
- [x] Test performance trước và sau khi refactor
- [x] Đảm bảo cache được clear khi có thay đổi dữ liệu
---
