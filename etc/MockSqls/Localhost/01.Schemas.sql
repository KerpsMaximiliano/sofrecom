IF NOT EXISTS ( SELECT 1 FROM sys.schemas WHERE name = 'tiger')
BEGIN
	EXEC('CREATE SCHEMA tiger');
END

IF NOT EXISTS ( SELECT 1 FROM sys.schemas WHERE name = 'rhpro')
BEGIN
	EXEC('CREATE SCHEMA rhpro');
END
