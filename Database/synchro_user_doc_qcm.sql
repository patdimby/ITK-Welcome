-- Liste des utilisateurs topé TEST OK alors qu'ils n'ont pas de résultats de QCM
-- Mise à jour du isTested
-- Sauvegarde UserDocument
-- SELECT * INTO UserDocumentSave FROM UserDocument
UPDATE
	UserDocument
SET 
	IsTested = NULL
WHERE
	IsTested IS NOT NULL AND
	NOT EXISTS(	SELECT 1
				FROM UserQcm UQC
				INNER JOIN Document DOC -- Document même version majeure
					  ON UQC.ID_Document = DOC.ID AND UQC.ID_Qcm = DOC.IDQcm AND SUBSTRING(UQC.Version,1,2) = SUBSTRING(DOC.Version,1,2) 
				WHERE -- Ayant obtenu le score minimum sur le QCM du document
					UserDocument.ID_IntitekUser = UQC.ID_IntitekUser AND UserDocument.ID_Document = UQC.Id_Document AND
					UQC.Score >= ISNULL(UQC.ScoreMinimal,0)
				)
