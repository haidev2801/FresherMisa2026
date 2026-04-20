CREATE DEFINER=`root`@`localhost` PROCEDURE `Proc_Employee_FilterPaging_2`(
    IN p_DepartmentID CHAR(36),
    IN p_PositionID   CHAR(36),
    IN p_SalaryFrom   DECIMAL(18,2),
    IN p_SalaryTo     DECIMAL(18,2),
    IN p_Gender       TINYINT,
    IN p_HireDateFrom DATE,
    IN p_HireDateTo   DATE,
    IN p_Limit        INT UNSIGNED,
    IN p_Offset       INT UNSIGNED,
    IN p_OrderBy      VARCHAR(100)
)
BEGIN
    DECLARE v_where   TEXT    DEFAULT ' WHERE 1=1 ';
    DECLARE v_orderBy VARCHAR(200) DEFAULT '';

    -- ------------------------------------------------
    -- 1. Validation
    -- ------------------------------------------------
    IF p_Limit IS NULL OR p_Limit = 0 THEN
        SET p_Limit = 20;
    END IF;
    IF p_Offset IS NULL THEN
        SET p_Offset = 0;
    END IF;
    IF p_SalaryFrom IS NOT NULL AND p_SalaryTo IS NOT NULL
       AND p_SalaryFrom > p_SalaryTo THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'SalaryFrom must be <= SalaryTo';
    END IF;

    -- ------------------------------------------------
    -- 2. Build WHERE (dùng session variable để truyền
    --    giá trị typed vào PREPARE — tránh SQL injection)
    -- ------------------------------------------------
    IF p_DepartmentID IS NOT NULL THEN
        SET @p_DepartmentID = p_DepartmentID;
        SET v_where = CONCAT(v_where, ' AND e.DepartmentID = @p_DepartmentID');
    END IF;

    IF p_PositionID IS NOT NULL THEN
        SET @p_PositionID = p_PositionID;
        SET v_where = CONCAT(v_where, ' AND e.PositionID = @p_PositionID');
    END IF;

    IF p_SalaryFrom IS NOT NULL THEN
        SET @p_SalaryFrom = p_SalaryFrom;
        SET v_where = CONCAT(v_where, ' AND e.Salary >= @p_SalaryFrom');
    END IF;

    IF p_SalaryTo IS NOT NULL THEN
        SET @p_SalaryTo = p_SalaryTo;
        SET v_where = CONCAT(v_where, ' AND e.Salary <= @p_SalaryTo');
    END IF;

    IF p_Gender IS NOT NULL THEN
        SET @p_Gender = p_Gender;
        SET v_where = CONCAT(v_where, ' AND e.Gender = @p_Gender');
    END IF;

    IF p_HireDateFrom IS NOT NULL THEN
        SET @p_HireDateFrom = p_HireDateFrom;
        SET v_where = CONCAT(v_where, ' AND e.HireDate >= @p_HireDateFrom');
    END IF;

    IF p_HireDateTo IS NOT NULL THEN
        SET @p_HireDateTo = p_HireDateTo;
        SET v_where = CONCAT(v_where, ' AND e.HireDate <= @p_HireDateTo');
    END IF;

    -- ------------------------------------------------
    -- 3. Build ORDER BY (whitelist — an toàn injection)
    -- ------------------------------------------------
    SET v_orderBy = CASE p_OrderBy
        WHEN 'salary_asc'  THEN ' ORDER BY e.Salary ASC,    e.EmployeeID DESC'
        WHEN 'salary_desc' THEN ' ORDER BY e.Salary DESC,   e.EmployeeID DESC'
        WHEN 'hire_asc'    THEN ' ORDER BY e.HireDate ASC,  e.EmployeeID DESC'
        WHEN 'hire_desc'   THEN ' ORDER BY e.HireDate DESC, e.EmployeeID DESC'
        ELSE                    ' ORDER BY e.EmployeeID DESC'
    END;

    -- ------------------------------------------------
    -- 4. Set paging params vào session variable
    -- ------------------------------------------------
    SET @p_Limit  = p_Limit;
    SET @p_Offset = p_Offset;

    -- ------------------------------------------------
    -- 5. Execute data query
    -- ------------------------------------------------
     SET @v_sql = CONCAT(
        'SELECT e.* FROM Employee e',
        v_where,
        v_orderBy,
        ' LIMIT ', p_Limit, ' OFFSET ', p_Offset
    );

    PREPARE stmt_data FROM @v_sql;
    EXECUTE stmt_data;
    DEALLOCATE PREPARE stmt_data;

    -- ------------------------------------------------
    -- 6. Execute count query (thay thế SQL_CALC_FOUND_ROWS)
    -- ------------------------------------------------
    SET @v_sql_count = CONCAT(
        'SELECT COUNT(*) AS TotalCount FROM Employee e',
        v_where
    );

    PREPARE stmt_count FROM @v_sql_count;
    EXECUTE stmt_count;
    DEALLOCATE PREPARE stmt_count;
END