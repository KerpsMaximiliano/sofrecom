CREATE OR ALTER VIEW qv.AnalyticsView AS
SELECT 
	a.Id,
	a.Title,
	CASE a.Status
		WHEN 1 THEN 'Abierta'
		WHEN 2 THEN 'Cerrada'
		WHEN 3 THEN 'Cerrada para costos'
	END Status,
	a.ClientExternalName, 
	c.Name ManagerName, 
	d.Name CommercialName, 
	a.StartDateContract, 
	a.EndDateContract,
	b.text ClientGroup, 
	e.Text ServiceName, 
	f.Text SolutionName, 
	g.text TechnologyName
FROM app.analytics a
LEFT JOIN app.ClientGroups b ON b.Id = a.clientgroupid
LEFT JOIN app.Users c ON c.id = a.managerid
LEFT JOIN app.Users d ON d.id = a.CommercialManagerId
LEFT JOIN app.ServiceTypes e ON e.Id = a.ServiceTypeId
LEFT JOIN app.Solutions f ON f.Id = a.SolutionId
LEFT JOIN app.Technologies g ON g.id = a.TechnologyId
