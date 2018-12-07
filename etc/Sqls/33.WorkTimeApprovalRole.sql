DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'WOTIM');
DECLARE @RoleId Int = (SELECT id FROM app.Roles WHERE Code = 'WORKTIMEAPPROVAL');
DECLARE @ControlFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'WORKTIMECONTROL')

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RoleId AND FunctionalityId = @ControlFuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RoleId, @ControlFuncId);
END
