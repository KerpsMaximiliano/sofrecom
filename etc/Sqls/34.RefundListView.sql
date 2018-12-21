DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'ADVAN');

DECLARE @RefundListCode NVARCHAR(100) = 'REFUND-LIST-VIEW'

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE [Code] = @RefundListCode))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Listado Reintegros', @RefundListCode, @ModuleId);
END

DECLARE @DafRoleCode NVARCHAR(100) = 'DAF'
IF (NOT EXISTS (SELECT Id from app.Roles WHERE Code = @DafRoleCode))
BEGIN
	UPDATE app.Roles SET Code = @DafRoleCode WHERE Description LIKE 'DAF Dir%';
END

DECLARE @GafRoleCode NVARCHAR(100) = 'GAF'
IF (NOT EXISTS (SELECT Id from app.Roles WHERE Code = @GafRoleCode))
BEGIN
	UPDATE app.Roles SET Code = @GafRoleCode WHERE Description LIKE 'GAF%';
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = @RefundListCode)

DECLARE @ComplianceRoleId Int = (select id from app.roles where DESCRIPTION = 'Compliance');
DECLARE @MangerRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Gerente');
DECLARE @DirectorRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Director');
DECLARE @DafRoleId Int = (SELECT Id from app.Roles WHERE Code = @DafRoleCode);
DECLARE @GafRoleId Int = (SELECT Id from app.Roles WHERE Code = @GafRoleCode);

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @ComplianceRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@ComplianceRoleId, @FuncId);
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

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @DafRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@DafRoleId, @FuncId);
END

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @GafRoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@GafRoleId, @FuncId);
END
