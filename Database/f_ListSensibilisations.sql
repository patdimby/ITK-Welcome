DROP FUNCTION IF EXISTS f_ListSensibilisations
GO

CREATE FUNCTION f_ListSensibilisations (
    @Mois VARCHAR(7) -- Mois demandé
)
-- LISTE DES UTILISATEURS ET DOCUMENTS SENSIBILISATIONS
-- SENSIBILISATIONS = Tous les documents soumis à une validation par questionnaire
-- POUR AFFICHER TOUS LES DOCUMENTS Y COMPRIS CEUX QUI NE NECESSITENT PAS D'ACTIONS, COMMENTER LA LIGNE :
-- "DOC.IsNoActionRequired = 0"
-- DECLARE @Mois VARCHAR(7)
-- SET @Mois = '2021-09'
-- SELECT * FROM f_ListSensibilisations ('2021-09') ORDER BY 1, 2,3,4,7,8
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
				CASE WHEN
					-- Si test réalisé et OK
					UDO.IsTested IS NOT NULL AND UQC.Score >= ISNULL(UQC.ScoreMinimal,0)
				THEN
					CONVERT(VARCHAR, UDO.IsTested, 23)
				ELSE
					-- Si test non réalisé ou tests KO
					NULL
				END
			ELSE
				 -- Si utilisateur pas dans la cible, alors 'N/A'
				 'N/A'
		END Resultat
	   ,USR.ID ID_User
	   ,DOC.ID ID_Document
	   ,UDO.IsTested Test_Terminé
	   ,UQC.Score
	   ,UQC.ScoreMinimal
	FROM
	  Histo_IntitekUser USR
	  CROSS JOIN Histo_Document DOC
	  LEFT  JOIN Histo_UserDocument UDO ON UDO.ID_IntitekUser = USR.ID AND UDO.ID_Document = DOC.ID AND UDO.Mois = @Mois
	  INNER JOIN Histo_DocumentLang DLA ON DLA.ID_Document = DOC.ID AND DLA.ID_Lang = 1 AND DLA.Mois = @Mois
	  LEFT  JOIN Histo_UserQcm UQC ON UQC.ID_IntitekUser = USR.ID AND UQC.ID_Document = DOC.ID AND SUBSTRING(UQC.Version,1,2) = SUBSTRING(DOC.Version,1,2) AND UQC.ID_Qcm = DOC.IDQcm AND UQC.Mois = @Mois
		-- Document même version majeure
	WHERE
	  USR.Mois = @Mois AND
	  DOC.Mois = @Mois AND
	  DOC.Inactif = 0 AND
	  DOC.IsNoActionRequired = 0 AND
	  USR.Active = 1 AND
	  DOC.Test = 1 -- Document soumis à une validation par questionnaire
GO
