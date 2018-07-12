DECLARE @ModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'PUROR');

DECLARE @PendingFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'PEND' AND ModuleId = @ModuleId)

DECLARE @ViewFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'VIEW' AND ModuleId = @ModuleId)

DECLARE @RejectFuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'REJEC' AND ModuleId = @ModuleId)

DECLARE @ApprovalTable TABLE (Id INT, Name NVARCHAR(100), RoleCode NVARCHAR(100), FuncCode NVARCHAR(100));
INSERT @ApprovalTable (Id, Name, RoleCode, FuncCode) VALUES (1, 'Compliance', 'POAPPROVAL_COMPL', 'COMPL');
INSERT @ApprovalTable (Id, Name, RoleCode, FuncCode) VALUES (2, 'Commercial', 'POAPPROVAL_COMER', 'COMER');
INSERT @ApprovalTable (Id, Name, RoleCode, FuncCode) VALUES (3, 'Operativo', 'POAPPROVAL_OPERA', 'OPERA');
INSERT @ApprovalTable (Id, Name, RoleCode, FuncCode) VALUES (4, 'DAF', 'POAPPROVAL_DAF', 'APDAF');

DECLARE @i int = 1
DECLARE @numrows int = (SELECT COUNT(*) FROM @ApprovalTable)

WHILE (@i <= (SELECT MAX(id) FROM @ApprovalTable))
BEGIN
	DECLARE @appRoleCode NVARCHAR(100) = (SELECT RoleCode FROM @ApprovalTable WHERE id = @i)
	DECLARE @appFuncCode NVARCHAR(100) = (SELECT FuncCode FROM @ApprovalTable WHERE id = @i)
	DECLARE @appName NVARCHAR(100) = (SELECT name FROM @ApprovalTable WHERE id = @i)

	IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @appRoleCode))
	BEGIN
		INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
		VALUES (1, 'Aprobador de Orden de Compra - ' + @appName, @appRoleCode, GetUtcDate());
	END

	DECLARE @roleId INT = (SELECT Id from app.Roles WHERE Code = @appRoleCode)
	DECLARE @funcId Int = (SELECT Id FROM app.Functionalities WHERE Code = @appFuncCode)

	-- Approval Functionality
	IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @roleId AND FunctionalityId = @funcId))
	BEGIN
		INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) VALUES (@roleId, @funcId);
	END

	-- Pending Functionality
	IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @roleId AND FunctionalityId = @PendingFuncId))
	BEGIN
		INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) VALUES (@roleId, @PendingFuncId);
	END

	-- View Functionality
	IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @roleId AND FunctionalityId = @ViewFuncId))
	BEGIN
		INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) VALUES (@roleId, @ViewFuncId);
	END

	-- Reject Functionality
	IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @roleId AND FunctionalityId = @RejectFuncId))
	BEGIN
		INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) VALUES (@roleId, @RejectFuncId);
	END

	SET @i = @i + 1
END
