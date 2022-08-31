DROP FUNCTION IF EXISTS f_ListDocsApprobations
GO

CREATE FUNCTION f_ListDocsApprobations (
    @Mois VARCHAR(7) -- Mois demandé
)
-- LISTE DES DOCUMENTS APPROBATIONS
-- SELECT * FROM f_ListDocsApprobations ('2021-09') ORDER BY 1, 2,3,4
RETURNS TABLE
AS
RETURN
	SELECT
	   DOC.Mois
	   ,DLA.Name Nom_Document
	   ,DOC.Version
	   ,DOC.ID ID_Document
	FROM
	  Histo_Document DOC
	  INNER JOIN Histo_DocumentLang DLA ON DLA.ID_Document = DOC.ID AND DLA.ID_Lang = 1 AND DLA.Mois = @Mois
	WHERE
	  DOC.Mois = @Mois AND
	  DOC.Inactif = 0 AND
	  DOC.IsNoActionRequired = 0 AND
	  DOC.Approbation = 1 AND
	  ISNULL(DOC.Test, 0) = 0 -- Document à approuver mais non soumis à une validation par questionnaire
GO
