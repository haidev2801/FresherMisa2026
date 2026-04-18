-- ============================================================
-- Task 3.4: Tối Ưu SQL Query với Index
-- Database: misaemployee_development / Table: employee
-- ============================================================

-- ============================================================
-- INDEX 1: (DepartmentID, PositionID, EmployeeID DESC)
-- ============================================================
-- Ứng dụng quản lý nhân viên luôn có chức năng lọc theo phòng
-- ban + chức vụ — đây là filter phổ biến nhất.
--
-- Tại sao composite 3 cột thay vì tạo riêng từng cột?
--   • Leftmost prefix rule: (DepartmentID, PositionID) bao luôn
--     trường hợp chỉ filter mình DepartmentID → không cần tạo
--     single-column IDX_Employee_DepartmentID thừa.
--   • EmployeeID DESC ở cuối: ORDER BY EmployeeID DESC được giải
--     quyết trong index, MySQL không cần bước Sort riêng
--     → loại bỏ "Using filesort".
--
-- Bao phủ các trường hợp:
--   WHERE DepartmentID = ?
--   WHERE DepartmentID = ? AND PositionID = ?
--   WHERE DepartmentID = ? AND PositionID = ? ORDER BY EmployeeID DESC
-- ============================================================
CREATE INDEX `IDX_Employee_Dept_Pos`
  ON `employee` (`DepartmentID`, `PositionID`, `EmployeeID` DESC);

-- ============================================================
-- INDEX 2: (PositionID)
-- ============================================================
-- Leftmost prefix của INDEX 1 là DepartmentID, nên INDEX 1
-- chỉ dùng được khi có DepartmentID trong điều kiện.
-- Khi query chỉ có WHERE PositionID = ? (không có DepartmentID)
-- MySQL không thể dùng INDEX 1 → cần index riêng cho PositionID.
-- ============================================================
CREATE INDEX `IDX_Employee_PositionID`
  ON `employee` (`PositionID`);

-- ============================================================
-- INDEX 3: (DepartmentID, Salary)
-- ============================================================
-- Phục vụ truy vấn lọc theo phòng ban + khoảng lương:
--   WHERE DepartmentID = ? AND Salary BETWEEN ? AND ?
--
-- Tại sao equality trước, range sau?
--   • DepartmentID = ? (equality) đứng trước → MySQL thu hẹp
--     ngay xuống nhóm nhân viên của 1 phòng ban.
--   • Salary BETWEEN (range) đứng sau → quét range trong đoạn
--     nhỏ đã thu hẹp, không quét toàn bảng.
--   • Nếu đặt ngược (Salary, DepartmentID): MySQL chỉ dùng được
--     Salary range, DepartmentID bị bỏ qua → kém hiệu quả hơn.
--
-- Lưu ý: Gender (3 giá trị) không được thêm vào composite vì
-- cardinality quá thấp, MySQL thường bỏ qua.
-- ============================================================
CREATE INDEX `IDX_Employee_Dept_Salary`
  ON `employee` (`DepartmentID`, `Salary`);

-- ============================================================
-- INDEX 4: (DepartmentID, HireDate)
-- ============================================================
-- Phục vụ truy vấn lọc theo phòng ban + khoảng ngày vào làm:
--   WHERE DepartmentID = ? AND HireDate BETWEEN ? AND ?
--
-- Tách riêng khỏi INDEX 3 vì MySQL chỉ dùng được 1 range column
-- trong 1 composite index — không thể gộp (DepartmentID, Salary,
-- HireDate) và kỳ vọng cả Salary lẫn HireDate đều range scan.
-- ============================================================
CREATE INDEX `IDX_Employee_Dept_HireDate`
  ON `employee` (`DepartmentID`, `HireDate`);

-- ============================================================
-- INDEX 5: FULLTEXT (EmployeeName, EmployeeCode, Email, Address)
-- ============================================================
-- Chức năng tìm kiếm tên/email/địa chỉ dùng LIKE '%keyword%'
-- (wildcard 2 đầu) → B-Tree index hoàn toàn vô dụng vì không
-- biết điểm bắt đầu để tra cứu → full scan toàn bảng.
--
-- FULLTEXT dùng inverted index: tách text thành token, cho phép
-- tìm kiếm từ khóa trong chuỗi dài mà không cần full scan.
--
-- Sau khi tạo index này, proc nên đổi:
--   LIKE '%keyword%'
-- thành:
--   MATCH(EmployeeName, EmployeeCode, Email, Address)
--   AGAINST ('keyword' IN BOOLEAN MODE)
--
-- Lưu ý: EmployeeCode đã có UQ_EmployeeCode (unique constraint)
-- → MySQL tự tạo B-Tree index → không cần tạo thêm index riêng
-- cho tìm kiếm chính xác WHERE EmployeeCode = ?
-- ============================================================
ALTER TABLE `employee`
  ADD FULLTEXT INDEX `FT_Employee_Search`
    (`EmployeeName`, `EmployeeCode`, `Email`, `Address`);