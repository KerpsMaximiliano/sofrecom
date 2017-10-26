IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'tiger' 
				AND  TABLE_NAME = 'Recursos'))
BEGIN
	CREATE TABLE tiger.[Recursos] (
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
				 WHERE TABLE_SCHEMA = 'rhpro' 
				AND  TABLE_NAME = 'empleado'))
BEGIN
	CREATE TABLE rhpro.[empleado] (
		[empleg] INT NOT NULL,
		[ternro] INT NOT NULL
	);
END
GO

IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'rhpro' 
				AND  TABLE_NAME = 'emp_lic'))
BEGIN
	CREATE TABLE rhpro.[emp_lic] (
		[empleado] INT NOT NULL,
		[elfechadesde] [DATETIME],
		[elfechahasta] [DATETIME],
		tdnro INT NOT NULL
	);
END
GO

IF (NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
				 WHERE TABLE_SCHEMA = 'rhpro' 
				AND  TABLE_NAME = 'tipdia'))
BEGIN
	CREATE TABLE rhpro.[tipdia] (
		[tdnro] INT NOT NULL,
		[tddesc] NVARCHAR(255)
	);
END
GO