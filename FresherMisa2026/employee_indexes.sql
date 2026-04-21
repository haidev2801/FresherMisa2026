-- Employee index optimization script for Task 3.4
-- Safe to run multiple times (idempotent).

USE misa_employee_development;

-- 1) Index for filtering by DepartmentID
SET @idx_department_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IDX_employee_DepartmentID'
);
SET @idx_department_sql := IF(
    @idx_department_exists = 0,
    'CREATE INDEX IDX_employee_DepartmentID ON employee (DepartmentID)',
    'SELECT 1'
);
PREPARE idx_department_stmt FROM @idx_department_sql;
EXECUTE idx_department_stmt;
DEALLOCATE PREPARE idx_department_stmt;

-- 2) Index for filtering by PositionID
SET @idx_position_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IDX_employee_PositionID'
);
SET @idx_position_sql := IF(
    @idx_position_exists = 0,
    'CREATE INDEX IDX_employee_PositionID ON employee (PositionID)',
    'SELECT 1'
);
PREPARE idx_position_stmt FROM @idx_position_sql;
EXECUTE idx_position_stmt;
DEALLOCATE PREPARE idx_position_stmt;

-- 3) Index for searching by EmployeeCode
-- If no index currently exists on EmployeeCode, create a unique index.
SET @idx_empcode_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND column_name = 'EmployeeCode'
);
SET @idx_empcode_sql := IF(
    @idx_empcode_exists = 0,
    'CREATE UNIQUE INDEX UX_employee_EmployeeCode ON employee (EmployeeCode)',
    'SELECT 1'
);
PREPARE idx_empcode_stmt FROM @idx_empcode_sql;
EXECUTE idx_empcode_stmt;
DEALLOCATE PREPARE idx_empcode_stmt;

-- 4) Composite index for common paging/filter query
-- Optimizes queries with DepartmentID + PositionID + ORDER BY CreatedDate, EmployeeCode.
SET @idx_composite_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'IDX_employee_Department_Position_CreatedDate_EmployeeCode'
);
SET @idx_composite_sql := IF(
    @idx_composite_exists = 0,
    'CREATE INDEX IDX_employee_Department_Position_CreatedDate_EmployeeCode ON employee (DepartmentID, PositionID, CreatedDate, EmployeeCode)',
    'SELECT 1'
);
PREPARE idx_composite_stmt FROM @idx_composite_sql;
EXECUTE idx_composite_stmt;
DEALLOCATE PREPARE idx_composite_stmt;

-- Refresh table statistics so optimizer can pick new indexes quickly.
ANALYZE TABLE employee;
