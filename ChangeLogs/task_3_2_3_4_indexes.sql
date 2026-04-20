-- =============================================
-- Task 3.2: Unique Index cho EmployeeCode
-- Chống race condition khi nhiều request thêm mới cùng lúc
-- =============================================

USE misaemployee_development;

-- Thêm PRIMARY KEY cho bảng employee (nếu chưa có)
ALTER TABLE employee ADD PRIMARY KEY (EmployeeID);

-- Thêm UNIQUE INDEX cho EmployeeCode
-- Đảm bảo ở tầng database không cho phép trùng mã nhân viên
ALTER TABLE employee 
ADD UNIQUE INDEX UQ_EmployeeCode (EmployeeCode);

-- =============================================
-- Task 3.4: Indexes tối ưu performance
-- =============================================

-- Index trên DepartmentID (cho filter theo phòng ban)
-- Sử dụng khi: GET /api/Employees/filter?departmentId=xxx
-- JOIN Employee với Department
ALTER TABLE employee 
ADD INDEX IX_Employee_DepartmentID (DepartmentID);

-- Index trên PositionID (cho filter theo vị trí)
-- Sử dụng khi: GET /api/Employees/filter?positionId=xxx
ALTER TABLE employee 
ADD INDEX IX_Employee_PositionID (PositionID);

-- Index trên Gender (cho filter theo giới tính)
ALTER TABLE employee 
ADD INDEX IX_Employee_Gender (Gender);

-- Index trên Salary (cho filter theo khoảng lương)
-- Giúp tối ưu range query: Salary >= @SalaryFrom AND Salary <= @SalaryTo
ALTER TABLE employee 
ADD INDEX IX_Employee_Salary (Salary);

-- Index trên CreatedDate (cho filter theo ngày vào làm)
ALTER TABLE employee 
ADD INDEX IX_Employee_CreatedDate (CreatedDate);

-- Composite index cho các truy vấn thường dùng
-- Khi filter kết hợp phòng ban + chức vụ (trường hợp phổ biến nhất)
ALTER TABLE employee 
ADD INDEX IX_Employee_Dept_Position (DepartmentID, PositionID);

-- Composite index cho filter phòng ban + lương (báo cáo lương theo phòng ban)
ALTER TABLE employee 
ADD INDEX IX_Employee_Dept_Salary (DepartmentID, Salary);
