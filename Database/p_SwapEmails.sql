DROP PROCEDURE IF EXISTS [p_SwapEmails]
GO

CREATE PROCEDURE [p_SwapEmails] (
	@OldEmail VARCHAR(150),
	@NewEmail VARCHAR(150),
	@Retour VARCHAR(500) OUTPUT
)
-- PROCEDURE qui SWAPE 2 ADRESSES EMAILS
AS
BEGIN	
	-- Gérer sous forme de transaction pour éviter des données polluées en cas d'erreur
	-- En cas d'erreur : Je rollbacke TOUT
	SET XACT_ABORT ON
	
	SET @Retour = Concat(CHAR(13), 'Bascule "', @OldEmail, '" vers "', @NewEmail, '", démarrage...')
	-- PRINT(Concat('Changement "', @OldEmail, '" vers "', @NewEmail, '", démarrage...'))
	
	BEGIN TRANSACTION
	-- Vérification existence ancienne adresse emails
	DECLARE @ID_oldUser int
	SELECT  @ID_oldUser = ID FROM IntitekUser WHERE Email = @OldEmail
	-- Si pas trouvé, alors sortir avec Code Retour 1
	-- PRINT (Concat('@ID_oldUser = ', @ID_oldUser))
	IF @ID_oldUser IS NULL
	BEGIN
		SET @Retour = CONCAT(@Retour, CHAR(13), '/!\ Email ', @OldEmail, ' non trouvé en base !')
		COMMIT
		RETURN
	END
	
	-- Suppression nouvel Email si existe
	DECLARE @ID_newUser int
	SELECT  @ID_newUser = ID FROM IntitekUser WHERE Email = @NewEmail
	-- Si trouvé
	IF @ID_newUser IS NOT NULL
	BEGIN
		SET @Retour = Concat(@Retour, CHAR(13), 'Nouvel email ', @NewEmail, ' déjà présent en base, suppression...')
		DELETE FROM ProfileUser WHERE ID_IntitekUser = @ID_newUser
		DELETE FROM HistoEmails WHERE ID_IntitekUser = @ID_newUser
		DELETE FROM HistoActions WHERE ID_IntitekUser = @ID_newUser
		DELETE FROM UserQcmReponse WHERE ID_UserQcm IN (SELECT ID FROM UserQcm WHERE ID_IntitekUser = @ID_newUser)
		DELETE FROM UserQcm WHERE ID_IntitekUser = @ID_newUser
		DELETE FROM UserDocument WHERE ID_IntitekUser = @ID_newUser
		DELETE FROM HistoUserQcmDocVersion WHERE ID_IntitekUser = @ID_newUser
		UPDATE Document SET ID_UserCre = @ID_oldUser, ID_UserUpd = @ID_oldUser, ID_UserDel = @ID_oldUser WHERE ID_UserCre = @ID_newUser
		UPDATE DocumentVersion SET ID_UserCre = @ID_oldUser WHERE ID_UserCre = @ID_newUser
		DELETE FROM IntitekUser WHERE ID = @ID_newUser
	END
	ELSE
		SET @Retour = Concat(@Retour, CHAR(13), 'Nouvel email ', @NewEmail, ' non présent en base...')
	
	-- Basculement OldEMAIL vers NewEMAIL
	UPDATE IntitekUser SET Email = @NewEmail WHERE Email = @OldEmail
	
	-- Valider
	COMMIT
	
	-- Message RETOUR
	SET @Retour = Concat(@Retour, CHAR(13), 'Bascule "', @OldEmail, '" vers "', @NewEmail, '" : Terminée avec succès')
END
GO

-- TESTEUR
-- DECLARE @Retour VARCHAR(500)
-- EXECUTE [dbo].[p_SwapEmails] 'arasamoely@astek.mg', 'arasamoely@groupeastek.mg', @Retour OUTPUT
-- PRINT @Retour