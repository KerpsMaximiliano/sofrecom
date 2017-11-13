IF (NOT EXISTS (SELECT 1 FROM app.GlobalSettings WHERE [Key] = 'AllocationManagement_Months'))
BEGIN
	INSERT app.GlobalSettings ([Key], Value, Type) VALUES ('AllocationManagement_Months', 12, 1)
END

IF (NOT EXISTS (SELECT 1 FROM app.Modules WHERE Code = 'PARMS'))
BEGIN
	INSERT INTO app.Modules  (Active, [Description], Code) 
	VALUES (1, 'Parámetros', 'PARMS');

	IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE [Description] = 'Parámetros'))
	BEGIN
		INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
		VALUES (1, 'Parámetros', 'UPDAT', @@IDENTITY);
	END
END

