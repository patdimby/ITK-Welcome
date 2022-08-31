-------------------
-- REQUETE GENERALE
-------------------
SELECT
	DOC.IsMetier, DOC.IsStructure, DLA.*
FROM
	Document DOC
	LEFT JOIN DocumentLang DLA ON DOC.ID = DLA.ID_Document AND DLA.ID_Lang = 1
WHERE
	(
	EXISTS (SELECT 1 FROM EntityDocument EDO
			WHERE EDO.ID_Document = DOC.ID AND EDO.EntityName = 'IFP' AND
			((EDO.AgencyName = 'BORDEAUX') OR EDO.AgencyName IS NULL)
			 AND (('STR' = 'MET' AND DOC.isMetier = 1) OR ('STR' = 'STR' AND DOC.isStructure = 1))
			)
	OR
	EXISTS (SELECT 1 FROM ProfileUser UPR
			INNER JOIN ProfileDocument PDO ON PDO.ID_Profile = UPR.ID_Profile
			WHERE UPR.ID_IntitekUser = 31 AND PDO.ID_Document = DOC.ID
			 AND (('STR' = 'MET' AND DOC.isMetier = 1) OR ('STR' = 'STR' AND DOC.isStructure = 1))
			)
	)
	 AND DOC.Inactif = 0
	 AND DOC.IsNoActionRequired = 0 -- Action nécessaire
	 AND DOC.Test = 1 -- Test à faire
ORDER BY DLA.Name, DOC.Version

----------------
-- REQUETE HISTO
----------------
SELECT
	DOC.IsMetier, DOC.IsStructure, DLA.*
FROM
	Histo_Document DOC
	LEFT JOIN Histo_DocumentLang DLA ON DOC.ID = DLA.ID_Document AND DLA.Mois = '2021-11' AND DLA.ID_Lang = 1
WHERE
	(
	EXISTS (SELECT 1 FROM Histo_EntityDocument EDO
			WHERE EDO.Mois = '2021-11' AND EDO.ID_Document = DOC.ID AND EDO.EntityName = 'IFP' AND
			((EDO.AgencyName = 'BORDEAUX') OR EDO.AgencyName IS NULL)
			 AND (('STR' = 'MET' AND DOC.isMetier = 1) OR ('STR' = 'STR' AND DOC.isStructure = 1))
			)
	OR
	EXISTS (SELECT 1 FROM Histo_ProfileUser UPR
			INNER JOIN Histo_ProfileDocument PDO ON PDO.ID_Profile = UPR.ID_Profile
			WHERE UPR.Mois = '2021-11' AND PDO.Mois = '2021-11' AND UPR.ID_IntitekUser = 31 AND PDO.ID_Document = DOC.ID
			 AND (('STR' = 'MET' AND DOC.isMetier = 1) OR ('STR' = 'STR' AND DOC.isStructure = 1))
			)
	)
	 AND DOC.Mois = '2021-11'
	 AND DOC.Inactif = 0
	 AND DOC.IsNoActionRequired = 0 -- Action nécessaire
	 AND DOC.Test = 1 -- Test à faire
ORDER BY DLA.Name, DOC.Version

