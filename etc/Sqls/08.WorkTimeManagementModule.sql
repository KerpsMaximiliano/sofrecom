DECLARE @WorkTimeCode NVARCHAR(100) = 'WOTIM'

IF (NOT EXISTS (SELECT 1 FROM app.Modules WHERE Code = @WorkTimeCode))
BEGIN
	INSERT INTO app.Modules  (Active, [Description], Code) 
	VALUES (1, 'WorkTime Management', @WorkTimeCode);

	IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE [Code] = 'WORKT'))
	BEGIN
		INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
		VALUES (1, 'Cargas Horas', 'WORKT', @@IDENTITY);
	END
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'WORKT')
DECLARE @GuestRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Visitante')

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @GuestRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@GuestRoleId, @FuncId);
END