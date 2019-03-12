
DECLARE @IdAdvan INT = (SELECT m.Id FROM app.Modules m WHERE	m.Code = 'ADVAN') 

INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'PAYMENT-PENDING-VIEW', N'Listado de pagos Pendientes de deposito', @IdAdvan)
		   
INSERT INTO [app].[Functionalities]
           ([Active], [Code], [Description], [ModuleId])
     VALUES
           (1, N'PP-MASIVE-APPROVAL', N'Aprobacion Masiva de pagos pendientes de deposito', @IdAdvan)

