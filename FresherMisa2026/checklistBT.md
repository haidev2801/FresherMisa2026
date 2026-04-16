## Bài Tập Cấp Độ 1: Beginner

### Task 1.1: Hoàn Thiện CRUD cho Position (1 điểm)

**Yêu cầu:**
- Tạo `PositionRepository` kế thừa `BaseRepository<Position>` - DONE
- Tạo `PositionService` kế thừa `BaseService<Position>` - DONE
- Tạo `PositionsController` kế thừa `BaseController<Position>` - DONE
- Đăng ký DI trong `ServiceExtensions.cs` - DONE

**Kiểm tra:**
- [X] API `GET /api/Positions` trả về danh sách position
- [X] API `GET /api/Positions/{id}` trả về position theo id
- [X] API `POST /api/Positions` thêm mới position
    fix lỗi: 
        sai utf dtb:    ALTER TABLE Position
                        CONVERT TO CHARACTER SET utf8mb4
                        COLLATE utf8mb4_0900_ai_ci; 
        Thiếu tạo ID trong repo => luôn lấy id 00000-000.. mặc định
- [X] API `PUT /api/Positions/{id}` cập nhật position
    fix lỗi: setid sau set param nên id không lấy được
- [X] API `DELETE /api/Positions/{id}` xóa position


### Task 1.2: Hoàn Thiện CRUD cho Employee (1 điểm)

**Yêu cầu:**
- Tạo `EmployeeRepository` kế thừa `BaseRepository<Employee>` - DONE
- Tạo `EmployeeService` kế thừa `BaseService<Employee>` - DONE
- Tạo `EmployeesController` kế thừa `BaseController<Employee>` - DONE
- Đăng ký DI trong `ServiceExtensions.cs` - DONE

**Kiểm tra:**
- [X] API `GET /api/Employees` trả về danh sách employee
- [X] API `GET /api/Employees/{id}` trả về employee theo id
- [X] API `POST /api/Employees` thêm mới employee
    fix lỗi: 
        sai utf dtb:    ALTER TABLE employee
                        CONVERT TO CHARACTER SET utf8mb4
                        COLLATE utf8mb4_0900_ai_ci; 
- [X] API `PUT /api/Employees/{id}` cập nhật employee
- [X] API `DELETE /api/Employees/{id}` xóa employee


## Bài Tập Cấp Độ 2: Intermediate

### Task 2.1: Thêm Validation cho Employee (2 điểm)

**Yêu cầu:**
- Thêm `[IRequired]` attribute cho các trường bắt buộc:
  - `EmployeeCode` - Mã nhân viên - DONE
  - `EmployeeName` - Tên nhân viên - DONE
  - `DepartmentID` - Phòng ban - DONE
  - `PositionID` - Vị trí - DONE
- Override method `ValidateCustom()` trong `EmployeeService` để thêm các validation tùy chỉnh:
  - Mã nhân viên không được trùng lặp - DONE
  - Email phải đúng định dạng (nếu có) - DONE
  - Số điện thoại phải đúng định dạng (nếu có) - DONE
  - Ngày sinh phải nhỏ hơn ngày hiện tại - DONE

**Kiểm tra:**
- [X] POST employee thiếu các trường bắt buộc → trả về lỗi 400 với thông báo rõ ràng
- [X] POST employee có mã trùng lặp → trả về lỗi "Mã nhân viên đã tồn tại"
- [X] POST employee có email sai định dạng → trả về lỗi "Email không đúng định dạng" -->

### Task 2.2: Tạo API Lọc Employee Theo Điều Kiện (2 điểm)

**Yêu cầu:**
- Thêm endpoint `GET /api/Employees/filter` với các tham số:
  - `departmentId` - Lọc theo phòng ban
  - `positionId` - Lọc theo vị trí
  - `salaryFrom` - Lọc lương từ
  - `salaryTo` - Lọc lương đến
  - `gender` - Lọc theo giới tính (0: Nam, 1: Nữ, 2: Khác)
  - `hireDateFrom` - Ngày vào làm từ
  - `hireDateTo` - Ngày vào làm đến

**Kiểm tra:**
- [X] Gọi API với `departmentId` hợp lệ → trả về danh sách nhân viên trong phòng ban đó
- [X] Gọi API với nhiều điều kiện lọc → trả về kết quả đúng

### Task 2.3: Thêm Custom Endpoint Cho Department (2 điểm)

**Yêu cầu:**
- Thêm endpoint `GET /api/Departments/{code}/employees` - Lấy danh sách nhân viên theo mã phòng ban
- Thêm endpoint `GET /api/Departments/{code}/employee-count` - Đếm số nhân viên trong phòng ban

**Kiểm tra:**
- [X] `GET /api/Departments/DV/employees` → trả về danh sách nhân viên phòng ban DV
- [X] `GET /api/Departments/DV/employee-count` → trả về số lượng nhân viên
