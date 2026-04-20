-- Position procedures for FresherMisa2026
-- Compatible with BaseRepository dynamic parameters (@v_<PropertyName>)
-- Target DB from appsettings: misa_employee_development

USE misa_employee_development;

DELIMITER $$

DROP PROCEDURE IF EXISTS Proc_InsertPosition $$
CREATE PROCEDURE Proc_InsertPosition(
    IN v_CreatedBy VARCHAR(100),
    IN v_CreateDate DATETIME,
    IN v_ModifiedBy VARCHAR(100),
    IN v_ModifiedDate DATETIME,
    IN v_State INT,
    IN v_IsDeleted BOOLEAN,
    IN v_PositionID CHAR(36),
    IN v_PositionCode VARCHAR(20),
    IN v_PositionName VARCHAR(255)
)
BEGIN
    INSERT INTO `position` (
        PositionID,
        PositionCode,
        PositionName
    )
    VALUES (
        v_PositionID,
        v_PositionCode,
        v_PositionName
    );
END $$

DROP PROCEDURE IF EXISTS Proc_UpdatePosition $$
CREATE PROCEDURE Proc_UpdatePosition(
    IN v_CreatedBy VARCHAR(100),
    IN v_CreateDate DATETIME,
    IN v_ModifiedBy VARCHAR(100),
    IN v_ModifiedDate DATETIME,
    IN v_State INT,
    IN v_IsDeleted BOOLEAN,
    IN v_PositionID CHAR(36),
    IN v_PositionCode VARCHAR(20),
    IN v_PositionName VARCHAR(255)
)
BEGIN
    UPDATE `position`
    SET PositionCode = v_PositionCode,
        PositionName = v_PositionName
    WHERE PositionID = v_PositionID;
END $$

DROP PROCEDURE IF EXISTS Proc_DeletePositionById $$
CREATE PROCEDURE Proc_DeletePositionById(
    IN v_PositionID CHAR(36)
)
BEGIN
    DELETE FROM `position`
    WHERE PositionID = v_PositionID;
END $$

DELIMITER ;
