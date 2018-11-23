DECLARE @FuncModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'ALLOC');

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = 'EMPLOYEE_END_NOTIF'))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Solicitudes bajas', 'EMPLOYEE_END_NOTIF', @FuncModuleId);
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'EMPLOYEE_END_NOTIF')
DECLARE @RrhhRoleId Int = (SELECT Id from app.Roles WHERE Code = 'RRHH')
DECLARE @MangerRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Gerente')
DECLARE @DirectorRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Director')
DECLARE @PmoRoleId Int = (SELECT Id from app.Roles WHERE Description = 'PMO')

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RrhhRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RrhhRoleId, @FuncId);
END

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @MangerRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@MangerRoleId, @FuncId);
END

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @DirectorRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@DirectorRoleId, @FuncId);
END

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @PmoRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@PmoRoleId, @FuncId);
END
