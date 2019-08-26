UPDATE [app].[CostDetailTypes] SET [Default] = 0
 WHERE Name = 'Gratificaciones'
 
INSERT INTO [app].[CostDetailTypes] VALUES ('Horas Extras', 1)