DROP PROCEDURE IF EXISTS [p_CreSnapshot]
GO

CREATE PROCEDURE [p_CreSnapshot] (
	@Mois VARCHAR(7),
	@NB_Users INT OUTPUT,
	@NB_Docs INT OUTPUT
)
-- PROCEDURE qui crée un Snapshop de la BDD
-- Retourne les nb de Users et de Docs snapshotés
AS
BEGIN
	
	PRINT (CONCAT ('MOIS = ', @MOIS))
	
	-- Gérer sous forme de transaction pour éviter des données polluées en cas d'erreur
	-- En cas d'erreur : Je rollbacke TOUT
	SET XACT_ABORT ON
	
	BEGIN TRANSACTION
	
	-- PURGES PREALABLE
	DELETE FROM Histo_UserQcm WHERE Mois = @Mois
	DELETE FROM Histo_UserDocument WHERE Mois = @Mois
	DELETE FROM Histo_ProfileUser WHERE Mois = @Mois
	DELETE FROM Histo_ProfileDocument WHERE Mois = @Mois
	DELETE FROM Histo_EntityDocument WHERE Mois = @Mois
	DELETE FROM Histo_Profile WHERE Mois = @Mois
	DELETE FROM Histo_DocumentVersionLang WHERE Mois = @Mois
	DELETE FROM Histo_DocumentVersion WHERE Mois = @Mois	
	DELETE FROM Histo_DocumentLang WHERE Mois = @Mois	
	DELETE FROM Histo_Document WHERE Mois = @Mois
	DELETE FROM Histo_IntitekUser WHERE Mois = @Mois
	DELETE FROM Histo_AD WHERE Mois = @Mois

	-- CREATION SNAPSHOT
	-- AD
	INSERT INTO [Histo_AD]
	   (
	   [Mois]
	   ,[ID]
	   ,[Name]
	   ,[Address]
	   ,[Domain]
	   ,[Username]
	   ,[Password]
	   ,[ToBeSynchronized]
	   ,[LastSynchronized])
    SELECT
	   @Mois
	   ,[ID]
	   ,[Name]
	   ,[Address]
	   ,[Domain]
	   ,[Username]
	   ,[Password]
	   ,[ToBeSynchronized]
	   ,[LastSynchronized]
	FROM
		AD
	   
	-- USERS
	INSERT INTO [dbo].[Histo_IntitekUser]
	   ([Mois]
	   ,[ID]
	   ,[Username]
	   ,[Status]
	   ,[EntityName]
	   ,[AgencyName]
	   ,[isOnBoarding]
	   ,[EmailOnBoarding]
	   ,[PasswordOnBoarding]
	   ,[DateLastVisit]
	   ,[Email]
	   ,[ID_AD]
	   ,[FullName]
	   ,[FirstName]
	   ,[Active]
	   ,[Type]
	   ,[InactivityStart]
	   ,[InactivityEnd]
	   ,[InactivityReason]
	   ,[Departement]
	   ,[Division]
	   ,[ID_Manager]
	   ,[Pays]
	   ,[Plaque]
	   ,[EntryDate]
	   ,[ExitDate])
	SELECT
		@Mois
	   ,[ID]
	   ,[Username]
	   ,[Status]
	   ,[EntityName]
	   ,[AgencyName]
	   ,[isOnBoarding]
	   ,[EmailOnBoarding]
	   ,[PasswordOnBoarding]
	   ,[DateLastVisit]
	   ,[Email]
	   ,[ID_AD]
	   ,[FullName]
	   ,[FirstName]
	   ,[Active]
	   ,[Type]
	   ,[InactivityStart]
	   ,[InactivityEnd]
	   ,[InactivityReason]
	   ,[Departement]
	   ,[Division]
	   ,[ID_Manager]
	   ,[Pays]
	   ,[Plaque]
	   ,[EntryDate]
	   ,[ExitDate]
	From
		IntitekUser

	SET @NB_Users = @@ROWCOUNT
	
	-- Duplication DOC
	INSERT INTO [Histo_Document]
	   ([Mois]
	   ,[ID]
	   ,[Version]
	   ,[Date]
	   ,[Approbation]
	   ,[Test]
	   ,[Commentaire]
	   ,[ContentType]
	   ,[Extension]
	   ,[Inactif]
	   ,[TypeAffectation]
	   ,[IdQcm]
	   ,[ID_Category]
	   ,[isMajor]
	   ,[IsNoActionRequired]
	   ,[ID_UserCre]
	   ,[ID_UserUpd]
	   ,[ID_UserDel]
	   ,[DateCre]
	   ,[DateUpd]
	   ,[DateDel]
	   ,[ReadBrowser]
	   ,[ReadDownload]
	   ,[PhaseEmployee]
	   ,[PhaseOnboarding]
	   ,[isMetier]
	   ,[isStructure]
	   ,[ID_SubCategory])
	SELECT
		@Mois
	   ,[ID]
	   ,[Version]
	   ,[Date]
	   ,[Approbation]
	   ,[Test]
	   ,[Commentaire]
	   ,[ContentType]
	   ,[Extension]
	   ,[Inactif]
	   ,[TypeAffectation]
	   ,[IdQcm]
	   ,[ID_Category]
	   ,[isMajor]
	   ,[IsNoActionRequired]
	   ,[ID_UserCre]
	   ,[ID_UserUpd]
	   ,[ID_UserDel]
	   ,[DateCre]
	   ,[DateUpd]
	   ,[DateDel]
	   ,[ReadBrowser]
	   ,[ReadDownload]
	   ,[PhaseEmployee]
	   ,[PhaseOnboarding]
	   ,[isMetier]
	   ,[isStructure]
	   ,[ID_SubCategory]
	FROM
		[Document]
		
	SET @NB_Docs = @@ROWCOUNT
	
	-- Table DocumentLang
	INSERT INTO [Histo_DocumentLang]
	   ([Mois]
	   ,[ID_Document]
	   ,[ID_Lang]
	   ,[Name]
	   ,[NomOrigineFichier]
	   ,[Data])
     SELECT
		@Mois
	   ,[ID_Document]
	   ,[ID_Lang]
	   ,[Name]
	   ,[NomOrigineFichier]
	   ,[Data]
	FROM
		DocumentLang
		
	INSERT INTO [Histo_DocumentVersion]
	   ([Mois]
	   ,[ID]
	   ,[ID_Document]
	   ,[ID_UserCre]
	   ,[IsMajor]
	   ,[DateCre]
	   ,[ContentType]
	   ,[Extension]
	   ,[Version])
     SELECT
		@Mois
	   ,[ID]
	   ,[ID_Document]
	   ,[ID_UserCre]
	   ,[IsMajor]
	   ,[DateCre]
	   ,[ContentType]
	   ,[Extension]
	   ,[Version]
	 FROM
		DocumentVersion
		
	INSERT INTO [Histo_DocumentVersionLang]
	   ([Mois]
	   ,[ID_DocumentVersion]
	   ,[ID_Lang]
	   ,[Name]
	   ,[NomOrigineFichier]
	   ,[Data])
	SELECT
		@Mois
	   ,[ID_DocumentVersion]
	   ,[ID_Lang]
	   ,[Name]
	   ,[NomOrigineFichier]
	   ,[Data]
	FROM
		DocumentVersionLang
		
	INSERT INTO [Histo_EntityDocument]
	   ([Mois]
	   ,[ID]
	   ,[EntityName]
	   ,[AgencyName]
	   ,[EntityDocDate]
	   ,[ID_Document])
	 SELECT 
		@Mois
	   ,[ID]
	   ,[EntityName]
	   ,[AgencyName]
	   ,[EntityDocDate]
	   ,[ID_Document]
	FROM
		EntityDocument
		
	INSERT INTO [Histo_Profile]
	   ([Mois]
	   ,[ID]
	   ,[Name])
	SELECT 
        @Mois
	   ,[ID]
        ,[Name]
	FROM
		Profile
		
	INSERT INTO [dbo].[Histo_ProfileDocument]
	   ([Mois]
	   ,[ID]
	   ,[ID_Profile]
	   ,[ID_Document]
	   ,[Date])
     SELECT
		@Mois
	   ,[ID]
	   ,[ID_Profile]
	   ,[ID_Document]
	   ,[Date]
	FROM
		ProfileDocument
		
	INSERT INTO [dbo].[Histo_ProfileUser]
	   ([Mois]
	   ,[ID]
	   ,[ID_Profile]
	   ,[ID_IntitekUser])
     SELECT
		@Mois
	   ,[ID]
	   ,[ID_Profile]
	   ,[ID_IntitekUser]
	FROM
		ProfileUser
		
	INSERT INTO [dbo].[Histo_UserDocument]
	   ([Mois]
	   ,[ID]
	   ,[ID_IntitekUser]
	   ,[ID_Document]
	   ,[UpdateDate]
	   ,[IsRead]
	   ,[IsTested]
	   ,[IsApproved])
	 SELECT
		@Mois
	   ,[ID]
	   ,[ID_IntitekUser]
	   ,[ID_Document]
	   ,[UpdateDate]
	   ,[IsRead]
	   ,[IsTested]
	   ,[IsApproved]
	FROM
		UserDocument

	INSERT INTO [Histo_UserQcm]
	   ([Mois]
	   ,[ID]
	   ,[ID_IntitekUser]
	   ,[ID_Qcm]
	   ,[DateCre]
	   ,[DateFin]
	   ,[Score]
	   ,[ScoreMinimal]
	   ,[NbQuestions]
	   ,[ID_Document]
	   ,[Version])
     SELECT
		@Mois
	   ,[ID]
	   ,[ID_IntitekUser]
	   ,[ID_Qcm]
	   ,[DateCre]
	   ,[DateFin]
	   ,[Score]
	   ,[ScoreMinimal]
	   ,[NbQuestions]
	   ,[ID_Document]
	   ,[Version]
	 FROM
		UserQcm
	  
	-- Valider
	COMMIT
	
END
GO

-- TESTEUR
-- DECLARE @NB_Docs INT, @NB_Users INT
-- EXECUTE [dbo].[p_CreSnapshot] '2021-09', @NB_Users OUTPUT, @NB_Docs OUTPUT
-- PRINT CONCAT ('NB USERS, NB_DOCS : ', @NB_Users, ' - ', @NB_Docs)

grant execute on p_CreSnapshot to welcome;