
UPDATE [app].[CostDetailResourceType] 
SET [Default] = 1
WHERE [Name] IN ('Empleados', '% Ajuste', 'Recupero Vacaciones', 'Gratificaciones', 'Riesgo/Retenciones')

INSERT INTO [app].[CostDetailResourceType] VALUES ('Estudio prestaciones y subcontratados', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Hardware y Software (no incluye amortizaciones)', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Viáticos', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Teléfono', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Correo', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Gastos vehículos', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Capacitación', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Seguros (caución - transporte etc para DWDM)', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Compra Materiales (Nac, Importados + Importaciones)', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Otros', 0)
INSERT INTO [app].[CostDetailResourceType] VALUES ('Amortizaciones', 0)

