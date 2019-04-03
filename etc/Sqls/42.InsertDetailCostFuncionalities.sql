DECLARE @IdAdvan INT = (SELECT m.Id FROM app.Modules m WHERE	m.Code = 'MANRE') 

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'EDIT-COST-DETAIL', N'Editar detalle de costos', @IdAdvan)

