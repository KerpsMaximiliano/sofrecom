DECLARE @RefundWorkflowTypeCode NVARCHAR(100) = 'REFUND'

IF (NOT EXISTS (SELECT 1 FROM app.WorkflowTypes WHERE [Code] = @RefundWorkflowTypeCode))
BEGIN
	INSERT INTO app.WorkflowTypes  (Code, Name, CreatedAt, CreatedById, ModifiedAt, ModifiedById) 
	VALUES (@RefundWorkflowTypeCode, 'Reintegros', GetUtcDate(), 6, GetUtcDate(), 6);
END

DECLARE @WorkflowTypeId Int = (SELECT id FROM app.WorkflowTypes WHERE Code = @RefundWorkflowTypeCode);

IF (NOT EXISTS (SELECT 1 FROM app.Workflows WHERE Version = 1 AND WorkflowTypeId = @WorkflowTypeId))
BEGIN
	INSERT INTO app.Workflows  (
		WorkflowTypeId, 
		Version, 
		Active, 
		Description, 
		CreatedAt, CreatedById, 
		ModifiedAt, ModifiedById) 

	VALUES (@WorkflowTypeId, 1, 1, 'Aprobación Reintegros 2018', GetUtcDate(), 6, GetUtcDate(), 6);
END
