DROP PROCEDURE IF EXISTS `Proc_Employee_Filter`;
DELIMITER ;;
CREATE PROCEDURE `Proc_Employee_Filter`(
    IN v_DepartmentID  CHAR(36),
    IN v_PositionID    CHAR(36),
    IN v_SalaryFrom    DECIMAL(18, 4),
    IN v_SalaryTo      DECIMAL(18, 4),
    IN v_Gender        INT,
    IN v_HireDateFrom  DATE,
    IN v_HireDateTo    DATE,
    IN v_pageSize      INT,
    IN v_pageIndex     INT
)
BEGIN
  DECLARE v_offset INT;
  DECLARE v_where TEXT DEFAULT ' WHERE 1=1 ';

  IF v_pageIndex IS NULL OR v_pageIndex < 1 THEN
    SET v_pageIndex = 1;
  END IF;

  IF v_pageSize IS NULL OR v_pageSize < 1 THEN
    SET v_pageSize = 10;
  END IF;

  SET v_offset = (v_pageIndex - 1) * v_pageSize;
 
  IF v_DepartmentID IS NOT NULL AND v_DepartmentID <> '' THEN
    SET v_where = CONCAT(v_where, ' AND e.DepartmentID = ''', v_DepartmentID, '''');
  END IF;
 
  IF v_PositionID IS NOT NULL AND v_PositionID <> '' THEN
    SET v_where = CONCAT(v_where, ' AND e.PositionID = ''', v_PositionID, '''');
  END IF;
  
  IF v_SalaryFrom IS NOT NULL THEN
    SET v_where = CONCAT(v_where, ' AND e.Salary >= ', v_SalaryFrom);
  END IF;
 
  IF v_SalaryTo IS NOT NULL THEN
    SET v_where = CONCAT(v_where, ' AND e.Salary <= ', v_SalaryTo);
  END IF;

  IF v_Gender IS NOT NULL THEN
    SET v_where = CONCAT(v_where, ' AND e.Gender = ', v_Gender);
  END IF;
 
  IF v_HireDateFrom IS NOT NULL THEN
    SET v_where = CONCAT(v_where, ' AND DATE(e.HireDate) >= ''', v_HireDateFrom, '''');
  END IF;
 
  IF v_HireDateTo IS NOT NULL THEN
    SET v_where = CONCAT(v_where, ' AND DATE(e.HireDate) <= ''', v_HireDateTo, '''');
  END IF;
 
  SET @v_sql = CONCAT(
    'SELECT
       e.EmployeeID,
       e.EmployeeCode,
       e.EmployeeName,
       e.Gender,
       CASE e.Gender
         WHEN 0 THEN ''Nữ''
         WHEN 1 THEN ''Nam''
         WHEN 2 THEN ''Khác''
         ELSE ''Không xác định''
       END AS GenderName,
       e.DateOfBirth,
       e.PhoneNumber,
       e.Email,
       e.Address,
       e.DepartmentID,
       d.DepartmentCode,
       d.DepartmentName,
       e.PositionID,
       p.PositionCode,
       p.PositionName,
       e.Salary,
       e.HireDate,
       e.CreatedDate
     FROM employee e
     LEFT JOIN department d ON e.DepartmentID = d.DepartmentID
     LEFT JOIN position   p ON e.PositionID   = p.PositionID',
    v_where,
    ' ORDER BY e.CreatedDate DESC',
    ' LIMIT ', v_offset, ', ', v_pageSize
  );

  SET @v_sqlCount = CONCAT(
    'SELECT COUNT(*) AS Total
     FROM employee e
     LEFT JOIN department d ON e.DepartmentID = d.DepartmentID
     LEFT JOIN position   p ON e.PositionID   = p.PositionID',
    v_where
  );
 
  PREPARE stmt1 FROM @v_sql;
  EXECUTE stmt1;
  DEALLOCATE PREPARE stmt1;

  PREPARE stmt2 FROM @v_sqlCount;
  EXECUTE stmt2;
  DEALLOCATE PREPARE stmt2;
 
END
;;
DELIMITER ;
