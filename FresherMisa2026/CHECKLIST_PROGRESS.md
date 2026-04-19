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

---
