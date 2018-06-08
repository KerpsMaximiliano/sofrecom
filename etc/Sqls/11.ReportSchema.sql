IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'report') 
BEGIN
	EXEC sp_executesql N'CREATE SCHEMA report'
END

IF EXISTS (SELECT name FROM [sys].[database_principals] WHERE name = 'GapsApp')
BEGIN
	GRANT SELECT ON SCHEMA ::report TO GapsApp
END

IF EXISTS (SELECT name FROM [sys].[database_principals] WHERE name = 'sofcoDB_login')
BEGIN
	GRANT SELECT ON SCHEMA ::report TO sofcoDB_login
END

IF EXISTS (SELECT name FROM [sys].[database_principals] WHERE name = 'gapsdb_qa_login')
BEGIN
	GRANT SELECT ON SCHEMA ::report TO gapsdb_qa_login
END

IF EXISTS (SELECT name FROM [sys].[database_principals] WHERE name = 'GAPDB_login')
BEGIN
	GRANT SELECT ON SCHEMA ::report TO GAPDB_login
END
