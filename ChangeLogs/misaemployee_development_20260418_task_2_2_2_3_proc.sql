-- =============================================
-- Task 2.2 + Task 2.3 + Taso 3.3 Stored Procedures
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
        IN p_HireDateTo DATE,
        IN p_PageSize INT,
        IN p_PageIndex INT
)
BEGIN
        DECLARE v_PageSize INT DEFAULT 10;
        DECLARE v_PageIndex INT DEFAULT 1;
        DECLARE v_Offset INT DEFAULT 0;

        SET v_PageSize = IFNULL(NULLIF(p_PageSize, 0), 10);
        SET v_PageIndex = IFNULL(NULLIF(p_PageIndex, 0), 1);
        IF v_PageIndex < 1 THEN SET v_PageIndex = 1; END IF;
        IF v_PageSize < 1 THEN SET v_PageSize = 10; END IF;
        SET v_Offset = (v_PageIndex - 1) * v_PageSize;

        SELECT COUNT(1) AS Total
        FROM Employee e
        WHERE (p_DepartmentID IS NULL OR p_DepartmentID = '' OR e.DepartmentID = p_DepartmentID)
            AND (p_PositionID IS NULL OR p_PositionID = '' OR e.PositionID = p_PositionID)
            AND (p_SalaryFrom IS NULL OR e.Salary >= p_SalaryFrom)
            AND (p_SalaryTo IS NULL OR e.Salary <= p_SalaryTo)
            AND (p_Gender IS NULL OR e.Gender = p_Gender)
            AND (p_HireDateFrom IS NULL OR DATE(e.HireDate) >= p_HireDateFrom)
            AND (p_HireDateTo IS NULL OR DATE(e.HireDate) <= p_HireDateTo);

    SELECT *
    FROM Employee e
    WHERE (p_DepartmentID IS NULL OR p_DepartmentID = '' OR e.DepartmentID = p_DepartmentID)
      AND (p_PositionID IS NULL OR p_PositionID = '' OR e.PositionID = p_PositionID)
      AND (p_SalaryFrom IS NULL OR e.Salary >= p_SalaryFrom)
      AND (p_SalaryTo IS NULL OR e.Salary <= p_SalaryTo)
      AND (p_Gender IS NULL OR e.Gender = p_Gender)
      AND (p_HireDateFrom IS NULL OR DATE(e.HireDate) >= p_HireDateFrom)
            AND (p_HireDateTo IS NULL OR DATE(e.HireDate) <= p_HireDateTo)
        ORDER BY e.CreatedDate DESC
        LIMIT v_Offset, v_PageSize;
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

-- =============================================
-- Task 3.2: Tạo unique index cho EmployeeCode
-- Cơ sở dữ liệu: misaemployee_development
-- Mục đích: Ngăn race condition khi thêm nhân viên bị trùng mã
-- =============================================

-- Tạo unique index an toàn nếu chưa tồn tại
SET @index_exists := (
        SELECT COUNT(1)
        FROM information_schema.statistics
        WHERE table_schema = DATABASE()
            AND table_name = 'Employee'
            AND index_name = 'UX_Employee_EmployeeCode'
);

SET @sql := IF(
        @index_exists = 0,
        'ALTER TABLE Employee ADD UNIQUE INDEX UX_Employee_EmployeeCode (EmployeeCode)',
        'SELECT 1'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Check Index đã được tạo thành công
SELECT index_name, column_name, non_unique
FROM information_schema.statistics
WHERE table_schema = DATABASE()
    AND table_name = 'Employee'
    AND index_name = 'UX_Employee_EmployeeCode';
