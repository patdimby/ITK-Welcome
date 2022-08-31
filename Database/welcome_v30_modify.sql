-- 2021-09-29
-- WELCOME V3
-- Adding column Illustration to QuestionLang table

ALTER TABLE [QuestionLang]
	ADD [Illustration] [varbinary](max) NULL;

-- End

-- 2021-10-08
-- WELCOME V3
-- Adding column isMagazine to Document table

ALTER TABLE [Document]
	ADD [isMagazine] bit NOT NULL DEFAULT 0;

-- End

INSERT INTO [dbo].[Batchs]
           ([ProgName]
           ,[Description]
           ,[Frequency]
           ,[LastExecution]
           ,[ForceExecution])
     VALUES
           ('CreateSnapshot'
           ,'Snapshot Histo Mensuel'
           ,NULL
           ,NULL
           ,1)
GO
