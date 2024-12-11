DELIMITER ;;

DROP PROCEDURE IF EXISTS `CreateUser`;

CREATE PROCEDURE `CreateUser` (
    IN p_Id VARCHAR(450),
    IN p_Role VARCHAR(50)
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
        @sqlstate = RETURNED_SQLSTATE,
        @errno = MYSQL_ERRNO,
        @text = MESSAGE_TEXT;
        
        ROLLBACK;
        
        -- Return the error details
        SELECT 
            'Error' as status,
            @sqlstate as sql_state,
            @errno as error_number,
            @text as message;
            
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = @text;
    END;

    START TRANSACTION;

    -- Create role-specific entity
    CASE p_Role
        WHEN 'Menaxher' THEN
            INSERT INTO menaxhers (Id, UserId, CreatedAt)
            VALUES (UUID(), p_Id, UTC_TIMESTAMP());
        WHEN 'Etiketues' THEN
            INSERT INTO etiketueses (Id, UserId, CreatedAt)
            VALUES (UUID(), p_Id, UTC_TIMESTAMP());
        WHEN 'Komercialist' THEN
            INSERT INTO komercialist (Id, UserId, CreatedAt)
            VALUES (UUID(), p_Id, UTC_TIMESTAMP());
        WHEN 'Shofer' THEN
            INSERT INTO shofers (Id, UserId, CreatedAt)
            VALUES (UUID(), p_Id, UTC_TIMESTAMP());
        WHEN 'Admin' THEN
            -- Admin role doesn't need a role-specific entity
            SET @dummy = 1;
    END CASE;

    COMMIT;
    
    -- Final success message
    SELECT 'Success' as status;
END ;;

DELIMITER ; 