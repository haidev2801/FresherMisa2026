CREATE DATABASE  IF NOT EXISTS `misaemployee_development` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `misaemployee_development`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: misaemployee_development
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `department`
--

DROP TABLE IF EXISTS `department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `department` (
  `DepartmentID` char(36) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Khóa chính phòng ban',
  `DepartmentCode` varchar(20) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Mã phòng ban',
  `DepartmentName` varchar(255) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Tên phòng ban',
  `Description` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Diễn giải',
  PRIMARY KEY (`DepartmentID`),
  UNIQUE KEY `UQ_DepartmentCode` (`DepartmentCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci AVG_ROW_LENGTH=2048;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `department`
--

LOCK TABLES `department` WRITE;
/*!40000 ALTER TABLE `department` DISABLE KEYS */;
INSERT INTO `department` VALUES ('550e8400-e29b-41d4-a716-446655440002','DEV','Developer','Lập trình viên'),('550e8400-e29b-41d4-a716-446655440010','RND','Research & Development','Nghiên cứu và phát triển sản phẩm'),('550e8400-e29b-41d4-a716-446655440011','CS','Customer Service','Chăm sóc khách hàng'),('550e8400-e29b-41d4-a716-446655440012','SALE','Sales','Kinh doanh và bán hàng'),('550e8400-e29b-41d4-a716-446655440013','ADMIN','Administration','Hành chính tổng hợp'),('550e8400-e29b-41d4-a716-446655440014','LEGAL','Legal','Pháp chế doanh nghiệp'),('550e8400-e29b-41d4-a716-446655440015','DATA','Data Engineering','Quản lý và phân tích dữ liệu'),('550e8400-e29b-41d4-a716-446655440016','SEC','Security','An ninh hệ thống'),('550e8400-e29b-41d4-a716-446655440017','SUP','Support','Hỗ trợ kỹ thuật');
/*!40000 ALTER TABLE `department` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee` (
  `EmployeeID` char(36) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Khóa chính nhân viên',
  `EmployeeCode` varchar(20) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Mã nhân viên',
  `EmployeeName` varchar(100) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Tên nhân viên',
  `Gender` int DEFAULT '0' COMMENT 'Giới tính: 0-Nữ, 1-Nam, 2-Khác',
  `DateOfBirth` date DEFAULT NULL COMMENT 'Ngày sinh',
  `PhoneNumber` varchar(50) COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Số điện thoại',
  `Email` varchar(100) COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Email',
  `Address` varchar(255) COLLATE utf8mb4_general_ci DEFAULT NULL COMMENT 'Địa chỉ',
  `DepartmentID` char(36) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Phòng ban',
  `PositionID` char(36) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Chức vụ',
  `Salary` decimal(18,4) DEFAULT '0.0000' COMMENT 'Lương cơ bản',
  `CreatedDate` datetime DEFAULT NULL COMMENT 'Ngày tạo',
  `HireDateFrom` datetime DEFAULT NULL COMMENT 'Thời điểm bắt đầu làm việc',
  `HireDateTo` datetime DEFAULT NULL COMMENT 'Thời điểm kết thúc làm việc'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci AVG_ROW_LENGTH=1092;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

LOCK TABLES `employee` WRITE;
/*!40000 ALTER TABLE `employee` DISABLE KEYS */;
INSERT INTO `employee` VALUES ('e0000001-0000-0000-0000-000000000001','EMP001','Nguyễn Văn An',1,'1995-03-15','0912345678','an.nguyen@misa.com','Hà Nội','550e8400-e29b-41d4-a716-446655440002','11111111-1111-1111-1111-111111111111',18000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000002','EMP002','Trần Thị Mai',0,'1998-07-20','0987654321','mai.tran@misa.com','Hồ Chí Minh','550e8400-e29b-41d4-a716-446655440002','22222222-2222-2222-2222-222222222222',15000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000003','EMP003','Phạm Minh Hoàng',1,'1993-11-05','0905123456','hoang.pham@misa.com','Đà Nẵng','550e8400-e29b-41d4-a716-446655440002','33333333-3333-3333-3333-333333333333',35000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00','2026-04-10 17:30:00'),('e0000001-0000-0000-0000-000000000004','EMP004','Lê Thị Hương',0,'1996-02-10','0934567890','huong.le@misa.com','Hà Nội','550e8400-e29b-41d4-a716-446655440000','44444444-4444-4444-4444-444444444444',14000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000005','EMP005','Nguyễn Quốc Bảo',1,'1992-09-18','0911223344','bao.nguyen@misa.com','Hà Nội','550e8400-e29b-41d4-a716-446655440000','44444444-4444-4444-4444-444444444444',16000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000006','EMP006','Đặng Thị Lan',0,'1994-04-22','0977112233','lan.dang@misa.com','HCM','550e8400-e29b-41d4-a716-446655440001','55555555-5555-5555-5555-555555555555',20000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000007','EMP007','Hoàng Văn Sơn',1,'1990-01-30','0944556677','son.hoang@misa.com','HCM','550e8400-e29b-41d4-a716-446655440001','55555555-5555-5555-5555-555555555555',22000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000008','EMP008','Vũ Thị Thu',0,'1997-06-12','0966889900','thu.vu@misa.com','Hà Nội','550e8400-e29b-41d4-a716-446655440003','66666666-6666-6666-6666-666666666666',17000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000009','EMP009','Phan Anh Tú',1,'1999-12-01','0922113344','tu.phan@misa.com','HCM','550e8400-e29b-41d4-a716-446655440003','66666666-6666-6666-6666-666666666666',16500000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00','2026-04-10 17:30:00'),('e0000001-0000-0000-0000-000000000010','EMP010','Bùi Văn Đức',1,'1991-08-08','0933445566','duc.bui@misa.com','Đà Nẵng','550e8400-e29b-41d4-a716-446655440004','77777777-7777-7777-7777-777777777777',21000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000011','EMP011','Lý Thị Mỹ',0,'1996-10-10','0988112233','my.ly@misa.com','Hà Nội','550e8400-e29b-41d4-a716-446655440004','77777777-7777-7777-7777-777777777777',19500000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000012','EMP012','Nguyễn Trung Kiên',1,'1998-05-05','0912000001','kien.nguyen@misa.com','Hà Nội','550e8400-e29b-41d4-a716-446655440002','11111111-1111-1111-1111-111111111111',17500000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000013','EMP013','Trịnh Hà My',0,'1997-09-09','0912000002','my.trinh@misa.com','HCM','550e8400-e29b-41d4-a716-446655440002','22222222-2222-2222-2222-222222222222',15500000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000014','EMP014','Đỗ Minh Tuấn',1,'1994-07-07','0912000003','tuan.do@misa.com','HCM','550e8400-e29b-41d4-a716-446655440002','11111111-1111-1111-1111-111111111111',19000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL),('e0000001-0000-0000-0000-000000000015','EMP015','Phạm Thị Linh',0,'1995-03-03','0912000004','linh.pham@misa.com','Đà Nẵng','550e8400-e29b-41d4-a716-446655440002','22222222-2222-2222-2222-222222222222',16000000.0000,'2026-04-15 09:29:18','2026-01-01 08:00:00',NULL);
/*!40000 ALTER TABLE `employee` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `position`
--

DROP TABLE IF EXISTS `position`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `position` (
  `PositionID` char(36) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Khóa chính chức vụ',
  `PositionCode` varchar(20) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Mã chức vụ',
  `PositionName` varchar(255) COLLATE utf8mb4_general_ci NOT NULL COMMENT 'Tên chức vụ',
  PRIMARY KEY (`PositionID`),
  UNIQUE KEY `UQ_PositionCode` (`PositionCode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci AVG_ROW_LENGTH=2340;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `position`
--

LOCK TABLES `position` WRITE;
/*!40000 ALTER TABLE `position` DISABLE KEYS */;
INSERT INTO `position` VALUES ('11111111-1111-1111-1111-111111111111','DEV','Developer'),('22222222-2222-2222-2222-222222222222','QA','Quality Assurance'),('33333333-3333-3333-3333-333333333333','PM','Project Manager'),('44444444-4444-4444-4444-444444444444','HR','HR Specialist'),('55555555-5555-5555-5555-555555555555','ACC','Accountant'),('66666666-6666-6666-6666-666666666666','MKT','Marketing Executive'),('77777777-7777-7777-7777-777777777777','OPS','Operation Staff');
/*!40000 ALTER TABLE `position` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-18 23:22:51
