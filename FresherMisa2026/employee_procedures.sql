-- Employee procedures for FresherMisa2026
-- Compatible with BaseRepository dynamic parameters (@v_<PropertyName>)
-- Target DB from appsettings: misa_employee_development

USE misa_employee_development;

-- Ensure database-level uniqueness for employee code (race-condition safe)
SET @idx_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'UX_employee_EmployeeCode'
);
SET @idx_sql := IF(
    @idx_exists = 0,
    'ALTER TABLE employee ADD CONSTRAINT UX_employee_EmployeeCode UNIQUE (EmployeeCode)',
    'SELECT 1'
);
PREPARE idx_stmt FROM @idx_sql;
EXECUTE idx_stmt;
DEALLOCATE PREPARE idx_stmt;

DELIMITER $$

DROP PROCEDURE IF EXISTS Proc_InsertEmployee $$
CREATE PROCEDURE Proc_InsertEmployee(
    IN v_CreatedBy VARCHAR(100),
    IN v_CreateDate DATETIME,
    IN v_ModifiedBy VARCHAR(100),
    IN v_ModifiedDate DATETIME,
    IN v_State INT,
    IN v_IsDeleted BOOLEAN,
    IN v_EmployeeID CHAR(36),
    IN v_EmployeeCode VARCHAR(20),
    IN v_EmployeeName VARCHAR(100),
    IN v_Gender INT,
    IN v_DateOfBirth DATE,
    IN v_PhoneNumber VARCHAR(50),
    IN v_Email VARCHAR(100),
    IN v_Address VARCHAR(255),
    IN v_DepartmentID CHAR(36),
    IN v_PositionID CHAR(36),
    IN v_Salary DECIMAL(18,4),
    IN v_CreatedDate DATETIME
)
BEGIN
    INSERT INTO employee (
        EmployeeID,
        EmployeeCode,
        EmployeeName,
        Gender,
        DateOfBirth,
        PhoneNumber,
        Email,
        Address,
        DepartmentID,
        PositionID,
        Salary,
        CreatedDate
    )
    VALUES (
        v_EmployeeID,
        v_EmployeeCode,
        v_EmployeeName,
        v_Gender,
        v_DateOfBirth,
        v_PhoneNumber,
        v_Email,
        v_Address,
        v_DepartmentID,
        v_PositionID,
        v_Salary,
        v_CreatedDate
    );
END $$

DROP PROCEDURE IF EXISTS Proc_UpdateEmployee $$
CREATE PROCEDURE Proc_UpdateEmployee(
    IN v_CreatedBy VARCHAR(100),
    IN v_CreateDate DATETIME,
    IN v_ModifiedBy VARCHAR(100),
    IN v_ModifiedDate DATETIME,
    IN v_State INT,
    IN v_IsDeleted BOOLEAN,
    IN v_EmployeeID CHAR(36),
    IN v_EmployeeCode VARCHAR(20),
    IN v_EmployeeName VARCHAR(100),
    IN v_Gender INT,
    IN v_DateOfBirth DATE,
    IN v_PhoneNumber VARCHAR(50),
    IN v_Email VARCHAR(100),
    IN v_Address VARCHAR(255),
    IN v_DepartmentID CHAR(36),
    IN v_PositionID CHAR(36),
    IN v_Salary DECIMAL(18,4),
    IN v_CreatedDate DATETIME
)
BEGIN
    UPDATE employee
    SET EmployeeCode = v_EmployeeCode,
        EmployeeName = v_EmployeeName,
        Gender = v_Gender,
        DateOfBirth = v_DateOfBirth,
        PhoneNumber = v_PhoneNumber,
        Email = v_Email,
        Address = v_Address,
        DepartmentID = v_DepartmentID,
        PositionID = v_PositionID,
        Salary = v_Salary,
        CreatedDate = v_CreatedDate
    WHERE EmployeeID = v_EmployeeID;
END $$

DROP PROCEDURE IF EXISTS Proc_DeleteEmployeeById $$
CREATE PROCEDURE Proc_DeleteEmployeeById(
    IN v_EmployeeID CHAR(36)
)
BEGIN
    DELETE FROM employee
    WHERE EmployeeID = v_EmployeeID;
END $$

DELIMITER ;
