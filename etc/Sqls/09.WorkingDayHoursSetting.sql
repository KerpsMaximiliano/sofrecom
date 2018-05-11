IF (NOT EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'WorkingHoursPerDaysMax'))
BEGIN
	INSERT app.settings ([Key], Category, Value, Type, Created) 
	VALUES ('WorkingHoursPerDaysMax', 1, 8, 1, GetUtcDate());
END

