-- 2021-09-29
-- WELCOME V3
-- Adding column Illustration to QuestionLang table

ALTER TABLE [QuestionLang]
	ADD [Illustration] [varbinary](max) NULL;

-- End