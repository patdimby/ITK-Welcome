/****** AJOUT DES TABLES HISTORIQUES ******/

/****** Object:  Table .[Document]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_Document](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[Version] [varchar](3) NULL,
	[Date] [date] NULL,
	[Approbation] [int] NULL,
	[Test] [int] NULL,
	[Commentaire] [varchar](255) NULL,
	[ContentType] [nvarchar](30) NULL,
	[Extension] [varchar](5) NULL,
	[Inactif] [bit] NULL,
	[TypeAffectation] [nvarchar](13) NULL,
	[IdQcm] [int] NULL,
	[ID_Category] [int] NULL,
	[isMajor] [bit] NOT NULL,
	[IsNoActionRequired] [bit] NOT NULL,
	[ID_UserCre] [int] NULL,
	[ID_UserUpd] [int] NULL,
	[ID_UserDel] [int] NULL,
	[DateCre] [datetime] NULL,
	[DateUpd] [datetime] NULL,
	[DateDel] [datetime] NULL,
	[ReadBrowser] [bit] NOT NULL,
	[ReadDownload] [bit] NOT NULL,
	[PhaseEmployee] [bit] NOT NULL,
	[PhaseOnboarding] [bit] NOT NULL,
	[isMetier] [bit] NOT NULL,
	[isStructure] [bit] NOT NULL,
	[ID_SubCategory] [int] NULL,
 CONSTRAINT [PK_Histo_Document] PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [Histo_DocumentLang](
    [Mois] [varchar](7) NOT NULL,
	[ID_Document] [int] NOT NULL,
	[ID_Lang] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[NomOrigineFichier] [nvarchar](255) NOT NULL,
	[Data] [varbinary](max) NULL,
 CONSTRAINT [PK_Histo_DocumentLang] PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID_Document] ASC,
	[ID_Lang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Histo_DocumentVersion]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_DocumentVersion](
    [Mois] [varchar](7) NOT NULL,
	[ID] [bigint] NOT NULL,
	[ID_Document] [int] NOT NULL,
	[ID_UserCre] [int] NOT NULL,
	[IsMajor] [bit] NOT NULL,
	[DateCre] [datetime] NOT NULL,
	[ContentType] [nvarchar](30) NOT NULL,
	[Extension] [varchar](5) NOT NULL,
	[Version] [varchar](3) NOT NULL,
 CONSTRAINT [PK_HISTO_DOCUMENTVERSION] PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Histo_DocumentVersionLang]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_DocumentVersionLang](
    [Mois] [varchar](7) NOT NULL,
	[ID_DocumentVersion] [bigint] NOT NULL,
	[ID_Lang] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[NomOrigineFichier] [varchar](255) NOT NULL,
	[Data] [varbinary](max) NULL,
 CONSTRAINT [PK_HISTO_DOCUMENTVERSIONLANG] PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID_DocumentVersion] ASC,
	[ID_Lang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  TABLE [Histo_EntityDocument]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_EntityDocument](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[EntityName] [varchar](255) NULL,
	[AgencyName] [varchar](255) NULL,
	[EntityDocDate] [date] NULL,
	[ID_Document] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/***** Object:  TABLE [Histo_AD]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_AD](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Address] [varchar](100) NOT NULL,
	[Domain] [varchar](100) NULL,
	[Username] [varchar](100) NOT NULL,
	[Password] [varchar](255) NULL,
	[ToBeSynchronized] [bit] NOT NULL,
	[LastSynchronized] [datetime] NULL,
 CONSTRAINT [PK_HISTO_AD] PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  TABLE [Histo_IntitekUser]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_IntitekUser](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[Username] [varchar](255) NULL,
	[Status] [int] NULL,
	[EntityName] [varchar](255) NULL,
	[AgencyName] [varchar](255) NULL,
	[isOnBoarding] [bit] NOT NULL,
	[EmailOnBoarding] [varchar](255) NULL,
	[PasswordOnBoarding] [varchar](255) NULL,
	[DateLastVisit] [datetime] NULL,
	[Email] [varchar](255) NOT NULL,
	[ID_AD] [int] NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[Active] [bit] NOT NULL,
	[Type] [varchar](3) NOT NULL,
	[InactivityStart] [date] NULL,
	[InactivityEnd] [date] NULL,
	[InactivityReason] [varchar](512) NULL,
	[Departement] [varchar](255) NULL,
	[Division] [varchar](255) NULL,
	[ID_Manager] [int] NULL,
	[Pays] [varchar](255) NULL,
	[Plaque] [varchar](255) NULL,
	[EntryDate] [date] NOT NULL,
	[ExitDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Profile]    Script Date: 23/09/2021 11:24:19 ******/
CREATE TABLE [Histo_Profile](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[Name] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  TABLE [Histo_ProfileDocument]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_ProfileDocument](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[ID_Profile] [int] NULL,
	[ID_Document] [int] NULL,
	[Date] [date] NULL,
PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  TABLE [Histo_ProfileUser]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_ProfileUser](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[ID_Profile] [int] NULL,
	[ID_IntitekUser] [int] NULL,
PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  TABLE [Histo_UserDocument]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_UserDocument](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[ID_IntitekUser] [int] NULL,
	[ID_Document] [int] NULL,
	[UpdateDate] [date] NULL,
	[IsRead] [date] NULL,
	[IsTested] [date] NULL,
	[IsApproved] [date] NULL,
PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  TABLE [Histo_UserQcm]    Script Date: 23/09/2021 09:12:12 ******/
CREATE TABLE [Histo_UserQcm](
    [Mois] [varchar](7) NOT NULL,
	[ID] [int] NOT NULL,
	[ID_IntitekUser] [int] NOT NULL,
	[ID_Qcm] [int] NOT NULL,
	[DateCre] [datetime] NOT NULL,
	[DateFin] [datetime] NULL,
	[Score] [int] NULL,
	[ScoreMinimal] [int] NULL,
	[NbQuestions] [int] NULL,
	[ID_Document] [int] NULL,
	[Version] [varchar](3) NULL,
 CONSTRAINT [PK_HISTO_USERQCM] PRIMARY KEY CLUSTERED 
(
    [Mois] ASC,	
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [Histo_Document]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_USER_CRE] FOREIGN KEY([Mois], [ID_UserCre])
REFERENCES [Histo_IntitekUser] ([Mois], [ID])
GO

ALTER TABLE [Histo_DocumentLang]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_DOC_LAN] FOREIGN KEY([Mois], [ID_Document])
REFERENCES [Histo_Document] ([Mois], [ID])
GO

ALTER TABLE [Histo_DocumentVersion]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_DOCUMENT_VERSION] FOREIGN KEY([Mois], [ID_Document])
REFERENCES [Histo_Document] ([Mois], [ID])
GO

ALTER TABLE [Histo_DocumentVersion] CHECK CONSTRAINT [FK_HISTO_DOCUMENT_VERSION]
GO

ALTER TABLE [Histo_DocumentVersionLang]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_DVER_LAN] FOREIGN KEY([Mois], [ID_DocumentVersion])
REFERENCES [Histo_DocumentVersion] ([Mois], [ID])
GO

ALTER TABLE [Histo_DocumentVersionLang] CHECK CONSTRAINT [FK_HISTO_DVER_LAN]
GO

ALTER TABLE [Histo_EntityDocument]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_EntityDoc_Document] FOREIGN KEY([Mois], [ID_Document])
REFERENCES [Histo_Document] ([Mois], [ID])
GO

ALTER TABLE [Histo_EntityDocument] CHECK CONSTRAINT [FK_HISTO_EntityDoc_Document]
GO

ALTER TABLE [Histo_IntitekUser] WITH CHECK ADD  CONSTRAINT [FK_HISTO_AD_USER] FOREIGN KEY([Mois], [ID_AD])
REFERENCES [Histo_AD] ([Mois], [ID])
GO

ALTER TABLE [Histo_IntitekUser] CHECK CONSTRAINT [FK_HISTO_AD_USER]
GO

ALTER TABLE [Histo_ProfileDocument]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_Profile_Doc] FOREIGN KEY([Mois], [ID_Document])
REFERENCES [Histo_Document] ([Mois], [ID])
GO

ALTER TABLE [Histo_ProfileDocument] CHECK CONSTRAINT [FK_HISTO_Profile_Doc]
GO

ALTER TABLE [Histo_ProfileDocument]  WITH CHECK ADD FOREIGN KEY([Mois], [ID_Profile])
REFERENCES [Histo_Profile] ([Mois], [ID])
GO

ALTER TABLE [Histo_ProfileUser]  WITH CHECK ADD FOREIGN KEY([Mois], [ID_Profile])
REFERENCES [Histo_Profile] ([Mois], [ID])
GO

ALTER TABLE [Histo_ProfileUser]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_USER_PROFILE] FOREIGN KEY([Mois], [ID_IntitekUser])
REFERENCES [Histo_IntitekUser] ([Mois], [ID])
GO

ALTER TABLE [Histo_ProfileUser] CHECK CONSTRAINT [FK_HISTO_USER_PROFILE]
GO

ALTER TABLE [Histo_UserDocument]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_DOCUMENT_USER] FOREIGN KEY([Mois], [ID_Document])
REFERENCES [Histo_Document] ([Mois], [ID])
GO

ALTER TABLE [Histo_UserDocument] CHECK CONSTRAINT [FK_HISTO_DOCUMENT_USER]
GO

ALTER TABLE [Histo_UserDocument]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_USER_DOCUMENT] FOREIGN KEY([Mois], [ID_IntitekUser])
REFERENCES [Histo_IntitekUser] ([Mois], [ID])
GO

ALTER TABLE [Histo_UserDocument] CHECK CONSTRAINT [FK_HISTO_USER_DOCUMENT]
GO

ALTER TABLE [Histo_UserQcm]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_QCM_UDOC] FOREIGN KEY([Mois], [ID_Document])
REFERENCES [Histo_Document] ([Mois], [ID])
GO

ALTER TABLE [Histo_UserQcm] CHECK CONSTRAINT [FK_HISTO_QCM_UDOC]
GO

ALTER TABLE [Histo_UserQcm]  WITH CHECK ADD  CONSTRAINT [FK_HISTO_USER_QCM] FOREIGN KEY([Mois], [ID_IntitekUser])
REFERENCES [Histo_IntitekUser] ([Mois], [ID])
GO

ALTER TABLE [Histo_UserQcm] CHECK CONSTRAINT [FK_HISTO_USER_QCM]
GO

ALTER TABLE [Histo_IntitekUser]  WITH CHECK ADD  CONSTRAINT [CKC_HISTO_TYPE_INTITEKU] CHECK  (([Type]='STR' OR [Type]='MET'))
GO

ALTER TABLE [Histo_IntitekUser] CHECK CONSTRAINT [CKC_HISTO_TYPE_INTITEKU]
GO
