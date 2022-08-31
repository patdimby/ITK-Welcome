DROP FUNCTION IF EXISTS f_ListApprobations
GO

CREATE FUNCTION f_ListApprobations (
    @Mois VARCHAR(7) -- Mois demandé
)
-- LISTE DES UTILISATEURS ET DOCUMENTS APPROBATION
-- POUR AFFICHER TOUS LES DOCUMENTS Y COMPRIS CEUX QUI NE NECESSITENT PAS D'ACTIONS, COMMENTER LA LIGNE :
-- "DOC.IsNoActionRequired = 0"
-- DECLARE @Mois VARCHAR(7)
-- SET @Mois = '2021-09'
-- SELECT * FROM f_ListApprobations ('2021-09') ORDER BY 1, 2,3,4,7,8
RETURNS TABLE
AS
RETURN
	SELECT
	   DOC.Mois
	   ,USR.EntityName Entité
	   ,USR.AgencyName Agence
	   ,USR.FullName
	   ,USR.Email
	   ,CASE WHEN EntryDate <= CONVERT(DATE, CONCAT(@Mois, '-01'), 23) And ( ExitDate >= CONVERT(DATE, CONCAT(@Mois, '-01'), 23)  OR ExitDate IS NULL)
	    THEN 1
		ELSE 0
		END Actif
	   ,CASE WHEN InactivityStart <= CONVERT(DATE, CONCAT(@Mois, '-01'), 23) And ( InactivityStart >= CONVERT(DATE, CONCAT(@Mois, '-01'), 23) OR InactivityStart IS NULL)
	    THEN 0
		ELSE 1
		END Present
	   ,DLA.Name Nom_Document
	   ,DOC.Version
	   ,CASE WHEN -- Si utilisateur dans la cible
			EXISTS (SELECT 1 FROM Histo_EntityDocument EDO
					WHERE EDO.Mois = @Mois AND EDO.ID_Document = DOC.ID AND EDO.EntityName = USR.EntityName
					  AND ((EDO.AgencyName = USR.AgencyName) OR EDO.AgencyName IS NULL) 
					  AND ((USR.Type = 'MET' AND DOC.isMetier = 1) OR (USR.Type = 'STR' AND DOC.isStructure = 1))
					)
			OR EXISTS (SELECT 1 FROM Histo_ProfileUser UPR
					INNER JOIN Histo_ProfileDocument PDO ON PDO.ID_Profile = UPR.ID_Profile
					WHERE UPR.Mois = @Mois AND PDO.Mois = @Mois AND UPR.ID_IntitekUser = USR.ID AND PDO.ID_Document = DOC.ID
					  AND ((USR.Type = 'MET' AND DOC.isMetier = 1) OR (USR.Type = 'STR' AND DOC.isStructure = 1))
					  )
			THEN 
				-- Date d'approbation
				CONVERT(VARCHAR, UDO.IsApproved, 23)
			ELSE
				-- Si utilisateur pas dans la cible, alors 'N/A'
				'N/A'
		END Resultat
	   ,USR.ID ID_User
	   ,DOC.ID ID_Document
	FROM
	  Histo_IntitekUser USR
	  CROSS JOIN Histo_Document DOC
	  LEFT  JOIN Histo_UserDocument UDO ON UDO.ID_IntitekUser = USR.ID AND UDO.ID_Document = DOC.ID AND UDO.Mois = @Mois
	  INNER JOIN Histo_DocumentLang DLA ON DLA.ID_Document = DOC.ID AND DLA.ID_Lang = 1 AND DLA.Mois = @Mois
	WHERE
	  USR.Mois = @Mois AND
	  DOC.Mois = @Mois AND
	  DOC.Inactif = 0 AND
	  DOC.IsNoActionRequired = 0 AND
	  DOC.Approbation = 1 AND
	  ISNULL(DOC.Test, 0) = 0 AND -- Document à approuver mais non soumis à une validation par questionnaire
	  USR.Active = 1
GO