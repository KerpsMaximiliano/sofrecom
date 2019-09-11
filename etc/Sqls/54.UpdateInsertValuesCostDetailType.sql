UPDATE [app].[CostDetailTypes] 
	SET [Default] = 0,
	    [BelongEmployee] = 1
 WHERE Name = 'Gratificaciones'
 
UPDATE [app].[CostDetailTypes]
   SET [BelongEmployee] = 1
 WHERE [Name] ='Recupero Vacaciones'

UPDATE [app].[CostDetailTypes]
   SET [BelongEmployee] = 1
 WHERE [Name] ='Riesgo/Retenciones'
 
INSERT INTO [app].[CostDetailTypes] VALUES ('Horas Extras', 1, 1)