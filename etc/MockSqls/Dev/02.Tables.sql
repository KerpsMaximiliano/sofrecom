IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'mock_tiger' 
				AND  TABLE_NAME = 'Recursos'))
BEGIN
	CREATE TABLE mock_tiger.[Recursos] (
		[legaj] INT NOT NULL,
		[nomb] NVARCHAR(255),
		[fenac] [DATETIME],
		[feiem] [DATETIME],
		[febaj] [DATETIME],
		[dtitu] NVARCHAR(255),
		[didio] NVARCHAR(255),
		[dgrup] NVARCHAR(255)
	);
END
GO

IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'mock_rhpro' 
				AND  TABLE_NAME = 'empleado'))
BEGIN
	CREATE TABLE mock_rhpro.[empleado] (
		[empleg] INT NOT NULL,
		[ternro] INT NOT NULL
	);
END
GO

IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'mock_rhpro' 
				AND  TABLE_NAME = 'emp_lic'))
BEGIN
	CREATE TABLE mock_rhpro.[emp_lic] (
		[empleado] INT NOT NULL,
		[elfechadesde] [DATETIME],
		[elfechahasta] [DATETIME],
		tdnro INT NOT NULL
	);
END
GO

IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'mock_rhpro' 
				AND  TABLE_NAME = 'tipdia'))
BEGIN
	CREATE TABLE mock_rhpro.[tipdia] (
		[tdnro] INT NOT NULL,
		[tddesc] NVARCHAR(255)
	);
END
GO