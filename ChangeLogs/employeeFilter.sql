DELIMITER $$

CREATE PROCEDURE Proc_Employee_Filter(
    IN p_DepartmentId CHAR(36),
    IN p_PositionId CHAR(36),
    IN p_SalaryFrom DECIMAL(18,4),
    IN p_SalaryTo DECIMAL(18,4),
    IN p_Gender INT,
    IN p_HireDateFrom DATE,
    IN p_HireDateTo DATE,
    IN p_PageIndex INT,
    IN p_PageSize INT
)
BEGIN

    DECLARE v_Offset INT;
    SET v_Offset = (p_PageIndex - 1) * p_PageSize;

    SELECT 
        e.EmployeeID,
        e.EmployeeCode,
        e.EmployeeName,
        e.Gender,
        e.DateOfBirth,
        e.PhoneNumber,
        e.Email,
        e.Address,
        e.DepartmentID,
        e.PositionID,
        e.Salary,
        e.CreatedDate
    FROM employee e
    WHERE 
        (p_DepartmentId IS NULL OR e.DepartmentID = p_DepartmentId)
        AND (p_PositionId IS NULL OR e.PositionID = p_PositionId)
        AND (p_Gender IS NULL OR e.Gender = p_Gender)
        AND (p_SalaryFrom IS NULL OR e.Salary >= p_SalaryFrom)
        AND (p_SalaryTo IS NULL OR e.Salary <= p_SalaryTo)
        AND (p_HireDateFrom IS NULL OR DATE(e.CreatedDate) >= p_HireDateFrom)
        AND (p_HireDateTo IS NULL OR DATE(e.CreatedDate) <= p_HireDateTo)
    ORDER BY e.CreatedDate DESC
    LIMIT v_Offset, p_PageSize;

END $$

DELIMITER ;