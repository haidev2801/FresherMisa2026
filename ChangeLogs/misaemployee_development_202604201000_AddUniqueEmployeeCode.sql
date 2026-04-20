-- Ensure EmployeeCode uniqueness at database level to prevent race condition.
USE misaemployee_development;

ALTER TABLE employee
ADD UNIQUE INDEX UQ_EmployeeCode (EmployeeCode);
