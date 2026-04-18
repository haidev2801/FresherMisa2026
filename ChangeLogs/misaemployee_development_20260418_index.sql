USE misaemployee_development;

/*
    Employee indexes for Task 3.4
    Mục tiêu:
    - Tối ưu các query filter trong EmployeeRepository
    - Giữ script an toàn khi chạy lặp lại nhiều lần
*/

/*
    1. Unique index cho EmployeeCode
    Dùng cho:
    - tìm theo EmployeeCode
    - chống duplicate/race condition khi insert employee
    Lợi ích:
    - tra cứu theo mã nhân viên nhanh hơn
    - database-level constraint bảo vệ dữ liệu

    Ghi chú:
    - Unique index này đã được thêm trong changelog Task 3.2 (UQ_EmployeeCode)
    - Ở đây chỉ comment lại để người review thấy rõ vai trò của nó
*/
-- ALTER TABLE employee ADD UNIQUE INDEX UQ_EmployeeCode (EmployeeCode);

/*
    2. Index trên DepartmentID
    Dùng cho:
    - GET /api/Employees/Department/{departmentId}
    - filter employee theo DepartmentId
    Lợi ích:
    - giảm full scan khi lọc nhân viên theo phòng ban
*/
SET @idx_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IX_Employee_DepartmentID'
);

SET @sql := IF(
    @idx_exists = 0,
    'ALTER TABLE employee ADD INDEX IX_Employee_DepartmentID (DepartmentID);',
    'SELECT ''IX_Employee_DepartmentID already exists'';'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

/*
    3. Index trên PositionID
    Dùng cho:
    - GET /api/Employees/Position/{positionId}
    - filter employee theo PositionId
    Lợi ích:
    - giảm full scan khi lọc nhân viên theo vị trí
*/
SET @idx_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IX_Employee_PositionID'
);

SET @sql := IF(
    @idx_exists = 0,
    'ALTER TABLE employee ADD INDEX IX_Employee_PositionID (PositionID);',
    'SELECT ''IX_Employee_PositionID already exists'';'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

/*
    4. Index trên Gender
    Dùng cho:
    - filter employee theo Gender
    Lợi ích:
    - tối ưu điều kiện lọc giới tính khi kết hợp với các filter khác
    Lưu ý:
    - cột này có cardinality thấp, nhưng vẫn hữu ích trong các truy vấn filter API hiện tại
*/
SET @idx_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IX_Employee_Gender'
);

SET @sql := IF(
    @idx_exists = 0,
    'ALTER TABLE employee ADD INDEX IX_Employee_Gender (Gender);',
    'SELECT ''IX_Employee_Gender already exists'';'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

/*
    5. Index trên CreatedDate
    Dùng cho:
    - filter employee theo HireDateFrom / HireDateTo
    Lợi ích:
    - tối ưu range scan theo ngày tạo
    - đặc biệt hữu ích cho truy vấn dạng CreatedDate >= ... và CreatedDate < ...
*/
SET @idx_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IX_Employee_CreatedDate'
);

SET @sql := IF(
    @idx_exists = 0,
    'ALTER TABLE employee ADD INDEX IX_Employee_CreatedDate (CreatedDate);',
    'SELECT ''IX_Employee_CreatedDate already exists'';'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

/*
    6. Composite index trên (DepartmentID, PositionID)
    Dùng cho:
    - các truy vấn filter đồng thời theo DepartmentId và PositionId
    Lợi ích:
    - tối ưu tốt hơn so với chỉ dùng hai single-column index riêng lẻ
    - phù hợp với pattern filter nhiều điều kiện thường gặp ở EmployeeRepository
*/
SET @idx_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IX_Employee_DepartmentID_PositionID'
);

SET @sql := IF(
    @idx_exists = 0,
    'ALTER TABLE employee ADD INDEX IX_Employee_DepartmentID_PositionID (DepartmentID, PositionID);',
    'SELECT ''IX_Employee_DepartmentID_PositionID already exists'';'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
