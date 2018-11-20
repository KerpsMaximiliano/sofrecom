DECLARE @FuncModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'ALLOC');

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = 'ALLOC'))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Solicitudes bajas', 'EMPLOYEE_END_NOTIF', @FuncModuleId);
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'EMPLOYEE_END_NOTIF')
DECLARE @RoleId Int = (SELECT Id from app.Roles WHERE Description = 'RRHH AB')

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RoleId, @FuncId);
END
