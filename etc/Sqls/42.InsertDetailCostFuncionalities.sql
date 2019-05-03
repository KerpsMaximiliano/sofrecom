
INSERT INTO [app].[Modules]
           ([Active], [Code] ,[Description])
     VALUES
           (1, N'MANRE', N'Informe de Gestión')

DECLARE @IdAdvan INT = (SELECT m.Id FROM app.Modules m WHERE	m.Code = 'MANRE') 

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'VIEW-DETAIL', N'Vista detalle', @IdAdvan)

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'EDIT-COST-DETAIL', N'Editar detalle de costos', @IdAdvan)

