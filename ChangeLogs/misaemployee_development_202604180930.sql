USE misaemployee_development;

SET @index_exists := (
    SELECT COUNT(1)
    FROM information_schema.statistics
    WHERE table_schema = DATABASE()
      AND table_name = 'employee'
      AND index_name = 'UQ_EmployeeCode'
);

SET @sql := IF(
    @index_exists = 0,
    'ALTER TABLE employee ADD UNIQUE INDEX UQ_EmployeeCode (EmployeeCode);',
    'SELECT ''UQ_EmployeeCode already exists'';'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
