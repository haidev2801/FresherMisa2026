
EXPLAIN SELECT * FROM department where DepartmentID = "550e8400-e29b-41d4-a716-446655440000";

EXPLAIN SELECT * FROM employee where EmployeeCode = "EMP00101013131244444";

EXPLAIN SELECT * FROM position where PositionID = "11111111-1111-1111-1111-111111111111";

EXPLAIN SELECT *
FROM employee
WHERE DepartmentID = "550e8400-e29b-41d4-a716-446655440000" AND PositionID = "11111111-1111-1111-1111-111111111111";

CREATE INDEX idx_employee_department_position
ON employee (DepartmentID, PositionID);

CREATE INDEX uq_employee_code ON employee (EmployeeCode);
