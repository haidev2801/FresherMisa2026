-- Task 3.4: Tạo Index tối ưu cho bảng Employee
-- Database: misaemployee_development
-- Created: 19/04/2026
-- Author: PhamXuanCuong
--
-- Mục đích: Khi bảng Employee có nhiều dữ liệu, các câu query lọc/tìm kiếm
-- sẽ phải duyệt toàn bộ bảng (full table scan) → rất chậm.
-- Index giúp MySQL giúp tìm kiếm nhanh hơn, không cần đọc hết bảng.
-- =====================================================================


-- ---------------------------------------------------------------------
-- 1. UNIQUE INDEX trên EmployeeCode
-- ---------------------------------------------------------------------
-- Mã nhân viên — không được trùng.
--
-- Trong code hiện tại có 2 chỗ query theo EmployeeCode:
--   • GetEmployeeByCode()  → tìm nhân viên theo mã để trả về API
--   • ValidateCustom()     → kiểm tra xem mã đã tồn tại chưa trước khi thêm/sửa
--
-- Nếu không có index, mỗi lần gọi 2 hàm trên, MySQL phải đọc qua
-- toàn bộ bảng để tìm → O(n).
-- Với UNIQUE INDEX, MySQL dùng B-tree nên tìm được ngay → O(log n).
--
-- UNIQUE còn có tác dụng: đảm bảo ràng buộc ở tầng database.
-- Dù application có bug hay 2 request cùng lúc cũng không thể insert trùng mã.
-- (Đây chính là cơ sở để fix race condition ở Task 3.2)
-- ---------------------------------------------------------------------
CREATE UNIQUE INDEX idx_employee_code
    ON employee (EmployeeCode);


-- ---------------------------------------------------------------------
-- 2. INDEX trên DepartmentID
-- ---------------------------------------------------------------------
-- DepartmentID xuất hiện rất nhiều trong các query lọc nhân viên:
--   • Lọc nhân viên theo phòng ban     → WHERE DepartmentID = @id
--   • API filter nhiều điều kiện       → WHERE DepartmentID = @id AND ...
--   • Lấy nhân viên theo mã phòng ban  → JOIN với bảng Department theo DepartmentID
--
-- Vì DepartmentID là foreign key, thường xuyên được dùng trong WHERE và JOIN,
-- nên được đánh index sau EmployeeCode.
-- ---------------------------------------------------------------------
CREATE INDEX idx_employee_departmentid
    ON employee (DepartmentID);


-- ---------------------------------------------------------------------
-- 3. INDEX trên PositionID
-- ---------------------------------------------------------------------
-- Tương tự DepartmentID, PositionID cũng được dùng để lọc nhân viên:
--   • Lấy nhân viên theo vị trí công việc  → WHERE PositionID = @id
--   • API filter kết hợp nhiều điều kiện   → WHERE ... AND PositionID = @id
--
-- Đây cũng là foreign key, tần suất filter cao → cần index để tránh full scan.
-- ---------------------------------------------------------------------
CREATE INDEX idx_employee_positionid
    ON employee (PositionID);


-- ---------------------------------------------------------------------
-- 4. COMPOSITE INDEX trên (DepartmentID, PositionID)
-- ---------------------------------------------------------------------
-- API FilterEmployees() cho phép lọc đồng thời cả phòng ban lẫn vị trí:
--   WHERE DepartmentID = ? AND PositionID = ?
--
-- Vấn đề: MySQL chỉ dùng được 1 index đơn cho mỗi lần query.
-- Nếu chỉ có 2 index riêng lẻ ở trên, MySQL sẽ chọn 1 trong 2,
-- còn cột kia vẫn phải scan → không tối ưu.
--
-- Composite index gộp cả 2 cột vào 1 index → MySQL xử lý cả 2 điều kiện
-- trong 1 lần tra cứu duy nhất, nhanh hơn đáng kể.
--
-- Lưu ý về "leftmost prefix rule" — index này hỗ trợ các trường hợp:
--   WHERE DepartmentID = ?                    → dùng được (lấy cột đầu tiên)
--   WHERE DepartmentID = ? AND PositionID = ? → dùng được (dùng cả 2 cột)
--   WHERE PositionID = ?                      → KHÔNG dùng được
--     (vì PositionID không phải cột đứng đầu trong composite index)
--
-- Vì vậy idx_employee_positionid ở index 3 vẫn cần giữ lại
-- để xử lý trường hợp chỉ lọc theo PositionID.
-- ---------------------------------------------------------------------
CREATE INDEX idx_employee_dept_pos
    ON employee (DepartmentID, PositionID);
