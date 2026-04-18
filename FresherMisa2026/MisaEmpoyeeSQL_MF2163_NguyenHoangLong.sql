CREATE DATABASE misaemployee_development;
USE misaemployee_development;

CREATE TABLE `department` (
  `DepartmentID` char(36) PRIMARY KEY DEFAULT (UUID()),
  `DepartmentCode` varchar(20) NOT NULL,
  `DepartmentName` varchar(255) NOT NULL,
  `Description` varchar(255),
  UNIQUE KEY `UQ_DepartmentCode` (`DepartmentCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `position` (
  `PositionID` char(36) PRIMARY KEY DEFAULT (UUID()),
  `PositionCode` varchar(20) NOT NULL,
  `PositionName` varchar(255) NOT NULL,
  UNIQUE KEY `UQ_PositionCode` (`PositionCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `employee` (
  `EmployeeID` char(36) PRIMARY KEY DEFAULT (UUID()),
  `EmployeeCode` varchar(20) NOT NULL,
  `EmployeeName` varchar(100) NOT NULL,
  `Gender` TINYINT DEFAULT 0,
  `DateOfBirth` date,
  `PhoneNumber` varchar(50),
  `Email` varchar(100),
  `Address` varchar(255),
  `DepartmentID` char(36) NOT NULL,
  `PositionID` char(36) NOT NULL,
  `Salary` decimal(18,4) DEFAULT 0.0000,
  `HireDate` datetime,
  `CreatedDate` datetime,
  UNIQUE KEY `UQ_EmployeeCode` (`EmployeeCode`),

  CONSTRAINT `fk_employee_department`
    FOREIGN KEY (`DepartmentID`)
    REFERENCES `department`(`DepartmentID`),

  CONSTRAINT `fk_employee_position`
    FOREIGN KEY (`PositionID`)
    REFERENCES `position`(`PositionID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX idx_employee_department_id ON employee(DepartmentId);
CREATE INDEX idx_employee_position_id  ON employee(PositionId);
CREATE INDEX idx_employee_code  ON employee(EmployeeCode);
CREATE INDEX idx_department_code  ON department(DepartmentCode);
CREATE INDEX idx_position_code  ON `position`(PositionCode);
CREATE INDEX idx_employee_department_position ON employee(DepartmentID, PositionID);

EXPLAIN SELECT * FROM employee WHERE EmployeeCode = 'EMP001';

EXPLAIN SELECT * FROM department WHERE DepartmentCode = 'D002';

EXPLAIN SELECT * FROM position WHERE PositionCode = 'P001';

EXPLAIN SELECT e.EmployeeName, d.DepartmentName, p.PositionName

FROM employee e
INNER JOIN department d ON e.DepartmentID = d.DepartmentID
INNER JOIN position p ON e.PositionID = p.PositionID
WHERE d.DepartmentCode = 'D002';

SELECT * FROM employee WHERE DepartmentID = '2a7d9f41-3c5e-4b8a-9e12-6d7f8a9b0c21' AND PositionID = 'b2c3d4e5-f6a7-4b8c-9d01-1e2f3a4b5c6d';



INSERT INTO department (DepartmentID, DepartmentCode, DepartmentName, Description) VALUES
('8f1c2a9e-6d5b-4f3a-9c21-1a2b3c4d5e61','D001','Phòng Kỹ thuật','Phát triển sản phẩm'),
('2a7d9f41-3c5e-4b8a-9e12-6d7f8a9b0c21','D002','Phòng Kinh doanh','Bán hàng và chăm sóc khách hàng'),
('b3c4d5e6-7f81-4a92-b123-9e8d7c6b5a41','D003','Phòng Nhân sự','Quản lý nhân sự'),
('c9d8e7f6-1a23-4b45-8c67-2d3e4f5a6b71','D004','Phòng Marketing','Quảng bá sản phẩm'),
('5e4d3c2b-1a9f-4b87-9c65-7d8e9f0a1b32','D005','Phòng Tài chính','Quản lý tài chính');

INSERT INTO position (PositionID, PositionCode, PositionName) VALUES
('a1b2c3d4-e5f6-4a7b-8c91-0d1e2f3a4b5c','P001','Nhân viên'),
('b2c3d4e5-f6a7-4b8c-9d01-1e2f3a4b5c6d','P002','Trưởng nhóm'),
('c3d4e5f6-a7b8-4c9d-0e12-2f3a4b5c6d7e','P003','Quản lý'),
('d4e5f6a7-b8c9-4d0e-1f23-3a4b5c6d7e8f','P004','Giám đốc'),
('e5f6a7b8-c9d0-4e1f-2a34-4b5c6d7e8f9a','P005','Thực tập sinh');

INSERT INTO employee (
    EmployeeID, EmployeeCode, EmployeeName, Gender,
    DateOfBirth, PhoneNumber, Email, Address,
    DepartmentID, PositionID, Salary, HireDate, CreatedDate
) VALUES
('11111111-aaaa-4bbb-8ccc-111111111111','EMP001','Nguyễn Văn An',1,'1995-05-10','0901234567','an@company.com','Hà Nội','8f1c2a9e-6d5b-4f3a-9c21-1a2b3c4d5e61','a1b2c3d4-e5f6-4a7b-8c91-0d1e2f3a4b5c',15000000,NOW(), NOW()),
('22222222-bbbb-4ccc-9ddd-222222222222','EMP002','Trần Thị Bình',0,'1998-08-20','0912345678','binh@company.com','HCM','2a7d9f41-3c5e-4b8a-9e12-6d7f8a9b0c21','b2c3d4e5-f6a7-4b8c-9d01-1e2f3a4b5c6d',12000000,NOW(), NOW()),
('33333333-cccc-4ddd-8eee-333333333333','EMP003','Lê Văn Cường',1,'1993-03-15','0923456789','cuong@company.com','Đà Nẵng','8f1c2a9e-6d5b-4f3a-9c21-1a2b3c4d5e61','c3d4e5f6-a7b8-4c9d-0e12-2f3a4b5c6d7e',18000000,NOW(),NOW()),
('44444444-dddd-4eee-9fff-444444444444','EMP004','Phạm Thị Dung',0,'1997-11-25','0934567890','dung@company.com','Hải Phòng','b3c4d5e6-7f81-4a92-b123-9e8d7c6b5a41','a1b2c3d4-e5f6-4a7b-8c91-0d1e2f3a4b5c',13000000,NOW(),NOW()),
('55555555-eeee-4fff-8aaa-555555555555','EMP005','Hoàng Văn Em',1,'1990-07-30','0945678901','em@company.com','Cần Thơ','2a7d9f41-3c5e-4b8a-9e12-6d7f8a9b0c21','c3d4e5f6-a7b8-4c9d-0e12-2f3a4b5c6d7e',20000000,NOW(),NOW()),
('66666666-ffff-4aaa-9bbb-666666666666','EMP006','Đặng Thị Hoa',0,'1996-02-14','0956789012','hoa@company.com','Huế','8f1c2a9e-6d5b-4f3a-9c21-1a2b3c4d5e61','b2c3d4e5-f6a7-4b8c-9d01-1e2f3a4b5c6d',14000000,NOW(),NOW()),
('77777777-aaaa-4bbb-8ccc-777777777777','EMP007','Bùi Văn Khánh',1,'1992-09-09','0967890123','khanh@company.com','Quảng Ninh','c9d8e7f6-1a23-4b45-8c67-2d3e4f5a6b71','a1b2c3d4-e5f6-4a7b-8c91-0d1e2f3a4b5c',17000000,NOW(),NOW()),
('88888888-bbbb-4ccc-9ddd-888888888888','EMP008','Võ Thị Lan',0,'1999-12-01','0978901234','lan@company.com','Bình Dương','2a7d9f41-3c5e-4b8a-9e12-6d7f8a9b0c21','e5f6a7b8-c9d0-4e1f-2a34-4b5c6d7e8f9a',11000000,NOW(),NOW()),
('99999999-cccc-4ddd-8eee-999999999999','EMP009','Ngô Văn Minh',1,'1994-06-18','0989012345','minh@company.com','Nghệ An','5e4d3c2b-1a9f-4b87-9c65-7d8e9f0a1b32','c3d4e5f6-a7b8-4c9d-0e12-2f3a4b5c6d7e',16000000,NOW(),NOW()),
('aaaaaaaa-dddd-4eee-9fff-aaaaaaaaaaaa','EMP010','Phan Thị Ngọc',0,'1997-04-22','0990123456','ngoc@company.com','Thanh Hóa','b3c4d5e6-7f81-4a92-b123-9e8d7c6b5a41','b2c3d4e5-f6a7-4b8c-9d01-1e2f3a4b5c6d',12500000,NOW(),NOW());


DELIMITER ;;

CREATE PROCEDURE Proc_DeleteEmployeeById(
    IN v_EmployeeID CHAR(36)
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM employee WHERE EmployeeID = v_EmployeeID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Employee không tồn tại';
    ELSE
        DELETE FROM employee
        WHERE EmployeeID = v_EmployeeID;
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_InsertEmployee(
    IN v_EmployeeCode VARCHAR(20),
    IN v_EmployeeName VARCHAR(255),
    IN v_Gender INT,
    IN v_DateOfBirth DATE,
    IN v_PhoneNumber VARCHAR(50),
    IN v_Email VARCHAR(100),
    IN v_Address VARCHAR(255),
    IN v_Salary DECIMAL(18,4),
    IN v_DepartmentID CHAR(36),
    IN v_PositionID CHAR(36),
    IN v_HireDate DATE
)
BEGIN
    -- Check duplicate code
    IF EXISTS (
        SELECT 1 FROM employee WHERE EmployeeCode = v_EmployeeCode
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'EmployeeCode đã tồn tại';

    -- Check FK Department
    ELSEIF NOT EXISTS (
        SELECT 1 FROM department WHERE DepartmentID = v_DepartmentID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Department không tồn tại';

    -- Check FK Position
    ELSEIF NOT EXISTS (
        SELECT 1 FROM position WHERE PositionID = v_PositionID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Position không tồn tại';

    ELSE
        INSERT INTO employee (
            EmployeeCode, EmployeeName, Gender,
            DateOfBirth, PhoneNumber, Email, Address,
            Salary, DepartmentID, PositionID, HireDate, CreatedDate
        )
        VALUES (
            v_EmployeeCode, v_EmployeeName, v_Gender,
            v_DateOfBirth, v_PhoneNumber, v_Email, v_Address,
            v_Salary, v_DepartmentID, v_PositionID, HireDate, NOW()
        );
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_UpdateEmployee(
    IN v_EmployeeID CHAR(36),
    IN v_EmployeeCode VARCHAR(20),
    IN v_EmployeeName VARCHAR(255),
    IN v_Gender INT,
    IN v_DateOfBirth DATE,
    IN v_PhoneNumber VARCHAR(50),
    IN v_Email VARCHAR(100),
    IN v_Address VARCHAR(255),
    IN v_Salary DECIMAL(18,4),
    IN v_DepartmentID CHAR(36),
    IN v_PositionID CHAR(36),
    IN v_HireDate DATE
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM employee WHERE EmployeeID = v_EmployeeID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Employee không tồn tại';

    ELSEIF EXISTS (
        SELECT 1 FROM employee
        WHERE EmployeeCode = v_EmployeeCode
        AND EmployeeID <> v_EmployeeID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'EmployeeCode đã tồn tại';

    ELSEIF NOT EXISTS (
        SELECT 1 FROM department WHERE DepartmentID = v_DepartmentID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Department không tồn tại';

    ELSEIF NOT EXISTS (
SELECT 1 FROM position WHERE PositionID = v_PositionID
    ) THEN
        SIGNAL SQLSTATE '45000'
SET MESSAGE_TEXT = 'Position không tồn tại';

    ELSE
        UPDATE employee
        SET 
            EmployeeCode = v_EmployeeCode,
            EmployeeName = v_EmployeeName,
            Gender = v_Gender,
            DateOfBirth = v_DateOfBirth,
            PhoneNumber = v_PhoneNumber,
            Email = v_Email,
            Address = v_Address,
            Salary = v_Salary,
            DepartmentID = v_DepartmentID,
            PositionID = v_PositionID,
            HireDate = v_HireDate
        WHERE EmployeeID = v_EmployeeID;
    END IF;
END;;

DELIMITER ;


DELIMITER ;;

CREATE PROCEDURE Proc_InsertDepartment(
    IN v_DepartmentCode VARCHAR(20),
    IN v_DepartmentName VARCHAR(255),
    IN v_Description VARCHAR(255)
)
BEGIN
    IF EXISTS (
        SELECT 1 FROM department WHERE DepartmentCode = v_DepartmentCode
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'DepartmentCode đã tồn tại';
    ELSE
        INSERT INTO department (
            DepartmentCode, DepartmentName, Description
        )
        VALUES (
            v_DepartmentCode, v_DepartmentName, v_Description
        );
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_UpdateDepartment(
    IN v_DepartmentID CHAR(36),
    IN v_DepartmentCode VARCHAR(20),
    IN v_DepartmentName VARCHAR(255),
    IN v_Description VARCHAR(255)
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM department WHERE DepartmentID = v_DepartmentID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Department không tồn tại';

    ELSEIF EXISTS (
        SELECT 1 FROM department
        WHERE DepartmentCode = v_DepartmentCode
        AND DepartmentID <> v_DepartmentID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'DepartmentCode đã tồn tại';

    ELSE
        UPDATE department
        SET 
            DepartmentCode = v_DepartmentCode,
            DepartmentName = v_DepartmentName,
            Description = v_Description
        WHERE DepartmentID = v_DepartmentID;
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_DeleteDepartmentById(
    IN v_DepartmentID CHAR(36)
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM department WHERE DepartmentID = v_DepartmentID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Department không tồn tại';

    ELSEIF EXISTS (
        SELECT 1 FROM employee WHERE DepartmentID = v_DepartmentID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể xóa: Department đang được sử dụng';

    ELSE
        DELETE FROM department WHERE DepartmentID = v_DepartmentID;
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_InsertPosition(
    IN v_PositionCode VARCHAR(20),
    IN v_PositionName VARCHAR(255)
)
BEGIN
IF EXISTS (
        SELECT 1 FROM position 
        WHERE PositionCode = v_PositionCode
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'PositionCode đã tồn tại';
    ELSE
INSERT INTO position (
            PositionCode,
            PositionName
        )
        VALUES (
            v_PositionCode,
            v_PositionName
        );
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_UpdatePosition(
    IN v_PositionID CHAR(36),
    IN v_PositionCode VARCHAR(20),
    IN v_PositionName VARCHAR(255)
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM position 
        WHERE PositionID = v_PositionID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Position không tồn tại';

    ELSEIF EXISTS (
        SELECT 1 FROM position
        WHERE PositionCode = v_PositionCode
        AND PositionID <> v_PositionID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'PositionCode đã tồn tại';

    ELSE
        UPDATE position
        SET 
            PositionCode = v_PositionCode,
            PositionName = v_PositionName
        WHERE PositionID = v_PositionID;
    END IF;
END;;

DELIMITER ;

DELIMITER ;;

CREATE PROCEDURE Proc_DeletePositionById(
    IN v_PositionID CHAR(36)
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM position 
        WHERE PositionID = v_PositionID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Position không tồn tại';

    -- Check đang được dùng bởi employee
    ELSEIF EXISTS (
        SELECT 1 FROM employee 
        WHERE PositionID = v_PositionID
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Không thể xóa: Position đang được sử dụng';

    ELSE
        DELETE FROM position
        WHERE PositionID = v_PositionID;
    END IF;
END;;

DELIMITER ;
    
    
    
    
    DELIMITER $$

	CREATE PROCEDURE Proc_BaseFilterPaging(
		IN v_TableName VARCHAR(100),
		IN v_Search VARCHAR(255),
		IN v_SortBy VARCHAR(255),
		IN v_PageIndex INT,
		IN v_PageSize INT,
		IN v_SearchFields JSON
	)
	BEGIN
		SET @sql = CONCAT('SELECT * FROM ', v_TableName, ' WHERE 1=1');
		SET @countSql = CONCAT('SELECT COUNT(*) FROM ', v_TableName, ' WHERE 1=1');

		-- Xử lý điều kiện tìm kiếm
		IF v_Search IS NOT NULL AND v_Search <> '' THEN
			SET @searchCondition = '';
			SET @fields = JSON_LENGTH(v_SearchFields);
			SET @i = 0;
			WHILE @i < @fields DO
				SET @field = JSON_UNQUOTE(JSON_EXTRACT(v_SearchFields, CONCAT('$[', @i, ']')));
				SET @searchCondition = CONCAT(@searchCondition, ' OR ', @field, ' LIKE ''%', v_Search, '%''');
				SET @i = @i + 1;
			END WHILE;
			SET @searchCondition = SUBSTRING(@searchCondition, 5); -- bỏ ' OR ' đầu
			SET @sql = CONCAT(@sql, ' AND (', @searchCondition, ')');
			SET @countSql = CONCAT(@countSql, ' AND (', @searchCondition, ')');
		END IF;

		-- Xử lý nhiều sortBy
		IF v_SortBy IS NOT NULL AND v_SortBy <> '' THEN
			SET @orderBy = '';
			SET @sorts = v_SortBy;
			WHILE LENGTH(@sorts) > 0 DO
				SET @comma = LOCATE(',', @sorts);
				IF @comma > 0 THEN
					SET @sort = LEFT(@sorts, @comma - 1);
SET @sorts = SUBSTRING(@sorts, @comma + 1);
				ELSE
					SET @sort = @sorts;
					SET @sorts = '';
				END IF;

				IF LEFT(@sort, 1) = '+' THEN
					SET @orderBy = CONCAT(@orderBy, ', ', SUBSTRING(@sort, 2), ' ASC');
				ELSEIF LEFT(@sort, 1) = '-' THEN
					SET @orderBy = CONCAT(@orderBy, ', ', SUBSTRING(@sort, 2), ' DESC');
				ELSE
					SET @orderBy = CONCAT(@orderBy, ', ', @sort, ' ASC');
				END IF;
			END WHILE;
			SET @orderBy = SUBSTRING(@orderBy, 3); -- bỏ dấu ', ' đầu
			SET @sql = CONCAT(@sql, ' ORDER BY ', @orderBy);
		END IF;

		SET @sql = CONCAT(@sql, ' LIMIT ', v_PageSize, ' OFFSET ', v_PageIndex * v_PageSize);
		

		
		PREPARE stmt FROM @sql;
		EXECUTE stmt;
		DEALLOCATE PREPARE stmt;

		PREPARE countStmt FROM @countSql;
		EXECUTE countStmt;
		DEALLOCATE PREPARE countStmt;
	END$$

	DELIMITER ;


DELIMITER $$

CREATE PROCEDURE Proc_FilterEmployee(
    IN v_DepartmentID CHAR(36),
    IN v_PositionID CHAR(36),
    IN v_SalaryFrom DECIMAL(18,4),
    IN v_SalaryTo DECIMAL(18,4),
    IN v_Gender TINYINT,
    IN v_HireDateFrom DATETIME,
    IN v_HireDateTo DATETIME,
    IN v_PageSize INT,
    IN v_PageIndex INT
)
BEGIN
    -- Khai báo biến cục bộ để tính toán offset
    DECLARE v_Offset INT;
    SET v_Offset = (v_PageIndex - 1) * v_PageSize;

    -- Chuẩn bị câu lệnh SQL dưới dạng chuỗi
    SET @sql = 'SELECT 
                    EmployeeID, EmployeeCode, EmployeeName, Gender, 
                    DateOfBirth, PhoneNumber, Email, Address, 
                    DepartmentID, PositionID, Salary, HireDate, CreatedDate
                FROM employee
                WHERE (1=1)';
                
	SET @count = 'SELECT COUNT(*) FROM employee WHERE (1=1)';

    -- Cộng dồn các điều kiện lọc (tránh lỗi NULL so sánh)
    IF v_DepartmentID IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND DepartmentID = "', v_DepartmentID, '"'); END IF;
    IF v_PositionID IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND PositionID = "', v_PositionID, '"'); END IF;
    IF v_Gender IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND Gender = ', v_Gender); END IF;
    IF v_SalaryFrom IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND Salary >= ', v_SalaryFrom); END IF;
    IF v_SalaryTo IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND Salary <= ', v_SalaryTo); END IF;
    IF v_HireDateFrom IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND HireDate >= "', v_HireDateFrom, '"'); END IF;
    IF v_HireDateTo IS NOT NULL THEN SET @sql = CONCAT(@sql, ' AND HireDate <= "', v_HireDateTo, '"'); END IF;
    
    -- Thêm LIMIT và OFFSET vào chuỗi SQL
    SET @sql = CONCAT(@sql, ' LIMIT ', v_PageSize, ' OFFSET ', v_Offset);
    
     -- Cộng dồn các điều kiện lọc (tránh lỗi NULL so sánh)
    IF v_DepartmentID IS NOT NULL THEN SET @count = CONCAT(@count, ' AND DepartmentID = "', v_DepartmentID, '"'); END IF;
    IF v_PositionID IS NOT NULL THEN SET @count = CONCAT(@count, ' AND PositionID = "', v_PositionID, '"'); END IF;
IF v_Gender IS NOT NULL THEN SET @count = CONCAT(@count, ' AND Gender = ', v_Gender); END IF;
    IF v_SalaryFrom IS NOT NULL THEN SET @count = CONCAT(@count, ' AND Salary >= ', v_SalaryFrom); END IF;
    IF v_SalaryTo IS NOT NULL THEN SET @count = CONCAT(@count, ' AND Salary <= ', v_SalaryTo); END IF;
    IF v_HireDateFrom IS NOT NULL THEN SET @count = CONCAT(@count, ' AND HireDate >= "', v_HireDateFrom, '"'); END IF;
    IF v_HireDateTo IS NOT NULL THEN SET @count = CONCAT(@count, ' AND HireDate <= "', v_HireDateTo, '"'); END IF;

    -- Thực thi câu lệnh
    PREPARE stmt FROM @sql;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;
    
    PREPARE countStmt FROM @count;
	EXECUTE countStmt;
	DEALLOCATE PREPARE countStmt;
END $$

DELIMITER ;