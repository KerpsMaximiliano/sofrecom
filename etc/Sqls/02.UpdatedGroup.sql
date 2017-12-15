IF (NOT EXISTS (SELECT 1 FROM app.Groups WHERE Code = 'DAF'))
BEGIN
	UPDATE app.Groups SET Code = 'DAF' WHERE Description = 'DAF'
END

IF (NOT EXISTS (SELECT 1 FROM app.Groups WHERE Code = 'CDG'))
BEGIN
	UPDATE app.Groups SET Code = 'CDG' WHERE Description = 'Control de Gestión'
END

IF (NOT EXISTS (SELECT 1 FROM app.Groups WHERE Code = 'PMO'))
BEGIN
	UPDATE app.Groups SET Code = 'PMO' WHERE Description = 'Gobierno Corporativo'
END
