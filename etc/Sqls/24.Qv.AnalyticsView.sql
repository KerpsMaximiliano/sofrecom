CREATE OR ALTER VIEW qv.AnalyticsView AS
SELECT 
	a.Id,
	replace(a.Title, '-', ' ') Titulo,
	CASE a.Status
		WHEN 1 THEN 'Abierta'
		WHEN 2 THEN 'Cerrada'
		WHEN 3 THEN 'Cerrada para costos'
	END Estado,
	a.StartDateContract Inicio_OC, 
	a.EndDateContract Fin_OC,
	a.ClientExternalName Cliente, 
	c.Name Gerente_Proyecto, 
	d.Name Responsable, 
	b.text Grupo_Cliente, 
	e.Text Servicio, 
	f.Text Solucion, 
	g.text Tecnologia 
FROM app.analytics a
LEFT JOIN app.ClientGroups b ON b.Id = a.clientgroupid
LEFT JOIN app.Users c ON c.id = a.managerid
LEFT JOIN app.Users d ON d.id = a.CommercialManagerId
LEFT JOIN app.ServiceTypes e ON e.Id = a.ServiceTypeId
LEFT JOIN app.Solutions f ON f.Id = a.SolutionId
LEFT JOIN app.Technologies g ON g.id = a.TechnologyId
