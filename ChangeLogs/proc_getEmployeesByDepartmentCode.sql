DROP PROCEDURE IF EXISTS `Proc_GetEmployeesByDepartmentCode`;
 
DELIMITER ;;
 
CREATE PROCEDURE `Proc_GetEmployeesByDepartmentCode`(
  IN v_DepartmentCode VARCHAR(20)
)
BEGIN
  IF v_DepartmentCode IS NULL OR v_DepartmentCode = '' THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'DepartmentCode không được để trống';
  END IF;
 
  SELECT
    e.EmployeeID,
    e.EmployeeCode,
    e.EmployeeName,
    e.Gender,
    CASE e.Gender
      WHEN 0 THEN 'Nữ'
      WHEN 1 THEN 'Nam'
      WHEN 2 THEN 'Khác'
      ELSE 'Không xác định'
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
    e.CreatedDate
  FROM employee e
  INNER JOIN department d ON e.DepartmentID = d.DepartmentID
  LEFT JOIN  position   p ON e.PositionID   = p.PositionID
  WHERE d.DepartmentCode = CONVERT(v_DepartmentCode USING utf8mb4) COLLATE utf8mb4_general_ci
  ORDER BY e.EmployeeName ASC;
 
END
;;
 
DELIMITER ;