-- Task 3.4 - Toi uu SQL Query voi Index
-- Muc tieu:
-- 1) Tang toc filter Employee theo DepartmentID
-- 2) Tang toc filter Employee theo PositionID
-- 3) Tang toc query ket hop DepartmentID + PositionID + sap xep EmployeeID
-- (Ghi chu: EmployeeCode da co unique index UQ_EmployeeCode tu task 3.2)

USE misaemployee_development;

-- ============================================================
-- A. Tao index can thiet cho bang Employee
-- ============================================================

-- 1) Index cho filter theo phong ban
SET @has_department_index = (
	SELECT COUNT(*)
	FROM information_schema.STATISTICS
	WHERE TABLE_SCHEMA = DATABASE()
	  AND TABLE_NAME = 'employee'
	  AND INDEX_NAME = 'IDX_Employee_DepartmentID'
);

SET @sql_department_index = IF(
	@has_department_index = 0,
	'CREATE INDEX IDX_Employee_DepartmentID ON Employee(DepartmentID)',
	'SELECT ''Skip create IDX_Employee_DepartmentID because it already exists'''
);

PREPARE stmt_department_index FROM @sql_department_index;
EXECUTE stmt_department_index;
DEALLOCATE PREPARE stmt_department_index;

-- 2) Index cho filter theo vi tri
SET @has_position_index = (
	SELECT COUNT(*)
	FROM information_schema.STATISTICS
	WHERE TABLE_SCHEMA = DATABASE()
	  AND TABLE_NAME = 'employee'
	  AND INDEX_NAME = 'IDX_Employee_PositionID'
);

SET @sql_position_index = IF(
	@has_position_index = 0,
	'CREATE INDEX IDX_Employee_PositionID ON Employee(PositionID)',
	'SELECT ''Skip create IDX_Employee_PositionID because it already exists'''
);

PREPARE stmt_position_index FROM @sql_position_index;
EXECUTE stmt_position_index;
DEALLOCATE PREPARE stmt_position_index;

-- 3) Composite index cho query thuong dung:
-- WHERE DepartmentID = ? AND PositionID = ? ORDER BY EmployeeID DESC
SET @has_composite_index = (
	SELECT COUNT(*)
	FROM information_schema.STATISTICS
	WHERE TABLE_SCHEMA = DATABASE()
	  AND TABLE_NAME = 'employee'
	  AND INDEX_NAME = 'IDX_Employee_Department_Position_Employee'
);

SET @sql_composite_index = IF(
	@has_composite_index = 0,
	'CREATE INDEX IDX_Employee_Department_Position_Employee ON Employee(DepartmentID, PositionID, EmployeeID)',
	'SELECT ''Skip create IDX_Employee_Department_Position_Employee because it already exists'''
);

PREPARE stmt_composite_index FROM @sql_composite_index;
EXECUTE stmt_composite_index;
DEALLOCATE PREPARE stmt_composite_index;


-- ============================================================
-- B. 4 test query de kiem chung (EXPLAIN)
-- ============================================================
-- Su dung gia tri mau co san trong DB
SET @DepartmentID = (SELECT DepartmentID FROM Department LIMIT 1);
SET @PositionID   = (SELECT PositionID FROM Position LIMIT 1);
SET @DepartmentCode = (SELECT DepartmentCode FROM Department LIMIT 1);
SET @EmployeeCode = (SELECT EmployeeCode FROM Employee LIMIT 1);

-- Test 1: Filter theo phong ban
EXPLAIN
SELECT e.*
FROM Employee e
WHERE e.DepartmentID = @DepartmentID;

-- Test 2: Filter theo vi tri
EXPLAIN
SELECT e.*
FROM Employee e
WHERE e.PositionID = @PositionID;

-- Test 3: Filter phong ban + vi tri + sap xep
EXPLAIN
SELECT e.*
FROM Employee e
WHERE e.DepartmentID = @DepartmentID
	AND e.PositionID = @PositionID
ORDER BY e.EmployeeID DESC
LIMIT 20 OFFSET 0;

-- Test 4: Join Department + Employee theo ma phong ban
EXPLAIN
SELECT e.*
FROM Employee e
INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
WHERE d.DepartmentCode = @DepartmentCode
ORDER BY e.EmployeeID DESC
LIMIT 20 OFFSET 0;
