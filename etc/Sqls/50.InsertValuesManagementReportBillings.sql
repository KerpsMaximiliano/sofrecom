DECLARE @MesesAnalitica TABLE	
(
	ManagementReportId int
	, MonthYear datetime2
)

DECLARE @Reportes TABLE	
(
	Id int IDENTITY(1,1) NOT NULL
	, AnalyticId int
	, StartDate datetime2
	, EndDate datetime2
)

INSERT INTO @Reportes
	SELECT Id, DATEADD(DAY, - (DATEPART(day, StartDate) -1), StartDate), DATEADD(DAY, - (DATEPART(day, EndDate) -1), EndDate) FROM app.ManagementReports

DECLARE @beginDate date 
DECLARE @EndDate date
DECLARE @Cont int = 1
DECLARE @Max int
set @Max = (select max(id) from @Reportes)
DECLARE @ManagementReportId int 

WHILE @Cont < @Max
BEGIN
	SET @beginDate = (select StartDate from @Reportes where id = @Cont)
	SET @endDate = (select EndDate from @Reportes where id = @Cont)
	SET @ManagementReportId = (select AnalyticId from @Reportes where id = @Cont)

	While @beginDate <= @endDate 
	Begin 
		Insert Into @MesesAnalitica VALUES (
										@ManagementReportId,
										@beginDate)

		Set @beginDate = DateAdd(Month, 1, @beginDate) 
	End
	
	SET @Cont = @Cont + 1
END


INSERT INTO app.managementreportbillings
	SELECT ManagementReportId, MonthYear, 0, 0 FROM @MesesAnalitica


