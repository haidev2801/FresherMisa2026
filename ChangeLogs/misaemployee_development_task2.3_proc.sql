DELIMITER $$

DROP PROCEDURE IF EXISTS Proc_Department_GetEmployeesByCode $$
CREATE PROCEDURE Proc_Department_GetEmployeesByCode(
    IN v_DepartmentCode VARCHAR(20)
)
BEGIN
    SELECT e.*
    FROM employee e
    INNER JOIN department d ON e.DepartmentID = d.DepartmentID
    WHERE d.DepartmentCode = v_DepartmentCode;
END $$

DROP PROCEDURE IF EXISTS Proc_Department_CountEmployeesByCode $$
CREATE PROCEDURE Proc_Department_CountEmployeesByCode(
    IN v_DepartmentCode VARCHAR(20)
)
BEGIN
    SELECT COUNT(*) AS Total
    FROM employee e
    INNER JOIN department d ON e.DepartmentID = d.DepartmentID
    WHERE d.DepartmentCode = v_DepartmentCode;
END $$

DELIMITER ;