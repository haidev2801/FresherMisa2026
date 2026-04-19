-- ============================================================
-- Migration: Thêm cột HireDate vào bảng Employee
-- Database: FresherMisa2026
-- Created: 19/04/2026
-- ============================================================

USE misaemployee_development;

-- Thêm cột HireDate vào bảng Employee
ALTER TABLE employee
ADD COLUMN HireDate datetime DEFAULT NULL COMMENT 'Ngày vào làm' AFTER Salary;
