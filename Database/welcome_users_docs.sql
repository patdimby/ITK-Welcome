-- WELCOME : 07/06/2021
-- LISTE DES UTILISATEURS ET DOCUMENTS + LEURS STATUTS
-- POUR AFFICHER TOUS LES DOCUMENTS Y COMPRIS CEUX QUI NE NECESSITENT PAS D'ACTIONS, COMMENTER LA LIGNE :
-- "DOC.IsNoActionRequired = 0 AND"

SELECT
    USR.ID ID_User
   ,USR.Email
   ,USR.FullName
   ,USR.EntityName Entité
   ,USR.AgencyName Agence
   ,DOC.ID ID_Document
   ,DLA.Name Nom_Document
   ,DCA.Name Categorie_Document
   ,SUC.Name Sous_Categorie_Document
   ,DOC.Version
   ,DOC.IsNoActionRequired Aucune_Action_Necessaire
   ,DOC.Approbation A_Approuver
   ,DOC.Test Test_A_Faire
   ,UDO.IsRead Lecture_Terminée
   ,UDO.IsApproved Est_Approuvé
   ,UDO.IsTested Test_Terminé
   ,UQC.Score
   ,UQC.ScoreMinimal
FROM
  IntitekUser USR
  CROSS JOIN Document DOC
  LEFT  JOIN UserDocument UDO ON UDO.ID_IntitekUser = USR.ID AND UDO.ID_Document = DOC.ID
  INNER JOIN DocumentLang DLA ON DLA.ID_Document = DOC.ID AND DLA.ID_Lang = 1
  LEFT  JOIN DocumentCategoryLang DCA ON DCA.ID_DocumentCategory = DOC.ID_Category AND DCA.ID_Lang = 1
  LEFT  JOIN SubCategoryLang SUC ON SUC.ID_SubCategory = DOC.ID_SubCategory AND SUC.ID_Lang = 1
  LEFT  JOIN UserQcm UQC ON UQC.ID_IntitekUser = USR.ID AND UQC.ID_Document = DOC.ID
WHERE
  USR.Active =  1 AND
  DOC.Inactif = 0 AND
  DOC.IsNoActionRequired = 0 AND
  (
  EXISTS (SELECT 1 FROM EntityDocument EDO WHERE EDO.ID_Document = DOC.ID AND EDO.EntityName = USR.EntityName AND ((EDO.AgencyName = USR.AgencyName) OR EDO.AgencyName IS NULL))
  OR
  EXISTS (SELECT 1 FROM ProfileUser UPR INNER JOIN ProfileDocument PDO ON PDO.ID_Profile = UPR.ID_Profile WHERE UPR.ID_IntitekUser = USR.ID AND PDO.ID_Document = DOC.ID)
  )
ORDER BY ID_User, Nom_Document, Categorie_Document, Sous_Categorie_Document
