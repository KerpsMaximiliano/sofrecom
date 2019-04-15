
UPDATE [app].[CostDetailResourceType] 
SET [Default] = 1
WHERE [Name] IN ('Empleados', '% Ajuste', 'Recupero Vacaciones', 'Gratificaciones', 'Riesgo/Retenciones')

INSERT INTO [app].[CostDetailResourceType] VALUES ('Estudio prestaciones y subcontratados', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Hardware y Software (no incluye amortizaciones)', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Vi�ticos', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Tel�fono', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Correo', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Gastos veh�culos', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Capacitaci�n', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Seguros (cauci�n - transporte etc para DWDM)', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Compra Materiales (Nac, Importados + Importaciones)', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Otros', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Amortizaciones', 0)

