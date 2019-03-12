

DECLARE @IdWorkflow int = (SELECT m.Id from app.Modules m	WHERE m.Code = 'WORKF')



INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'HABIN', N'Habilitar / Deshabilitar Workflow', @IdWorkflow)

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'ALTA', N'Alta Workflow', @IdWorkflow)

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'UPDAT', N'Modificacion de Workflow', @IdWorkflow)

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'QUERY', N'Consulta de Workflow', @IdWorkflow)
GO


