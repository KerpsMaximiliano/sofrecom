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

DECLARE @RoleHapDelegateCode NVARCHAR(100) = 'PURCHASEORDERACTIVE'
DECLARE @DelegateRoleId Int = (SELECT Id FROM app.Roles WHERE Code = @RoleHapDelegateCode)

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @DelegateRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@DelegateRoleId, @FuncId);
END
