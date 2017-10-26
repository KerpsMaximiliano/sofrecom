IF NOT EXISTS ( SELECT 1 FROM sys.schemas WHERE name = 'mock_tiger')
BEGIN
	EXEC('CREATE SCHEMA mock_tiger');
END

IF NOT EXISTS ( SELECT 1 FROM sys.schemas WHERE name = 'mock_rhpro')
BEGIN
	EXEC('CREATE SCHEMA mock_rhpro');
END

