-- =============================================================================
-- Task 3.4 — Index tối ưu bảng employee (MySQL / MariaDB)
-- Chạy một lần trên DB misaemployee_development (hoặc đổi USE phù hợp).
-- Nếu báo "Duplicate key name", index đã tồn tại — bỏ dòng tương ứng hoặc DROP INDEX rồi chạy lại.
-- =============================================================================
--
-- NOTE NGẮN (deliverable)
--
-- | Index / vùng phủ              | Phục vụ truy vấn |
-- |--------------------------------|------------------|
-- | DepartmentID                   | Employee.GetByDepartmentId; tiền tố hỗ trợ lọc theo phòng |
-- | PositionID                     | Employee.GetByPositionId |
-- | EmployeeCode                   | Employee.GetByCode — THƯỜNG ĐÃ CÓ UK_employee_EmployeeCode (Task 3.2); UK vừa chống trùng vừa phục vụ lookup theo mã |
-- | (DepartmentID, PositionID, IsDeleted) | Employee.GetByFilter + các list theo dept/pos có IsDeleted=FALSE |
-- | (DepartmentID, HireDate, IsDeleted) | Employee.GetByFilter có khoảng hireDateFrom/To trên HireDate |
--
-- Trade-off (ghi/chèn):
-- - Đọc / filter / JOIN theo cột trong index: nhanh hơn (ít quét bảng).
-- - INSERT/UPDATE/DELETE: MySQL phải cập nhật mọi index liên quan → ghi chậm hơn nhẹ; tốn thêm dung lượng đĩa/RAM.
-- - Nhiều index chồng cột đầu (vd vừa riêng DepartmentID vừa composite bắt đầu DepartmentID): lợi ích đọc tăng ít, chi phí ghi tăng — production có thể bỏ index đơn DepartmentID nếu đã có composite (DepartmentID, PositionID, IsDeleted).
--
-- =============================================================================

USE misaemployee_development;

-- ---------------------------------------------------------------------------
-- Cột đơn (theo yêu cầu bài)
-- ---------------------------------------------------------------------------
CREATE INDEX idx_employee_departmentid ON employee (DepartmentID);
CREATE INDEX idx_employee_positionid ON employee (PositionID);

-- EmployeeCode: nếu đã tạo UNIQUE UK_employee_EmployeeCode (Task 3.2) thì KHÔNG chạy dòng dưới (trùng cột, thừa chi phí ghi).
-- CREATE INDEX idx_employee_employeecode ON employee (EmployeeCode);

-- ---------------------------------------------------------------------------
-- Composite theo filter thực tế (Query.json: điều kiện dept + pos + IsDeleted)
-- ---------------------------------------------------------------------------
CREATE INDEX idx_employee_dept_pos_isdeleted ON employee (DepartmentID, PositionID, IsDeleted);

-- Composite: lọc theo phòng + khoảng ngày vào làm
CREATE INDEX idx_employee_dept_hire_isdeleted ON employee (DepartmentID, HireDate, IsDeleted);
