IF (NOT EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'ValidatePeriodCloseMonth'))
BEGIN
	INSERT app.settings ([Key], Category, Value, Type, Created) 
	VALUES ('ValidatePeriodCloseMonth', 0, 'false', 2, GetUtcDate());
END

