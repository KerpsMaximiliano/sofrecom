IF (NOT EXISTS (SELECT 1 FROM app.Modules WHERE Code = 'REPOR'))
BEGIN
	INSERT INTO app.Modules  (Active, [Description], Code) 
	VALUES (1, 'Reporte', 'REPOR');

	IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE [Description] = 'Reporte Solfac'))
	BEGIN
		INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
		VALUES (1, 'Reporte Solfac', 'REPOR', @@IDENTITY);
	END
END
