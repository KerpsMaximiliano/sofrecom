DECLARE @SolfacModuleId Int = (SELECT id FROM app.Modules WHERE Code = 'SOLFA');

IF (NOT EXISTS (SELECT 1 FROM app.Functionalities WHERE Code = 'SOLDE'))
BEGIN
	INSERT INTO app.Functionalities  (Active, [Description], Code, ModuleId) 
	VALUES (1, 'Delegar Solfac', 'SOLDE', @SolfacModuleId);
END

DECLARE @FuncId Int = (SELECT Id FROM app.Functionalities WHERE Code = 'SOLDE')
DECLARE @RoleId Int = (SELECT Id from app.Roles WHERE Description = 'Gerente')

IF (NOT EXISTS (SELECT 1 FROM app.RoleFunctionality WHERE RoleId = @RoleId AND FunctionalityId = @FuncId))
BEGIN
	INSERT INTO app.RoleFunctionality  (RoleId, FunctionalityId) 
	VALUES (@RoleId, @FuncId);
END

DECLARE @RoleSolfacDelegateCode NVARCHAR(100) = 'SOLFACGENERATOR'
IF (NOT EXISTS (SELECT 1 FROM app.Roles WHERE Code = @RoleSolfacDelegateCode))
BEGIN
	INSERT INTO app.Roles  (Active, Description, Code, StartDate) 
	VALUES (1, 'Generador de Solfac', @RoleSolfacDelegateCode, GetUtcDate());
END