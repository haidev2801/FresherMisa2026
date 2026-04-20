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
 Git commit: "refactor(task-3.1): optimize BaseRepository with connection pooling and caching"

 - **Task 3.2:  Xử Lý Race Condition trong Validate  (2 điểm)**

 - [x] Vấn đề: Khi nhiều request cùng tạo hoặc cập nhật nhân viên với cùng mã nhân viên, có thể xảy ra
 - [x] Thêm xử lý trong insertAsyc trong BaseService để kiểm tra tồn tại mã nhân viên trước khi insert, nếu đã tồn tại thì trả về lỗi
 - [x] Thêm UniqueMessage cho trường EmployeeCode để đảm bảo ở tầng database cũng không cho phép trùng lặp
 - [x] Test đồng thời nhiều request POST với cùng `EmployeeCode` → chỉ một request thành công, các request còn lại trả về lỗi "Mã nhân viên đã tồn tại"
 Git commit: "fix(task-3.2): handle race condition for employee code validation"

 - [x] **Task 3.3:  Thêm Paging Cho Employee Filter (2 điểm)**
 - [x] Thêm các tham số `pageNumber` và `pageSize` vào endpoint `GET /api/employees/filter`
 - [x] Reponse trả về thêm thông tin paging: tổng số trang, trang hiện tại
 - [x] Test gọi API với paging → trả về đúng số lượng bản ghi theo pageSize và thông tin paging chính xác
 Git commit: "feat(task-3.3): add paging to employee filter API"

 - **Task 3.4:Tối Ưu SQL Query với Index (2 điểm)**
 - Index đã tạo
 - [x]IX_Employee_DepartmentID → (DepartmentID)
 - [x]IX_Employee_PositionID → (PositionID)
 - [x]IX_Employee_Department_Position_Gender → (DepartmentID, PositionID, Gender)
 - [x]IX_Employee_HireDate → (HireDate)
 - [x]IX_Employee_CreatedDate → (CreatedDate)
 - [x]EmployeeCode đã có sẵn unique index UQ_EmployeeCode
 - Giải thích ngắn gọn
 - DepartmentID: tăng tốc filter theo phòng ban và join với bảng department.
 - PositionID: tăng tốc filter theo vị trí/chức vụ.
 - EmployeeCode: đã có unique index → vừa đảm bảo không trùng, vừa hỗ trợ tìm kiếm theo mã.
 - Composite (DepartmentID, PositionID, Gender): tối ưu truy vấn filter nhiều điều kiện thường dùng.
 - HireDate: tăng tốc filter theo khoảng ngày vào làm.
 - CreatedDate: hỗ trợ sort và paging (ORDER BY).
 - Kết luận
   Các index được tạo dựa trên các cột xuất hiện nhiều trong WHERE, JOIN, ORDER BY, giúp:
   giảm full table scan tăng tốc truy vấn filter tối ưu phân trang và sắp xếp dữ liệu
 Git commit: "feat(task-3.4): optimize SQL queries with indexes"
---
