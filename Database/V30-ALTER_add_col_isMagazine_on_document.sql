-- 2021-10-08
-- WELCOME V3
-- Adding column isMagazine to Document table

ALTER TABLE [Document]
	ADD [isMagazine] bit NOT NULL DEFAULT 0;

-- End