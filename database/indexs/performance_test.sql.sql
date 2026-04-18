/*
  ════════════════════════════════════════════════════════════════════════════
  File  : performance_test.sql
  Mục đích : So sánh hiệu năng KHÔNG index vs CÓ index
  Query test: WHERE DepartmentID + PositionID, trang cuối (OFFSET 99950)
  Database : misaemployee_development
  ════════════════════════════════════════════════════════════════════════════
*/

USE `misaemployee_development`;


-- ════════════════════════════════════════════════════════════════════════════
-- PHẦN 1: KHÔNG CÓ INDEX
-- ════════════════════════════════════════════════════════════════════════════

CALL drop_index_if_exists('employee', 'IDX_Employee_Dept_Pos');
CALL drop_index_if_exists('employee', 'IDX_Employee_PositionID');
CALL drop_index_if_exists('employee', 'IDX_Employee_Dept_Salary');
CALL drop_index_if_exists('employee', 'IDX_Employee_Dept_HireDate');
CALL drop_index_if_exists('employee', 'FT_Employee_Search');

SHOW INDEX FROM `employee`;
FLUSH TABLES;

-- Kỳ vọng: type=ALL, key=NULL, rows=~100k, Using filesort
EXPLAIN ANALYZE
SELECT * FROM employee
WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440010'
  AND PositionID   = '11111111-1111-1111-1111-111111111111'
ORDER BY EmployeeID DESC
LIMIT 50 OFFSET 1750

SET profiling = 1;
SET profiling_history_size = 20;

SELECT /* có dữ liệu */ *
FROM employee
WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440010'
  AND PositionID   = '11111111-1111-1111-1111-111111111111'
ORDER BY EmployeeID DESC
LIMIT 50 OFFSET 1750;

SHOW PROFILES;
SHOW PROFILE CPU, BLOCK IO FOR QUERY 1;

SET profiling = 0;


-- ════════════════════════════════════════════════════════════════════════════
-- PHẦN 2: TẠO INDEX
-- ════════════════════════════════════════════════════════════════════════════

-- (DepartmentID, PositionID, EmployeeID DESC)
-- → filter + sort luôn trong 1 index, không cần filesort
CREATE INDEX `IDX_Employee_Dept_Pos`
  ON `employee` (`DepartmentID`, `PositionID`, `EmployeeID` DESC);

CREATE INDEX `IDX_Employee_PositionID`
  ON `employee` (`PositionID`);

CREATE INDEX `IDX_Employee_Dept_Salary`
  ON `employee` (`DepartmentID`, `Salary`);

CREATE INDEX `IDX_Employee_Dept_HireDate`
  ON `employee` (`DepartmentID`, `HireDate`);

ALTER TABLE `employee`
  ADD FULLTEXT INDEX `FT_Employee_Search`
    (`EmployeeName`, `EmployeeCode`, `Email`, `Address`);

SHOW INDEX FROM `employee`;


-- ════════════════════════════════════════════════════════════════════════════
-- PHẦN 3: CÓ INDEX
-- ════════════════════════════════════════════════════════════════════════════

FLUSH TABLES;

-- Kỳ vọng: type=ref, key=IDX_Employee_Dept_Pos, rows nhỏ, KHÔNG còn filesort
EXPLAIN ANALYZE
SELECT * FROM employee
WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440010'
  AND PositionID   = '11111111-1111-1111-1111-111111111111'
ORDER BY EmployeeID DESC
LIMIT 50 OFFSET 1750

SET profiling = 1;
SET profiling_history_size = 20;

SELECT /* PHẦN 3 — có index */ *
FROM employee
WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440010'
  AND PositionID   = '11111111-1111-1111-1111-111111111111'
ORDER BY EmployeeID DESC
LIMIT 50 OFFSET 99950;

SHOW PROFILES;
SHOW PROFILE CPU, BLOCK IO FOR QUERY 1;

SET profiling = 0;