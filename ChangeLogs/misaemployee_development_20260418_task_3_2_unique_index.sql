-- =============================================
-- Task 3.2: Tạo unique index cho EmployeeCode
-- Cơ sở dữ liệu: misaemployee_development
-- Mục đích: Ngăn race condition khi thêm nhân viên bị trùng mã
-- =============================================

USE misaemployee_development;

-- Tạo unique index an toàn nếu chưa tồn tại
SET @index_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'Employee'
      AND index_name = 'UX_Employee_EmployeeCode'
);

SET @sql := IF(
    @index_exists = 0,
    'ALTER TABLE Employee ADD UNIQUE INDEX UX_Employee_EmployeeCode (EmployeeCode)',
    'SELECT 1'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Câu lệnh kiểm tra index (tùy chọn)
SELECT index_name, column_name, non_unique
FROM information_schema.statistics
WHERE table_schema = DATABASE()
  AND table_name = 'Employee'
  AND index_name = 'UX_Employee_EmployeeCode';
