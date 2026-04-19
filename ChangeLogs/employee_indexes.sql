-- ============================================================
-- Task 3.4: Tối Ưu SQL Query với Index cho bảng Employee
-- Ngày tạo: 2026-04-19
-- Tác giả: HaThanhChuc
-- ============================================================

USE misaemployee_development;

-- ============================================================
-- 1. INDEX TRÊN DepartmentID
-- ============================================================
-- Lý do: API GET /api/Employees/filter?departmentId=xxx 
-- và GET /api/Departments/{code}/employees đều lọc theo DepartmentID.
-- Không có index → MySQL phải quét TOÀN BỘ bảng (Full Table Scan).
-- Có index → MySQL nhảy thẳng tới các dòng có DepartmentID tương ứng.
-- EXPLAIN trước: type=ALL, key=NULL
-- EXPLAIN sau:   type=ref, key=IDX_Employee_DepartmentID
-- ============================================================
CREATE INDEX IDX_Employee_DepartmentID ON employee(DepartmentID);

-- ============================================================
-- 2. INDEX TRÊN PositionID
-- ============================================================
-- Lý do: API GET /api/Employees/filter?positionId=xxx lọc theo PositionID.
-- Tương tự DepartmentID, index giúp MySQL tìm nhanh nhân viên theo vị trí
-- mà không cần quét toàn bộ bảng.
-- EXPLAIN trước: type=ALL, key=NULL
-- EXPLAIN sau:   type=ref, key=IDX_Employee_PositionID
-- ============================================================
CREATE INDEX IDX_Employee_PositionID ON employee(PositionID);

-- ============================================================
-- 3. INDEX TRÊN EmployeeCode (đã có UNIQUE INDEX từ Task 3.2)
-- ============================================================
-- Lý do: API GET /api/Employees/Code/{code} tìm nhân viên theo mã.
-- Đã tạo UNIQUE INDEX UQ_Employee_EmployeeCode trong Task 3.2
-- → Vừa đảm bảo không trùng mã, vừa tăng tốc truy vấn.
-- EXPLAIN: type=const, key=UQ_Employee_EmployeeCode (tối ưu nhất)
-- → KHÔNG cần tạo thêm index mới cho EmployeeCode.

-- ============================================================
-- 4. COMPOSITE INDEX cho filter thường dùng
-- ============================================================
-- Lý do: API filter thường kết hợp DepartmentID + PositionID + Gender
-- Ví dụ: GET /api/Employees/filter?departmentId=xxx&positionId=yyy&gender=1
-- Composite index giúp MySQL sử dụng 1 index duy nhất cho nhiều điều kiện
-- thay vì chỉ dùng được 1 index rồi quét phần còn lại.
-- Thứ tự cột: DepartmentID (phổ biến nhất) → PositionID → Gender
-- MySQL sử dụng index theo thứ tự từ trái sang phải (leftmost prefix).
-- EXPLAIN trước: type=ALL, key=NULL
-- EXPLAIN sau:   type=ref, key=IDX_Employee_Dept_Pos_Gender
-- ============================================================
CREATE INDEX IDX_Employee_Dept_Pos_Gender ON employee(DepartmentID, PositionID, Gender);

-- ============================================================
-- 5. INDEX CHO SALARY RANGE (filter lương)
-- ============================================================
-- Lý do: API filter?salaryFrom=xxx&salaryTo=yyy dùng range query trên Salary.
-- Index trên Salary giúp MySQL dùng range scan thay vì full table scan.
-- EXPLAIN trước: type=ALL, key=NULL
-- EXPLAIN sau:   type=range, key=IDX_Employee_Salary
-- ============================================================
CREATE INDEX IDX_Employee_Salary ON employee(Salary);

-- ============================================================
-- KIỂM TRA INDEX ĐÃ TẠO
-- ============================================================
SHOW INDEX FROM employee;

-- ============================================================
-- KIỂM TRA BẰNG EXPLAIN (chạy từng lệnh để cap màn hình)
-- ============================================================

-- Test 1: Filter theo DepartmentID (CÓ index)
-- EXPLAIN SELECT * FROM employee WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440002';

-- Test 2: Filter theo PositionID (CÓ index)
-- EXPLAIN SELECT * FROM employee WHERE PositionID = '11111111-1111-1111-1111-111111111111';

-- Test 3: Tìm theo EmployeeCode (CÓ UNIQUE index)
-- EXPLAIN SELECT * FROM employee WHERE EmployeeCode = 'EMP001';

-- Test 4: Filter kết hợp DepartmentID + PositionID + Gender (CÓ composite index)
-- EXPLAIN SELECT * FROM employee WHERE DepartmentID = '550e8400-e29b-41d4-a716-446655440002' AND PositionID = '11111111-1111-1111-1111-111111111111' AND Gender = 1;

-- Test 5: Filter Salary range (CÓ index)
-- EXPLAIN SELECT * FROM employee WHERE Salary >= 5000000 AND Salary <= 20000000;

-- Test 6: Filter không có index - LIKE trên tên (KHÔNG CÓ index → Full Table Scan)
-- EXPLAIN SELECT * FROM employee WHERE EmployeeName LIKE '%Nguyễn%';
