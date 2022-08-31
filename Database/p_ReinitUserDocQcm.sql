DROP PROCEDURE IF EXISTS [p_ReinitUserDocQcm]
GO

CREATE PROCEDURE [p_ReinitUserDocQcm] (
	@ID_Document INT,
	@isMajor BIT) -- Si changement de version majeure
-- PROCEDURE QUI SYNCHRONISE L'ETAT DES QCM REALISES
-- A APPELER LORS DE LA MISE A JOUR DU QCM ET/OU DE VERSION MAJEURE D'UN DOCUMENT 
-- EXECUTE [dbo].[p_ReinitUserDocQcm] 215, 1
AS
BEGIN
	-- Gérer sous forme de transaction pour éviter des données polluées en cas d'erreur
	-- En cas d'erreur : Je rollbacke TOUT
	SET XACT_ABORT ON
	
	-- DEBUT TRANSACTION
	BEGIN TRANSACTION
	-- Suppression des résultats QCM dans tous les cas
	DELETE FROM UserQcmReponse WHERE ID_UserQcm IN (SELECT ID FROM UserQcm WHERE ID_Document = @ID_Document)
	DELETE FROM UserQcm WHERE ID_Document = @ID_Document
	
	-- Si nouvelle version majeure : Suppression de toutes les actions
	IF(@isMajor = 1)
		DELETE FROM UserDocument WHERE ID_Document = @ID_Document
	ELSE
	-- Sinon (mise à jour QCM uniquement) : Effacement juste de la date de test
	-- Les dates d'approbation / lecture étant maintenues
		UPDATE UserDocument SET IsTested = NULL WHERE ID_Document = @ID_Document

	-- Valider
	COMMIT
END
GO
