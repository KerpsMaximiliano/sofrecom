UPDATE [app].[WorkflowStates] 
   SET [Active] = 'true'
 WHERE [app].[WorkflowStates].Name IN ('Aprobado/a', 'Rechazado/a', 'Pendiente aprobaci�n', 'Pendiente aprobaci�n DAF'
										, 'Pendiente aprobaci�n Compliance', 'Pendiente aprobaci�n RRHH', 'Pendiente autorizaci�n'
										, 'Pendiente aprobaci�n Gerente', 'Pendiente de procesar GAF', 'Finalizado'
										, 'Pendiente aprobaci�n Director', 'Pendiente aprobacion Director General', 'Pendiente de deposito'
										, 'Cuenta Corriente'
										)




