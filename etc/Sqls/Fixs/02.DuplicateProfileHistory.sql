DELETE
FROM app.employeeProfileHistory
WHERE 
	ModifiedFields = '["Cuil"]'
AND 
	REPLACE(JSON_VALUE(EmployeePreviousData, '$.cuil'), '.00', '') = REPLACE(JSON_VALUE(EmployeeData, '$.cuil'), '.0', '')
