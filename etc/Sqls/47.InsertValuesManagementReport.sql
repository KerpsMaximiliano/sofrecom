
INSERT INTO [app].[ManagementReports]
           ([AnalyticId], [StartDate], [EndDate])
	SELECT [Id]
		  ,[StartDateContract]
		  ,[EndDateContract]      
	  FROM [app].[Analytics]