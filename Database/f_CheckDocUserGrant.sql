DROP FUNCTION IF EXISTS f_CheckDocUserGrant
GO

CREATE FUNCTION f_CheckDocUserGrant (
    @ID_Document INT, -- ID Document
    @ID_User INT -- ID User
)
--Vérification DROIT d'un USER sur un DOCUMENT
-- EXEMPLE :
-- SELECT dbo.f_CheckDocUserGrant(96,27) AS 'CHECK DROIT';
-- Retourne 1 si DROIT OK, 0 sinon
RETURNS BIT
AS
BEGIN
	DECLARE @retour INT=0	
	
	SELECT
		@retour=1
	FROM
		IntitekUser USR
	WHERE
		USR.ID = @ID_User AND
		USR.Active = 1 AND
		(EXISTS (SELECT 1 FROM EntityDocument EDO
				INNER JOIN Document DOC ON EDO.ID_Document = DOC.ID AND DOC.Inactif = 0 AND EDO.ID_Document = @ID_Document
				WHERE EDO.EntityName = USR.EntityName
				  AND ((EDO.AgencyName = USR.AgencyName) OR EDO.AgencyName IS NULL)
				  AND ((USR.Type = 'MET' AND DOC.isMetier = 1) OR (USR.Type = 'STR' AND DOC.isStructure = 1))
				)
		 OR EXISTS (SELECT 1 FROM ProfileUser UPR
				INNER JOIN ProfileDocument PDO ON PDO.ID_Profile = UPR.ID_Profile
				INNER JOIN Document DOC ON PDO.ID_Document = DOC.ID AND DOC.Inactif = 0 AND DOC.ID = @ID_Document
				WHERE UPR.ID_IntitekUser = USR.ID
				  AND ((USR.Type = 'MET' AND DOC.isMetier = 1) OR (USR.Type = 'STR' AND DOC.isStructure = 1))
				)
		)
	
	-- RETOUR
	RETURN @retour
END
GO
