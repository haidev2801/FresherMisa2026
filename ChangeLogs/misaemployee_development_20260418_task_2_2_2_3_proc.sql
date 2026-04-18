-- =============================================
-- Task 2.2 + Task 2.3 Stored Procedures
-- Database: misaemployee_development
-- =============================================

USE misaemployee_development;

DROP PROCEDURE IF EXISTS Proc_Employee_FilterByCondition;
DROP PROCEDURE IF EXISTS Proc_Employee_GetByDepartmentId;
DROP PROCEDURE IF EXISTS Proc_Employee_GetByPositionId;
DROP PROCEDURE IF EXISTS Proc_Employee_GetByDepartmentCode;
DROP PROCEDURE IF EXISTS Proc_Employee_GetCountByDepartmentCode;

DELIMITER $$

CREATE PROCEDURE Proc_Employee_FilterByCondition(
    IN p_DepartmentID VARCHAR(36),
    IN p_PositionID VARCHAR(36),
    IN p_SalaryFrom DECIMAL(18,2),
    IN p_SalaryTo DECIMAL(18,2),
    IN p_Gender INT,
    IN p_HireDateFrom DATE,
    IN p_HireDateTo DATE
)
BEGIN
    SELECT *
    FROM Employee e
    WHERE (p_DepartmentID IS NULL OR p_DepartmentID = '' OR e.DepartmentID = p_DepartmentID)
      AND (p_PositionID IS NULL OR p_PositionID = '' OR e.PositionID = p_PositionID)
      AND (p_SalaryFrom IS NULL OR e.Salary >= p_SalaryFrom)
      AND (p_SalaryTo IS NULL OR e.Salary <= p_SalaryTo)
      AND (p_Gender IS NULL OR e.Gender = p_Gender)
      AND (p_HireDateFrom IS NULL OR DATE(e.HireDate) >= p_HireDateFrom)
      AND (p_HireDateTo IS NULL OR DATE(e.HireDate) <= p_HireDateTo);
END $$

CREATE PROCEDURE Proc_Employee_GetByDepartmentId(
    IN p_DepartmentID VARCHAR(36)
)
BEGIN
    SELECT *
    FROM Employee
    WHERE DepartmentID = p_DepartmentID;
END $$

CREATE PROCEDURE Proc_Employee_GetByPositionId(
    IN p_PositionID VARCHAR(36)
)
BEGIN
    SELECT *
    FROM Employee
    WHERE PositionID = p_PositionID;
END $$

CREATE PROCEDURE Proc_Employee_GetByDepartmentCode(
    IN p_DepartmentCode VARCHAR(50)
)
BEGIN
    SELECT e.*
    FROM Employee e
    INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
    WHERE d.DepartmentCode = p_DepartmentCode;
END $$

CREATE PROCEDURE Proc_Employee_GetCountByDepartmentCode(
    IN p_DepartmentCode VARCHAR(50)
)
BEGIN
    SELECT COUNT(1)
    FROM Employee e
    INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
    WHERE d.DepartmentCode = p_DepartmentCode;
END $$

DELIMITER ;
