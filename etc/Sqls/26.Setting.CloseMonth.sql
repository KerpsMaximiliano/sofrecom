DECLARE @Code NVARCHAR(100) = 'CloseMonth'

IF (NOT EXISTS (SELECT 1 FROM app.Settings WHERE [Key] = @Code))
BEGIN
	INSERT INTO app.Settings  ([Key], [Type], [Value], Created, Category) 
	VALUES (@Code, 2, 22, GetUtcDate(), 0);
END
