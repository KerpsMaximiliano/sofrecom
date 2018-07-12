DECLARE @groupCode NVARCHAR(100) = 'PO_DAF';

DECLARE @roleCode NVARCHAR(100) = 'POAPPROVAL_DAF';

DECLARE @roleId INT = (SELECT Id from app.Roles WHERE Code = @roleCode)

IF (NOT EXISTS (SELECT 1 FROM app.Groups WHERE Code = @GroupCode))
BEGIN
	INSERT INTO app.Groups  (Active, Code, Description, Email, RoleId, StartDate) 
	VALUES (1, @groupCode, 'DAF - Orden de Compra', 'jlarenze@sofrecom.com.ar', @roleId, GetUtcDate());
END
