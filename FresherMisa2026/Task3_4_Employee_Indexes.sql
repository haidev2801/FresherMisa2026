-- 1. Unique Index cho EmployeeCode
-- 2. Index cho DepartmentID và IsDeleted
-- Mục tiêu: Tối ưu các truy vấn lọc theo phòng ban (GetEmployeesByDepartmentId) 
-- và các câu lệnh JOIN trong DepartmentRepository.
CREATE INDEX IX_Employee_DepartmentID_IsDeleted ON Employee(DepartmentID, IsDeleted);

-- 3. Index cho PositionID và IsDeleted
-- Mục tiêu: Tối ưu truy vấn lọc theo vị trí (GetEmployeesByPositionId).
CREATE INDEX IX_Employee_PositionID_IsDeleted ON Employee(PositionID, IsDeleted);

-- 4. Composite Index cho Bộ lọc Phân trang (Filter & Paging)
-- Mục tiêu: Tối ưu Task 2.2 và 3.3. 
-- Khi lọc theo giới tính và luôn lấy các bản ghi chưa xóa, sắp xếp theo ngày tạo mới nhất.
CREATE INDEX IX_Employee_Gender_IsDeleted_CreatedDate ON Employee(Gender, IsDeleted, CreatedDate DESC);

-- 5. Index hỗ trợ sắp xếp mặc định
-- Mục tiêu: Hầu hết các câu lệnh GetEntities và Filter đều có 'ORDER BY CreatedDate DESC'.
-- Index này giúp Database không phải thực hiện thao tác Sort trên bộ nhớ (FileSort).
CREATE INDEX IX_Employee_CreatedDate ON Employee(CreatedDate DESC);

-- 6. Index cho các trường lọc theo khoảng (Range Filters)
-- Lưu ý: Với Salary và HireDate, chúng ta thường lọc theo khoảng nên việc đánh index riêng lẻ 
-- sẽ giúp Optimizer của MySQL thực hiện Index Range Scan hiệu quả hơn.
CREATE INDEX IX_Employee_Salary ON Employee(Salary);
CREATE INDEX IX_Employee_HireDate ON Employee(HireDate);

-- Kiểm tra lại danh sách Index sau khi tạo
-- SHOW INDEX FROM Employee;
