-- Delegate Functionality

DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'CTRLI');
DECLARE @FunCode NVARCHAR(100) = 'LIC_VIEW_DELEGATE'

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = @FunCode))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Delegar Vista Licencia', @FunCode, @ModuleId);
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

-- Delegate Role

DECLARE @DelegateRoleCode NVARCHAR(100) = 'LIC_VIEW_DELEGATE'

IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @DelegateRoleCode))
BEGIN
	INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
	VALUES (1, 'Acceso Delegar Vista Licencia', @DelegateRoleCode, GetUtcDate());
END

DECLARE @DelegateRoleId Int = (SELECT Id FROM app.Roles WHERE Code = @DelegateRoleCode)

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @DelegateRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@DelegateRoleId, @FuncId);
END

-- View Role

DECLARE @ViewRoleCode NVARCHAR(100) = 'LIC_VIEW';

IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @ViewRoleCode))
BEGIN
	INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
	VALUES (1, 'Vista Licencia', @ViewRoleCode, GetUtcDate());
END

-- View Functionality

DECLARE @ViewFunCode NVARCHAR(100) = 'AUTH'

DECLARE @ViewFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = @ViewFunCode AND ModuleId = @ModuleId)

DECLARE @ViewRoleId Int = (SELECT Id FROM app.Roles WHERE Code = @ViewRoleCode)

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @ViewRoleId AND FunctionalityId = @ViewFuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) VALUES (@ViewRoleId, @ViewFuncId);
END
