IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'report') 
BEGIN
	EXEC sp_executesql N'CREATE SCHEMA report'
END

GRANT SELECT ON SCHEMA ::report TO GapsApp
