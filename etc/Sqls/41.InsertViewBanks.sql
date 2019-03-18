CREATE VIEW [report].[BanksView] AS

	SELECT DISTINCT(e.Bank) as 'Name'
		 ,CAST(ROW_NUMBER() OVER (ORDER BY e.Bank) AS INT) AS 'Id'
	FROM app.Employees e
	WHERE e.Bank IS NOT    NULL
		AND     LTRIM(RTRIM(e.Bank)) != ''
	GROUP BY e.bank


GO
