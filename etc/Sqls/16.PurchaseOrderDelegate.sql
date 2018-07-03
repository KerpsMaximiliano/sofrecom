DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'PUROR');
DECLARE @FunCode NVARCHAR(100) = 'PODE'

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = @FunCode))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Delegar Aprobación Orden de Compra', @FunCode, @ModuleId);
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

DECLARE @RoleHapDelegateCode NVARCHAR(100) = 'PURCHASEORDERAPPROVAL'
IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @RoleHapDelegateCode))
BEGIN
	INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
	VALUES (1, 'Aprobador de Orden de Compra', @RoleHapDelegateCode, GetUtcDate());
END

