DELIMITER $$

CREATE PROCEDURE GetEmployeeCountByDepartmentCode (
    IN p_DepartmentCode VARCHAR(50)
)
BEGIN
    SELECT 
        COUNT(e.EmployeeId) AS TotalEmployee
    FROM Department d
    LEFT JOIN Employee e 
        ON e.DepartmentID = d.DepartmentID
    WHERE d.DepartmentCode = p_DepartmentCode;
END $$

DELIMITER ;