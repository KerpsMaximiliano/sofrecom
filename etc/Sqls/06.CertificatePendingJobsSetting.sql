IF (NOT EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'LicenseCertificatePendingDayOfMonth'))
BEGIN
	INSERT app.settings ([Key], Category, Value, Type, Created) 
	VALUES ('LicenseCertificatePendingDayOfMonth', 2, 27, 1, GetUtcDate());
END

