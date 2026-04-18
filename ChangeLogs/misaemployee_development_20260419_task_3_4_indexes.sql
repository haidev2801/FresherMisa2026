-- =============================================
-- Task 3.4: Tối ưu SQL Query với Index
-- Cơ sở dữ liệu: misaemployee_development
-- Mục đích: tăng tốc lọc, tìm kiếm và paging trên bảng Employee
-- =============================================

USE misaemployee_development;

-- CREATE INDEX idx_employee_department
-- ON Employee (DepartmentID);
-- => Tạo index cho DepartmentID
-- -> Giảm số dòng cần đọc khi filter theo phòng ban
SET @index_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'Employee'
            AND index_name = 'idx_employee_department'
);

SET @sql := IF(
    @index_exists = 0,
        'ALTER TABLE Employee ADD INDEX idx_employee_department (DepartmentID)',
    'SELECT 1'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- CREATE INDEX idx_employee_position
-- ON Employee (PositionID);
-- => Tạo index cho PositionID
-- -> Tìm nhanh nhân viên theo vị trí
SET @index_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'Employee'
            AND index_name = 'idx_employee_position'
);

SET @sql := IF(
    @index_exists = 0,
        'ALTER TABLE Employee ADD INDEX idx_employee_position (PositionID)',
    'SELECT 1'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ALTER TABLE Employee
-- ADD CONSTRAINT uq_employee_code
-- UNIQUE (EmployeeCode);
-- => Tạo constraint/unique index cho EmployeeCode
-- -> Kiểm tra trùng mã nhân viên và phục vụ race condition ở Task 3.2
SET @index_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'Employee'
            AND index_name = 'uq_employee_code'
);

SET @sql := IF(
    @index_exists = 0,
        'ALTER TABLE Employee ADD CONSTRAINT uq_employee_code UNIQUE (EmployeeCode)',
    'SELECT 1'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- CREATE INDEX idx_employee_filter
-- ON Employee (DepartmentID, PositionID, CreatedDate);
-- => Tạo composite index cho truy vấn filter phổ biến
-- -> Tìm trực tiếp theo DepartmentID/PositionID và hỗ trợ sort theo CreatedDate
SET @index_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'Employee'
            AND index_name = 'idx_employee_filter'
);

SET @sql := IF(
    @index_exists = 0,
        'ALTER TABLE Employee ADD INDEX idx_employee_filter (DepartmentID, PositionID, CreatedDate)',
    'SELECT 1'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check index đã được tạo thành công
SELECT index_name, column_name, non_unique, seq_in_index
FROM information_schema.statistics
WHERE table_schema = DATABASE()
  AND table_name = 'Employee'
  AND index_name IN (
            'idx_employee_department',
            'idx_employee_position',
            'uq_employee_code',
            'idx_employee_filter'
  )
ORDER BY index_name, seq_in_index;

-- EXPLAIN mẫu để kiểm tra tác dụng của composite index:
-- EXPLAIN
-- SELECT *
-- FROM Employee
-- WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440002'
--   AND PositionID = '11111111-1111-1111-1111-111111111111'
-- ORDER BY CreatedDate DESC;