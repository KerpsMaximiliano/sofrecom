DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'WOTIM');
DECLARE @RoleId Int = (SELECT id FROM app.Roles WHERE Code = 'WORKTIMEAPPROVAL');
DECLARE @WorkTimeControlFunCode NVARCHAR(100) = 'WORKTIMECONTROL'
DECLARE @WorkTimeQueryFunCode NVARCHAR(100) = 'QUERY'

DECLARE @ControlFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = @WorkTimeControlFunCode)
DECLARE @QueryFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = @WorkTimeQueryFunCode AND ModuleId = @ModuleId)

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RoleId AND FunctionalityId = @ControlFuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RoleId, @ControlFuncId);
END

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RoleId AND FunctionalityId = @QueryFuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RoleId, @QueryFuncId);
END
