IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'ExamDaysAllowTogether' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'ExamDaysAllowTogether';
END

IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'Holidays10Days' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'Holidays10Days';
END

IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'Holidays15Days' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'Holidays15Days';
END

IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'Holidays20Days' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'Holidays20Days';
END

IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'Holidays25Days' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'Holidays25Days';
END

IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'StartDateUpdateHolidays' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'StartDateUpdateHolidays';
END

IF (EXISTS (SELECT 1 FROM app.settings WHERE [Key] = 'StartDateHolidays' AND Category = 0))
BEGIN
	UPDATE app.settings SET Category = 1 WHERE [Key] = 'StartDateHolidays';
END
