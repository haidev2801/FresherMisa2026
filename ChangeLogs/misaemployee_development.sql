/*
 Navicat Premium Dump SQL

 Source Server         : localhost
 Source Server Type    : MySQL
 Source Server Version : 80406 (8.4.6)
 Source Host           : localhost:3306
 Source Schema         : misaemployee_development

 Target Server Type    : MySQL
 Target Server Version : 80406 (8.4.6)
 File Encoding         : 65001

 Date: 18/04/2026 15:16:05
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for department
-- ----------------------------
DROP TABLE IF EXISTS `department`;
CREATE TABLE `department`  (
  `DepartmentID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Khóa chính phòng ban',
  `DepartmentCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Mã phòng ban',
  `DepartmentName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Tên phòng ban',
  `Description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'Diễn giải',
  PRIMARY KEY (`DepartmentID`) USING BTREE,
  UNIQUE INDEX `UQ_DepartmentCode`(`DepartmentCode` ASC) USING BTREE
) ENGINE = InnoDB AVG_ROW_LENGTH = 2048 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of department
-- ----------------------------
INSERT INTO `department` VALUES ('32a44a52-e4e4-470a-9db6-dc2cc42d8296', 'string', 'string', 'string');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440010', 'RND', 'Research & Development', 'Nghiên cứu và phát triển sản phẩm');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440011', 'CS', 'Customer Service', 'Chăm sóc khách hàng');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440012', 'SALE', 'Sales', 'Kinh doanh và bán hàng');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440013', 'ADMIN', 'Administration', 'Hành chính tổng hợp');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440014', 'LEGAL', 'Legal', 'Pháp chế doanh nghiệp');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440015', 'DATA', 'Data Engineering', 'Quản lý và phân tích dữ liệu');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440016', 'SEC', 'Security', 'An ninh hệ thống');
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440017', 'SUP', 'Support', 'Hỗ trợ kỹ thuật');

-- ----------------------------
-- Table structure for employee
-- ----------------------------
DROP TABLE IF EXISTS `employee`;
CREATE TABLE `employee`  (
  `EmployeeID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Khóa chính nhân viên',
  `EmployeeCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Mã nhân viên',
  `EmployeeName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Tên nhân viên',
  `Gender` int NULL DEFAULT 0 COMMENT 'Giới tính: 0-Nữ, 1-Nam, 2-Khác',
  `DateOfBirth` date NULL DEFAULT NULL COMMENT 'Ngày sinh',
  `PhoneNumber` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'Số điện thoại',
  `Email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'Email',
  `Address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'Địa chỉ',
  `DepartmentID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Phòng ban',
  `PositionID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Chức vụ',
  `Salary` decimal(18, 4) NULL DEFAULT 0.0000 COMMENT 'Lương cơ bản',
  `HireDate` datetime NULL DEFAULT NULL COMMENT 'Ngày vào làm',
  `CreatedDate` datetime NULL DEFAULT NULL COMMENT 'Ngày tạo',
  UNIQUE INDEX `UQ_EmployeeCode`(`EmployeeCode` ASC) USING BTREE
) ENGINE = InnoDB AVG_ROW_LENGTH = 1092 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of employee
-- ----------------------------
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000001', 'EMP001', 'Nguyễn Văn An', 1, '1995-03-15', '0912345678', 'an.nguyen@misa.com', 'Hà Nội', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 18000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000002', 'EMP002', 'Trần Thị Mai', 0, '1998-07-20', '0987654321', 'mai.tran@misa.com', 'Hồ Chí Minh', '550e8400-e29b-41d4-a716-446655440010', '22222222-2222-2222-2222-222222222222', 15000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000003', 'EMP003', 'Phạm Minh Hoàng', 1, '1993-11-05', '0905123456', 'hoang.pham@misa.com', 'Đà Nẵng', '550e8400-e29b-41d4-a716-446655440010', '33333333-3333-3333-3333-333333333333', 35000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000004', 'EMP004', 'Lê Thị Hương', 0, '1996-02-10', '0934567890', 'huong.le@misa.com', 'Hà Nội', '550e8400-e29b-41d4-a716-446655440013', '44444444-4444-4444-4444-444444444444', 14000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000005', 'EMP005', 'Nguyễn Quốc Bảo', 1, '1992-09-18', '0911223344', 'bao.nguyen@misa.com', 'Hà Nội', '550e8400-e29b-41d4-a716-446655440013', '44444444-4444-4444-4444-444444444444', 16000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000006', 'EMP006', 'Đặng Thị Lan', 0, '1994-04-22', '0977112233', 'lan.dang@misa.com', 'HCM', '550e8400-e29b-41d4-a716-446655440011', '55555555-5555-5555-5555-555555555555', 20000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000007', 'EMP007', 'Hoàng Văn Sơn', 1, '1990-01-30', '0944556677', 'son.hoang@misa.com', 'HCM', '550e8400-e29b-41d4-a716-446655440011', '55555555-5555-5555-5555-555555555555', 22000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000008', 'EMP008', 'Vũ Thị Thu', 0, '1997-06-12', '0966889900', 'thu.vu@misa.com', 'Hà Nội', '550e8400-e29b-41d4-a716-446655440012', '66666666-6666-6666-6666-666666666666', 17000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000009', 'EMP009', 'Phan Anh Tú', 1, '1999-12-01', '0922113344', 'tu.phan@misa.com', 'HCM', '550e8400-e29b-41d4-a716-446655440012', '66666666-6666-6666-6666-666666666666', 16500000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000012', 'EMP012', 'Nguyễn Trung Kiên', 1, '1998-05-05', '0912000001', 'kien.nguyen@misa.com', 'Hà Nội', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 17500000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000013', 'EMP013', 'Trịnh Hà My', 0, '1997-09-09', '0912000002', 'my.trinh@misa.com', 'HCM', '550e8400-e29b-41d4-a716-446655440010', '22222222-2222-2222-2222-222222222222', 15500000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000014', 'EMP014', 'Đỗ Minh Tuấn', 1, '1994-07-07', '0912000003', 'tuan.do@misa.com', 'HCM', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 19000000.0000, '2026-04-15 09:29:18', '2026-04-15 09:29:18');
INSERT INTO `employee` VALUES ('e67efcdf-2bf6-41d3-a7d3-fa823f2181c2', 'EMP655882', 'Employee 655882', 1, '1998-01-01', '0901234567', 'employee655882@test.com', 'Ha Noi', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 12000000.0000, '2026-04-16 09:00:00', '2026-04-16 09:00:00');
INSERT INTO `employee` VALUES ('94aac558-0d77-4ba5-a481-d5dde96e1882', 'EMP713713', 'Employee 713713', 1, '1998-01-01', '0901234567', 'employee713713@test.com', 'Ha Noi', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 12000000.0000, '2026-04-16 09:00:00', '2026-04-16 09:00:00');
INSERT INTO `employee` VALUES ('37fd89ba-64a8-4055-8bb4-ed5202f7510d', 'EMP_RACE001', 'Race Test Nhân Viên 1', 1, '1996-08-10', '0911000001', 'race1@misa.com', 'Hà Nội', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 16000000.0000, '2025-01-01 00:00:00', NULL);
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000015', 'UPD713713', 'Updated Employee 713713', 0, '1997-05-05', '0911002200', 'updated@test.com', 'HCM', '550e8400-e29b-41d4-a716-446655440010', '11111111-1111-1111-1111-111111111111', 15000000.0000, '2026-04-16 09:00:00', '2026-04-16 09:00:00');

-- ----------------------------
-- Table structure for position
-- ----------------------------
DROP TABLE IF EXISTS `position`;
CREATE TABLE `position`  (
  `PositionID` char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Khóa chính chức vụ',
  `PositionCode` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Mã chức vụ',
  `PositionName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Tên chức vụ',
  PRIMARY KEY (`PositionID`) USING BTREE,
  UNIQUE INDEX `UQ_PositionCode`(`PositionCode` ASC) USING BTREE
) ENGINE = InnoDB AVG_ROW_LENGTH = 2340 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of position
-- ----------------------------
INSERT INTO `position` VALUES ('00000000-0000-0000-0000-000000000001', 'DEV1', 'Senior Developer');
INSERT INTO `position` VALUES ('0e57e405-ce03-47a4-bc0c-2b536b80ab2c', 'POS278452', 'Position 278452');
INSERT INTO `position` VALUES ('11111111-1111-1111-1111-111111111111', 'DevWeb', 'Developer web');
INSERT INTO `position` VALUES ('22222222-2222-2222-2222-222222222222', 'QA', 'Quality Assurance');
INSERT INTO `position` VALUES ('33333333-3333-3333-3333-333333333333', 'PM', 'Project Manager');
INSERT INTO `position` VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afa6', 'string', 'string');
INSERT INTO `position` VALUES ('44444444-4444-4444-4444-444444444444', 'HR', 'HR Specialist');
INSERT INTO `position` VALUES ('55555555-5555-5555-5555-555555555555', 'ACC', 'Accountant');
INSERT INTO `position` VALUES ('66666666-6666-6666-6666-666666666666', 'MKT', 'Marketing Executive');
INSERT INTO `position` VALUES ('77777777-7777-7777-7777-777777777777', 'OPS', 'Operation Staff');

-- ----------------------------
-- Procedure structure for Proc_DeleteDepartmentById
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_DeleteDepartmentById`;
delimiter ;;
CREATE PROCEDURE `Proc_DeleteDepartmentById`(IN v_DepartmentID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  DELETE FROM Department WHERE DepartmentID = v_DepartmentID;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_DeleteEmployeeById
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_DeleteEmployeeById`;
delimiter ;;
CREATE PROCEDURE `Proc_DeleteEmployeeById`(IN v_EmployeeID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  DELETE FROM Employee WHERE EmployeeID = v_EmployeeID;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_DeletePositionById
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_DeletePositionById`;
delimiter ;;
CREATE PROCEDURE `Proc_DeletePositionById`(IN v_PositionID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  DELETE FROM Position WHERE PositionID = v_PositionID;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_Department_FilterPaging
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_Department_FilterPaging`;
delimiter ;;
CREATE PROCEDURE `Proc_Department_FilterPaging`(IN v_pageIndex int,
IN v_pageSize int,
IN v_search varchar(255),
IN v_sort varchar(200),
IN v_searchFields json)
BEGIN

  DECLARE v_offset int;
  DECLARE v_where text DEFAULT ' WHERE 1=1 ';
  DECLARE v_orderBy text DEFAULT '';
  DECLARE v_searchCondition text;

  -- 1️⃣ Paging cơ bản
  IF v_pageIndex < 1 THEN
    SET v_pageIndex = 1;
  END IF;
  IF v_pageSize < 1 THEN
    SET v_pageSize = 20;
  END IF;

  SET v_offset = (v_pageIndex - 1) * v_pageSize;

  -- 2️⃣ Search nhiều field
  IF v_search IS NOT NULL
    AND v_search <> ''
    AND v_searchFields IS NOT NULL THEN

    SELECT
      GROUP_CONCAT(
      CONCAT(
      '`',
      JSON_UNQUOTE(JSON_EXTRACT(v_searchFields, CONCAT('$[', n, ']'))),
      '` LIKE "%', v_search, '%"'
      )
      SEPARATOR ' OR '
      ) INTO v_searchCondition
    FROM (SELECT
        0 n
      UNION
      SELECT
        1
      UNION
      SELECT
        2
      UNION
      SELECT
        3
      UNION
      SELECT
        4) t
    WHERE n < JSON_LENGTH(v_searchFields);

    IF v_searchCondition IS NOT NULL THEN
      SET v_where = CONCAT(v_where, ' AND (', v_searchCondition, ')');
    END IF;

  END IF;

  -- 3️⃣ Sort nhiều cột
  IF v_sort IS NOT NULL
    AND v_sort <> '' THEN

    SELECT
      GROUP_CONCAT(
      CONCAT(
      '`', SUBSTRING(item, 2), '` ',
      IF(LEFT(item, 1) = '-', 'DESC', 'ASC')
      )
      SEPARATOR ', '
      ) INTO v_orderBy
    FROM (SELECT
        TRIM(SUBSTRING_INDEX (SUBSTRING_INDEX (v_sort, ',', n), ',', -1)) item
      FROM (SELECT
          1 n
        UNION
        SELECT
          2
        UNION
        SELECT
          3
        UNION
        SELECT
          4) x
      WHERE n <= 1 + LENGTH(v_sort) - LENGTH(REPLACE(v_sort, ',', ''))) y;

    IF v_orderBy IS NOT NULL THEN
      SET v_orderBy = CONCAT(' ORDER BY ', v_orderBy);
    END IF;

  END IF;

  -- 4️⃣ Default sort
  IF v_orderBy IS NULL
    OR v_orderBy = '' THEN
    SET v_orderBy = ' ORDER BY DepartmentID DESC';
  END IF;

  -- 5️⃣ Build SQL
  SET @v_sql = CONCAT(
  'SELECT * FROM Department ',
  v_where,
  v_orderBy,
  ' LIMIT ', v_offset, ',', v_pageSize, '; '
  );


  -- 6. SQL count
  SET @v_sqlCount = CONCAT(
  'SELECT COUNT(*) AS Total FROM Department ',
  v_where
  );



  PREPARE stmt1 FROM @v_sql;
  EXECUTE stmt1;
  DEALLOCATE PREPARE stmt1;

  PREPARE stmt FROM @v_sqlCount;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;


END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_Employee_Filter
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_Employee_Filter`;
delimiter ;;
CREATE PROCEDURE `Proc_Employee_Filter`(IN v_DepartmentID char(36),
  IN v_PositionID char(36),
  IN v_SalaryFrom decimal(18,4),
  IN v_SalaryTo decimal(18,4),
  IN v_Gender int,
  IN v_HireDateFrom datetime,
  IN v_HireDateTo datetime)
BEGIN
  SELECT *
  FROM employee
  WHERE 1 = 1
    AND (v_DepartmentID IS NULL OR DepartmentID = v_DepartmentID)
    AND (v_PositionID IS NULL OR PositionID = v_PositionID)
    AND (v_SalaryFrom IS NULL OR Salary >= v_SalaryFrom)
    AND (v_SalaryTo IS NULL OR Salary <= v_SalaryTo)
    AND (v_Gender IS NULL OR Gender = v_Gender)
    AND (v_HireDateFrom IS NULL OR HireDate >= v_HireDateFrom)
    AND (v_HireDateTo IS NULL OR HireDate <= v_HireDateTo)
  ORDER BY EmployeeID DESC;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_Employee_FilterPaging
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_Employee_FilterPaging`;
delimiter ;;
CREATE PROCEDURE `Proc_Employee_FilterPaging`(IN v_pageIndex int,
IN v_pageSize int,
IN v_search varchar(255),
IN v_sort varchar(200),
IN v_searchFields json)
BEGIN
  DECLARE v_offset int;
  DECLARE v_where text DEFAULT ' WHERE 1=1 ';
  DECLARE v_orderBy text DEFAULT '';
  DECLARE v_searchCondition text;

  -- 1. Paging
  IF v_pageIndex < 1 THEN
    SET v_pageIndex = 1;
  END IF;
  IF v_pageSize < 1 THEN
    SET v_pageSize = 20;
  END IF;

  SET v_offset = (v_pageIndex - 1) * v_pageSize;

  -- 2. Search multiple fields
  IF v_search IS NOT NULL
    AND v_search <> ''
    AND v_searchFields IS NOT NULL THEN

    SELECT
      GROUP_CONCAT(
      CONCAT(
      '`',
      JSON_UNQUOTE(JSON_EXTRACT(v_searchFields, CONCAT('$[', n, ']'))),
      '` LIKE "%', v_search, '%"'
      )
      SEPARATOR ' OR '
      ) INTO v_searchCondition
    FROM (SELECT
        0 n
      UNION
      SELECT
        1
      UNION
      SELECT
        2
      UNION
      SELECT
        3
      UNION
      SELECT
        4
      UNION
      SELECT
        5
      UNION
      SELECT
        6
      UNION
      SELECT
        7
      UNION
      SELECT
        8
      UNION
      SELECT
        9) t
    WHERE n < JSON_LENGTH(v_searchFields);

    IF v_searchCondition IS NOT NULL THEN
      SET v_where = CONCAT(v_where, ' AND (', v_searchCondition, ')');
    END IF;
  END IF;

  -- 3. Sort multiple columns
  IF v_sort IS NOT NULL
    AND v_sort <> '' THEN
    SELECT
      GROUP_CONCAT(
      CONCAT(
      '`', SUBSTRING(item, 2), '` ',
      IF(LEFT(item, 1) = '-', 'DESC', 'ASC')
      )
      SEPARATOR ', '
      ) INTO v_orderBy
    FROM (SELECT
        TRIM(SUBSTRING_INDEX (SUBSTRING_INDEX (v_sort, ',', n), ',', -1)) item
      FROM (SELECT
          1 n
        UNION
        SELECT
          2
        UNION
        SELECT
          3
        UNION
        SELECT
          4
        UNION
        SELECT
          5) x
      WHERE n <= 1 + LENGTH(v_sort) - LENGTH(REPLACE(v_sort, ',', ''))) y;

    IF v_orderBy IS NOT NULL THEN
      SET v_orderBy = CONCAT(' ORDER BY ', v_orderBy);
    END IF;
  END IF;

  -- 4. Default sort
  IF v_orderBy IS NULL
    OR v_orderBy = '' THEN
    SET v_orderBy = ' ORDER BY EmployeeID DESC';
  END IF;

  -- 5. Build SQL
  SET @v_sql = CONCAT(
  'SELECT * FROM Employee ',
  v_where,
  v_orderBy,
  ' LIMIT ', v_offset, ',', v_pageSize, ';'
  );

  -- 6. SQL count
  SET @v_sqlCount = CONCAT(
  'SELECT COUNT(*) AS Total FROM Employee ',
  v_where
  );

  PREPARE stmt1 FROM @v_sql;
  EXECUTE stmt1;
  DEALLOCATE PREPARE stmt1;

  PREPARE stmt FROM @v_sqlCount;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_InsertDepartment
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_InsertDepartment`;
delimiter ;;
CREATE PROCEDURE `Proc_InsertDepartment`(IN v_DepartmentID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_DepartmentCode varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_DepartmentName varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_Description varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  IF EXISTS (SELECT 1 FROM department WHERE DepartmentCode = v_DepartmentCode) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'DepartmentCode đã tồn tại';
  ELSE
    INSERT INTO department (DepartmentID, DepartmentCode, DepartmentName, Description)
    VALUES (v_DepartmentID, v_DepartmentCode, v_DepartmentName, v_Description);
  END IF;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_InsertEmployee
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_InsertEmployee`;
delimiter ;;
CREATE PROCEDURE `Proc_InsertEmployee`(IN v_EmployeeID char(36),
  IN v_EmployeeCode varchar(20),
  IN v_EmployeeName varchar(100),
  IN v_Gender int,
  IN v_DateOfBirth date,
  IN v_PhoneNumber varchar(50),
  IN v_Email varchar(100),
  IN v_Address varchar(255),
  IN v_DepartmentID char(36),
  IN v_PositionID char(36),
  IN v_Salary decimal(18,4),
  IN v_HireDate datetime,
  IN v_CreatedDate datetime)
BEGIN
  IF EXISTS (SELECT 1 FROM employee WHERE EmployeeCode = v_EmployeeCode) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'EmployeeCode đã tồn tại';
  ELSE
    INSERT INTO employee (
      EmployeeID, EmployeeCode, EmployeeName, Gender, DateOfBirth,
      PhoneNumber, Email, Address, DepartmentID, PositionID, Salary, HireDate, CreatedDate
    ) VALUES (
      v_EmployeeID, v_EmployeeCode, v_EmployeeName, v_Gender, v_DateOfBirth,
      v_PhoneNumber, v_Email, v_Address, v_DepartmentID, v_PositionID, v_Salary, v_HireDate, v_CreatedDate
    );
  END IF;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_InsertPosition
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_InsertPosition`;
delimiter ;;
CREATE PROCEDURE `Proc_InsertPosition`(IN v_PositionID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_PositionCode varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_PositionName varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  IF EXISTS (SELECT 1 FROM position WHERE PositionCode = v_PositionCode) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'PositionCode đã tồn tại';
  ELSE
    INSERT INTO position (PositionID, PositionCode, PositionName)
    VALUES (v_PositionID, v_PositionCode, v_PositionName);
  END IF;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_Position_FilterPaging
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_Position_FilterPaging`;
delimiter ;;
CREATE PROCEDURE `Proc_Position_FilterPaging`(IN v_pageIndex int,
IN v_pageSize int,
IN v_search varchar(255),
IN v_sort varchar(200),
IN v_searchFields json)
BEGIN
  DECLARE v_offset int;
  DECLARE v_where text DEFAULT ' WHERE 1=1 ';
  DECLARE v_orderBy text DEFAULT '';
  DECLARE v_searchCondition text;

  -- 1. Paging
  IF v_pageIndex < 1 THEN
    SET v_pageIndex = 1;
  END IF;
  IF v_pageSize < 1 THEN
    SET v_pageSize = 20;
  END IF;

  SET v_offset = (v_pageIndex - 1) * v_pageSize;

  -- 2. Search multiple fields
  IF v_search IS NOT NULL
    AND v_search <> ''
    AND v_searchFields IS NOT NULL THEN

    SELECT
      GROUP_CONCAT(
      CONCAT(
      '`',
      JSON_UNQUOTE(JSON_EXTRACT(v_searchFields, CONCAT('$[', n, ']'))),
      '` LIKE "%', v_search, '%"'
      )
      SEPARATOR ' OR '
      ) INTO v_searchCondition
    FROM (SELECT
        0 n
      UNION
      SELECT
        1
      UNION
      SELECT
        2
      UNION
      SELECT
        3
      UNION
      SELECT
        4) t
    WHERE n < JSON_LENGTH(v_searchFields);

    IF v_searchCondition IS NOT NULL THEN
      SET v_where = CONCAT(v_where, ' AND (', v_searchCondition, ')');
    END IF;
  END IF;

  -- 3. Sort multiple columns
  IF v_sort IS NOT NULL
    AND v_sort <> '' THEN
    SELECT
      GROUP_CONCAT(
      CONCAT(
      '`', SUBSTRING(item, 2), '` ',
      IF(LEFT(item, 1) = '-', 'DESC', 'ASC')
      )
      SEPARATOR ', '
      ) INTO v_orderBy
    FROM (SELECT
        TRIM(SUBSTRING_INDEX (SUBSTRING_INDEX (v_sort, ',', n), ',', -1)) item
      FROM (SELECT
          1 n
        UNION
        SELECT
          2
        UNION
        SELECT
          3
        UNION
        SELECT
          4) x
      WHERE n <= 1 + LENGTH(v_sort) - LENGTH(REPLACE(v_sort, ',', ''))) y;

    IF v_orderBy IS NOT NULL THEN
      SET v_orderBy = CONCAT(' ORDER BY ', v_orderBy);
    END IF;
  END IF;

  -- 4. Default sort
  IF v_orderBy IS NULL
    OR v_orderBy = '' THEN
    SET v_orderBy = ' ORDER BY PositionID DESC';
  END IF;

  -- 5. Build SQL
  SET @v_sql = CONCAT(
  'SELECT * FROM Position ',
  v_where,
  v_orderBy,
  ' LIMIT ', v_offset, ',', v_pageSize, ';'
  );

  -- 6. SQL count
  SET @v_sqlCount = CONCAT(
  'SELECT COUNT(*) AS Total FROM Position ',
  v_where
  );

  PREPARE stmt1 FROM @v_sql;
  EXECUTE stmt1;
  DEALLOCATE PREPARE stmt1;

  PREPARE stmt FROM @v_sqlCount;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_UpdateDepartment
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_UpdateDepartment`;
delimiter ;;
CREATE PROCEDURE `Proc_UpdateDepartment`(IN v_DepartmentID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_DepartmentCode varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_DepartmentName varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_Description varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  IF NOT EXISTS (SELECT 1 FROM department WHERE DepartmentID = v_DepartmentID) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'Department không tồn tại';
  END IF;

  IF EXISTS (SELECT 1 FROM department WHERE DepartmentCode = v_DepartmentCode AND DepartmentID <> v_DepartmentID) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'DepartmentCode đã tồn tại';
  END IF;

  UPDATE department
  SET DepartmentCode = v_DepartmentCode,
      DepartmentName = v_DepartmentName,
      Description = v_Description
  WHERE DepartmentID = v_DepartmentID;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_UpdateEmployee
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_UpdateEmployee`;
delimiter ;;
CREATE PROCEDURE `Proc_UpdateEmployee`(IN v_EmployeeID char(36),
  IN v_EmployeeCode varchar(20),
  IN v_EmployeeName varchar(100),
  IN v_Gender int,
  IN v_DateOfBirth date,
  IN v_PhoneNumber varchar(50),
  IN v_Email varchar(100),
  IN v_Address varchar(255),
  IN v_DepartmentID char(36),
  IN v_PositionID char(36),
  IN v_Salary decimal(18,4),
  IN v_HireDate datetime,
  IN v_CreatedDate datetime)
BEGIN
  IF NOT EXISTS (SELECT 1 FROM employee WHERE EmployeeID = v_EmployeeID) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'Employee không tồn tại';
  END IF;

  IF EXISTS (SELECT 1 FROM employee WHERE EmployeeCode = v_EmployeeCode AND EmployeeID <> v_EmployeeID) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'EmployeeCode đã tồn tại';
  END IF;

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
      HireDate = v_HireDate,
      CreatedDate = v_CreatedDate
  WHERE EmployeeID = v_EmployeeID;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for Proc_UpdatePosition
-- ----------------------------
DROP PROCEDURE IF EXISTS `Proc_UpdatePosition`;
delimiter ;;
CREATE PROCEDURE `Proc_UpdatePosition`(IN v_PositionID char(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_PositionCode varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  IN v_PositionName varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci)
BEGIN
  IF NOT EXISTS (SELECT 1 FROM position WHERE PositionID = v_PositionID) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'Position không tồn tại';
  END IF;

  IF EXISTS (SELECT 1 FROM position WHERE PositionCode = v_PositionCode AND PositionID <> v_PositionID) THEN
    SIGNAL SQLSTATE '45000'
    SET MESSAGE_TEXT = 'PositionCode đã tồn tại';
  END IF;

  UPDATE position
  SET PositionCode = v_PositionCode,
      PositionName = v_PositionName
  WHERE PositionID = v_PositionID;
END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
