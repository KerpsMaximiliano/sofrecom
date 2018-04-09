DECLARE @AllocModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'ALLOC');

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = 'TAPDE'))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Delegar aprobación horas', 'TAPDE', @AllocModuleId);
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'TAPDE')
DECLARE @MangerRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Gerente')
DECLARE @DirectorRoleId Int = (SELECT Id from app.Roles WHERE Description = 'Gerente')

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

DECLARE @RoleHapDelegateCode NVARCHAR(100) = 'WORKTIMEAPPROVAL'
IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @RoleHapDelegateCode))
BEGIN
	INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
	VALUES (1, 'Aprobador de horas', @RoleHapDelegateCode, GetUtcDate());
END