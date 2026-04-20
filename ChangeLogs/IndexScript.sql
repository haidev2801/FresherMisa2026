CREATE UNIQUE INDEX idx_employee_code 
ON employee(EmployeeCode);

CREATE INDEX idx_employee_department 
ON employee(DepartmentID);

CREATE INDEX idx_employee_position 
ON employee(PositionID);

CREATE INDEX idx_employee_dept_pos_created 
ON employee(DepartmentID, PositionID, CreatedDate DESC);