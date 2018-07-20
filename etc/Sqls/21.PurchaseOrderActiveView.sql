DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'PUROR');
DECLARE @FunCode NVARCHAR(100) = 'POACTIVEVIEW'

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = @FunCode))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Vista Orden de Compra Activas', @FunCode, @ModuleId);
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = @FunCode)
DECLARE @MangerRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Gerente')
DECLARE @DirectorRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Director')

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

DECLARE @ViewRoleCode NVARCHAR(100) = 'PO_VIEW_ACTIVE'

IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @ViewRoleCode))
BEGIN
	INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
	VALUES (1, 'Acceso Ver Orden de Compra Activas', @ViewRoleCode, GetUtcDate());
END

DECLARE @ViewRoleId Int = (SELECT Id FROM app.Roles WHERE Code = @ViewRoleCode)

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @ViewRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@ViewRoleId, @FuncId);
END

DECLARE @ViewIconFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'VIEW' AND ModuleId = @ModuleId)

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @ViewRoleId AND FunctionalityId = @ViewIconFuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) VALUES (@ViewRoleId, @ViewIconFuncId);
END
