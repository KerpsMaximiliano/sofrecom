UPDATE [app].[WorkflowStates] 
   SET [Active] = 'true'
 WHERE [app].[WorkflowStates].Name IN ('Aprobado/a', 'Rechazado/a', 'Pendiente aprobación', 'Pendiente aprobación DAF'
										, 'Pendiente aprobación Compliance', 'Pendiente aprobación RRHH', 'Pendiente autorización'
										, 'Pendiente aprobación Gerente', 'Pendiente de procesar GAF', 'Finalizado'
										, 'Pendiente aprobación Director', 'Pendiente aprobacion Director General', 'Pendiente de deposito'
										, 'Cuenta Corriente'
										)




