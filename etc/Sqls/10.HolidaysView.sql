DECLARE @FuncModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'WOTIM');

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = 'WOTIM'))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'ABM Feriados', 'HOLID', @FuncModuleId);
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'HOLID')
DECLARE @RoleId Int = (SELECT Id from app.Roles WHERE Description = 'RRHH AB')

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RoleId, @FuncId);
END
