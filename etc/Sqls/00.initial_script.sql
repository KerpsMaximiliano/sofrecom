SET IDENTITY_INSERT [app].[Solutions] ON 

INSERT [app].[Solutions] ([Id], [Text]) VALUES (1, N'N/A')
INSERT [app].[Solutions] ([Id], [Text]) VALUES (2, N'ARQA')
INSERT [app].[Solutions] ([Id], [Text]) VALUES (3, N'SYSM')
INSERT [app].[Solutions] ([Id], [Text]) VALUES (4, N'SCAB')
INSERT [app].[Solutions] ([Id], [Text]) VALUES (5, N'SDGD')
INSERT [app].[Solutions] ([Id], [Text]) VALUES (6, N'SMOV')
INSERT [app].[Solutions] ([Id], [Text]) VALUES (7, N'VAR')
SET IDENTITY_INSERT [app].[Solutions] OFF
SET IDENTITY_INSERT [app].[Technologies] ON 

INSERT [app].[Technologies] ([Id], [Text]) VALUES (1, N'N/A')
INSERT [app].[Technologies] ([Id], [Text]) VALUES (2, N'NET')
INSERT [app].[Technologies] ([Id], [Text]) VALUES (3, N'BSI')
INSERT [app].[Technologies] ([Id], [Text]) VALUES (4, N'ORA')
INSERT [app].[Technologies] ([Id], [Text]) VALUES (5, N'SHA')
INSERT [app].[Technologies] ([Id], [Text]) VALUES (6, N'FIL')
INSERT [app].[Technologies] ([Id], [Text]) VALUES (7, N'JAV')
SET IDENTITY_INSERT [app].[Technologies] OFF
SET IDENTITY_INSERT [app].[ClientGroups] ON 

INSERT [app].[ClientGroups] ([Id], [Text]) VALUES (1, N'Otros Clientes')
INSERT [app].[ClientGroups] ([Id], [Text]) VALUES (2, N'YPF')
INSERT [app].[ClientGroups] ([Id], [Text]) VALUES (3, N'Grupo Orange')
INSERT [app].[ClientGroups] ([Id], [Text]) VALUES (4, N'Banco Galicia')
INSERT [app].[ClientGroups] ([Id], [Text]) VALUES (5, N'Claro')
SET IDENTITY_INSERT [app].[ClientGroups] OFF
SET IDENTITY_INSERT [app].[Products] ON 

INSERT [app].[Products] ([Id], [Text]) VALUES (1, N'NAP')
INSERT [app].[Products] ([Id], [Text]) VALUES (2, N'BSI')
INSERT [app].[Products] ([Id], [Text]) VALUES (3, N'EQTC')
SET IDENTITY_INSERT [app].[Products] OFF
SET IDENTITY_INSERT [app].[Currencies] ON 

INSERT [app].[Currencies] ([Id], [Text]) VALUES (1, N'Pesos ($)')
INSERT [app].[Currencies] ([Id], [Text]) VALUES (2, N'Dolares (U$S)')
INSERT [app].[Currencies] ([Id], [Text]) VALUES (3, N'Euros (€)')
SET IDENTITY_INSERT [app].[Currencies] OFF
SET IDENTITY_INSERT [app].[ImputationNumbers] ON 

INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (1, N'012')
INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (2, N'024')
INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (3, N'091')
INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (4, N'093')
INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (5, N'096')
INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (6, N'097')
INSERT [app].[ImputationNumbers] ([Id], [Text]) VALUES (7, N'099')
SET IDENTITY_INSERT [app].[ImputationNumbers] OFF
SET IDENTITY_INSERT [app].[Analytics] ON 

INSERT [app].[Analytics] ([Id], [ActivityId], [AmountEarned], [AmountProject], [BugsAccess], [ClientExternalId], [ClientExternalName], [ClientGroupId], [ClientProjectTfs], [CommercialManager], [ContractNumber], [CreationDate], [CurrencyId], [Description], [DirectorId], [EndDateContract], [EvalProp], [ManagerId], [Name], [ProductId], [Proposal], [PurchaseOrder], [Service], [SoftwareLaw], [SolutionId], [StartDateContract], [Status], [TechnologyId], [Title], [UsersQv]) VALUES (1, 1, N'10000', N'100000', 0, N'00000000000', N'Total Gas', 1, N'FIAT | Follow Up', N'Karina Jesica Pain', N'1', CAST(N'2017-10-18 00:00:00.0000000' AS DateTime2), 1, N'Migrar el aplicativo CoVeGan de Power Builder a un online web .Net, reutilizando la lógica de negocio que se encuentra en la base de datos.', 5, CAST(N'2017-12-31 00:00:00.0000000' AS DateTime2), 0, 5, N'Servicio de reescritura de PowerBuilder a .NET', 1, N'2333-2599', 1, N'1', 1, 1, CAST(N'2017-01-01 00:00:00.0000000' AS DateTime2), 1, 1, N'210-00000', N'jlarenze@sofrecom.com.ar')
INSERT [app].[Analytics] ([Id], [ActivityId], [AmountEarned], [AmountProject], [BugsAccess], [ClientExternalId], [ClientExternalName], [ClientGroupId], [ClientProjectTfs], [CommercialManager], [ContractNumber], [CreationDate], [CurrencyId], [Description], [DirectorId], [EndDateContract], [EvalProp], [ManagerId], [Name], [ProductId], [Proposal], [PurchaseOrder], [Service], [SoftwareLaw], [SolutionId], [StartDateContract], [Status], [TechnologyId], [Title], [UsersQv]) VALUES (2, 1, N'10000', N'100000', 0, N'00000000000', N'AGEA Arte grafico editorial Argentino SA', 1, N'Deutsche_Bank|Servicio_de_Desarrollo
', N'Karina Jesica Pain', N'1', CAST(N'2017-10-18 00:00:00.0000000' AS DateTime2), 1, N'Desarrollo aplicaciones Qlikview en Clarín', 5, CAST(N'2017-12-31 00:00:00.0000000' AS DateTime2), 0, 5, N'Servicio de Consultoría QlikView 2014', 1, N'2545-2958-4091', 1, N'1', 1, 1, CAST(N'2017-01-01 00:00:00.0000000' AS DateTime2), 1, 1, N'444-F0183', N'jlarenze@sofrecom.com.ar')
INSERT [app].[Analytics] ([Id], [ActivityId], [AmountEarned], [AmountProject], [BugsAccess], [ClientExternalId], [ClientExternalName], [ClientGroupId], [ClientProjectTfs], [CommercialManager], [ContractNumber], [CreationDate], [CurrencyId], [Description], [DirectorId], [EndDateContract], [EvalProp], [ManagerId], [Name], [ProductId], [Proposal], [PurchaseOrder], [Service], [SoftwareLaw], [SolutionId], [StartDateContract], [Status], [TechnologyId], [Title], [UsersQv]) VALUES (5, 1, N'10000', N'10000', 0, N'00000000000', N'Coca Cola', 1, N'Coca Cola', N'Karina Jesica Pain', N'1', CAST(N'2017-10-18 00:00:00.0000000' AS DateTime2), 1, N'Desarrollo de aplicacion para coca cola', 5, CAST(N'2018-06-30 00:00:00.0000000' AS DateTime2), 0, 5, N'Servicios externos', 1, N'4839-49390', 1, N'1', 1, 1, CAST(N'2017-01-01 00:00:00.0000000' AS DateTime2), 1, 1, N'3921-9899', N'jlarenze@sofrecom.com.ar')
SET IDENTITY_INSERT [app].[Analytics] OFF
SET IDENTITY_INSERT [app].[Roles] ON 

INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6012, 1, N'Administrador', NULL, CAST(N'2017-09-27 16:03:35.8994325' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6013, 1, N'Gerente', NULL, CAST(N'2017-08-22 10:02:13.1585236' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6014, 1, N'Visitante', NULL, CAST(N'2017-08-22 10:04:59.3585293' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6015, 0, N'Planificación Presupuestaria', CAST(N'2017-11-16 12:54:10.2751178' AS DateTime2), CAST(N'2017-11-16 12:54:07.5519032' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6016, 1, N'Control de Gestión', NULL, CAST(N'2017-08-31 09:30:38.8616094' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6018, 1, N'Director', NULL, CAST(N'2017-09-04 12:01:12.8541720' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (6019, 1, N'DAF Dirección de Administración y Finanzas', NULL, CAST(N'2017-09-07 15:48:03.2284023' AS DateTime2))
INSERT [app].[Roles] ([Id], [Active], [Description], [EndDate], [StartDate]) VALUES (8019, 1, N'Testing', NULL, CAST(N'2017-09-29 16:34:18.4768777' AS DateTime2))
SET IDENTITY_INSERT [app].[Roles] OFF
SET IDENTITY_INSERT [app].[Groups] ON 

INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (3018, 1, N'Visitante', NULL, 6014, CAST(N'2017-08-22 10:05:11.4986881' AS DateTime2), N'jlarenze@sofrecom.com.ar', NULL)
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (3019, 1, N'Control de Gestión', NULL, 6016, CAST(N'2017-08-31 09:26:56.1326293' AS DateTime2), N'jlarenze@sofrecom.com.ar', N'CDG')
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (3020, 0, N'Planificación Presupuestaria', CAST(N'2017-09-29 15:11:35.5606547' AS DateTime2), 6015, CAST(N'2017-08-31 09:27:09.1791703' AS DateTime2), N'calidad@sofrecom.com.ar', NULL)
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (3021, 1, N'DAF', NULL, 6019, CAST(N'2017-08-31 09:29:23.8947363' AS DateTime2), N'jlarenze@sofrecom.com.ar', N'DAF')
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (3022, 1, N'Directores', NULL, 6018, CAST(N'2017-09-04 12:01:28.1518302' AS DateTime2), N'jlarenze@sofrecom.com.ar', NULL)
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (4025, 1, N'Gerentes', NULL, 6013, CAST(N'2017-09-25 15:17:22.3322422' AS DateTime2), N'calidad@sofrecom.com.ar', NULL)
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (4026, 1, N'Administradores', NULL, 6012, CAST(N'2017-09-25 15:19:10.9129392' AS DateTime2), N'jlarenze@sofrecom.com.ar', NULL)
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (4032, 1, N'Testing', NULL, 8019, CAST(N'2017-09-29 16:35:00.0232273' AS DateTime2), N'jlarenze@sofrecom.com.ar', NULL)
INSERT [app].[Groups] ([Id], [Active], [Description], [EndDate], [RoleId], [StartDate], [Email], [Code]) VALUES (4033, 1, N'Gobierno Corporativo', NULL, 6013, CAST(N'2017-10-30 15:39:12.9128317' AS DateTime2), N'frosales@sofrecom.com.ar', N'PMO')
SET IDENTITY_INSERT [app].[Groups] OFF
SET IDENTITY_INSERT [app].[Modules] ON 

INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1010, 1, N'Solicitud de Facturación', N'SOLFA')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1011, 1, N'Remitos', N'REM')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1012, 0, N'Control de Licencias', N'CCCC')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1013, 0, N'Asignación de Horas', N'DDDD')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1014, 1, N'Usuarios', N'USR')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1015, 1, N'Grupos', N'GRP')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1016, 1, N'Roles', N'ROL')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1017, 1, N'Modulos', N'MOD')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1018, 1, N'Funcionalidades', N'FUNC')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1019, 1, N'Reporte', N'REPOR')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1020, 1, N'Gestion de recursos', N'ALLOC')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1021, 1, N'Parámetros', N'PARMS')
SET IDENTITY_INSERT [app].[Modules] OFF
SET IDENTITY_INSERT [app].[Functionalities] ON 

INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1030, 1, N'Consulta Solfac', N'QUERY', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1032, 1, N'Generacion Remito', N'ALTA', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1034, 1, N'Consulta Remito', N'QUERY', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1035, 1, N'Aprobacion Remito', N'APROB', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1036, 1, N'Consulta Usuarios', N'QUERY', 1014)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1037, 1, N'Detalle Usuario', N'UPDAT', 1014)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1039, 1, N'Habilitar / Deshabilitar Usuario', N'HABIN', 1014)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1040, 1, N'Alta de grupo', N'ALTA', 1015)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1041, 1, N'Modificación de grupo', N'UPDAT', 1015)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1042, 1, N'Consulta Grupos', N'QUERY', 1015)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1044, 1, N'Habilitar / Deshabilitar Grupo', N'HABIN', 1015)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1045, 1, N'Alta de rol', N'ALTA', 1016)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1046, 1, N'Modificación de rol', N'UPDAT', 1016)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1047, 1, N'Consulta Roles', N'QUERY', 1016)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1049, 1, N'Habilitar / Deshabilitar rol', N'HABIN', 1016)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1050, 1, N'Consulta Modulos', N'QUERY', 1017)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1051, 1, N'Detalle de modulo', N'UPDAT', 1017)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1052, 1, N'Habilitar / Deshabilitar Modulo', N'HABIN', 1017)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1053, 1, N'Consulta Funcionalidades', N'QUERY', 1018)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1056, 1, N'Habilitar / Deshabilitar Funcionalidad', N'HABIN', 1018)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1057, 1, N'Enviar Remito', N'SEND', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1058, 1, N'Rechazar Remito', N'REJEC', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1059, 1, N'Guardar', N'ALTA', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1061, 1, N'Rechazar por CDG', N'REJEC', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1062, 1, N'Enviar a DAF', N'SDAF', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1063, 1, N'Facturar', N'BILL', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1065, 1, N'Enviar a CDG', N'SCDG', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1066, 1, N'Cobrar', N'CASH', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1068, 1, N'Anular Remito', N'ANNUL', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1069, 1, N'Eliminar remito', N'RMV', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1070, 1, N'Adjuntar PDF', N'ADPDF', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1071, 1, N'Adjuntar Excel', N'ADEXC', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1072, 1, N'Adjuntar archivos', N'AFILE', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1073, 1, N'Modificar datos facturacion o cobro', N'UPBIL', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1074, 1, N'Clonar Remito', N'CLONE', 1011)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1075, 1, N'Agregar grupo', N'ADGRP', 1014)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1076, 1, N'Reporte Solfac', N'REPOR', 1019)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1077, 1, N'Ver analiticas', N'QUERY', 1020)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1078, 1, N'Asignar recursos', N'ADRES', 1020)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1079, 1, N'Busqueda en diferentes fechas', N'QARDD', 1020)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1080, 1, N'Parámetros', N'UPDAT', 1021)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1082, 1, N'Dividir Hito', N'SPLIH', 1010)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1085, 1, N'Lista Centro de Costos', N'QUERY', 1020)
INSERT [app].[Functionalities] ([Id], [Active], [Description], [Code], [ModuleId]) VALUES (1086, 1, N'Alta Centro de Costos', N'ADD', 1020)

SET IDENTITY_INSERT [app].[Functionalities] OFF
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6014, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6015, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6016, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1030)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1032)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1032)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1032)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6014, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6015, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6016, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1034)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1035)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1035)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1036)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1036)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1036)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1037)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1037)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1037)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1039)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1039)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1040)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1040)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1041)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1041)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1042)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1042)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1042)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1044)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1044)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1045)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1045)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1046)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1046)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1047)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1047)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1047)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1049)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1049)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1050)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1050)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1051)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1051)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1052)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1052)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1053)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1053)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1056)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1056)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1057)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1057)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1057)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1058)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1058)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1059)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1059)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1059)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1061)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6016, 1061)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1061)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1062)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6015, 1062)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6016, 1062)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1062)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1063)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1063)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1063)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1065)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1065)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6018, 1065)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1065)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6012, 1066)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1066)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1066)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6019, 1068)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1068)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1069)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1070)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1071)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1072)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1073)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (6013, 1074)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1074)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1075)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1076)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1077)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1078)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1079)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1080)
INSERT [app].[RoleFunctionality] ([RoleId], [FunctionalityId]) VALUES (8019, 1082)
SET IDENTITY_INSERT [app].[Users] ON 

INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (1, 1, N'vchiuki@sofrecom.com.ar', N'Valeria Chiuki', NULL, CAST(N'2017-08-31 16:08:07.1485571' AS DateTime2), N'vchiuki')
INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (2, 1, N'vcuomo@sofrecom.com.ar', N'Virginia Cuomo', NULL, CAST(N'2017-09-21 14:14:58.4585412' AS DateTime2), N'vcuomo')
INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (3, 1, N'domiguel@sofrecom.com.ar', N'Diego O. Miguel', NULL, CAST(N'2017-08-04 00:00:00.0000000' AS DateTime2), N'domiguel')
INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (5, 1, N'jlarenze@sofrecom.com.ar', N'Juan Jose Larenze - Dev', NULL, CAST(N'2017-08-04 00:00:00.0000000' AS DateTime2), N'jlarenze')
INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (6, 1, N'frosales@sofrecom.com.ar', N'Franscisco Rosales', NULL, CAST(N'2017-09-25 15:21:10.3038362' AS DateTime2), N'frosales')
INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (7, 1, N'ledrughieri@sofrecom.com.ar', N'Lucas Drughieri', NULL, CAST(N'2017-08-04 00:00:00.0000000' AS DateTime2), N'ledrughieri')
INSERT [app].[Users] ([Id], [Active], [Email], [Name], [EndDate], [StartDate], [UserName]) VALUES (1008, 1, N'dmgonzalez@sofrecom.com.ar', N'Diego Gonzalez', NULL, CAST(N'2017-11-03 16:49:02.1091485' AS DateTime2), N'dmgonzalez')
SET IDENTITY_INSERT [app].[Users] OFF
INSERT [app].[UserGroup] ([UserId], [GroupId]) VALUES (2, 3018)
INSERT [app].[UserGroup] ([UserId], [GroupId]) VALUES (7, 3019)
INSERT [app].[UserGroup] ([UserId], [GroupId]) VALUES (8, 3021)
INSERT [app].[UserGroup] ([UserId], [GroupId]) VALUES (6, 4025)
INSERT [app].[UserGroup] ([UserId], [GroupId]) VALUES (5, 4032)
SET IDENTITY_INSERT [app].[DocumentTypes] ON 

INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (1, N'Factura A')
INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (2, N'Nota de Crédito A')
INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (3, N'Factura B')
INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (4, N'Nota de Crédito B')
INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (5, N'Nota de Débito')
INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (6, N'Fact Exterior')
INSERT [app].[DocumentTypes] ([Id], [Text]) VALUES (7, N'Otros')
SET IDENTITY_INSERT [app].[DocumentTypes] OFF
SET IDENTITY_INSERT [app].[GlobalSettings] ON 

INSERT [app].[GlobalSettings] ([Id], [Created], [Key], [Modified], [Type], [Value]) VALUES (1, CAST(N'2017-11-09 16:54:31.7466667' AS DateTime2), N'AllocationManagement_Months', CAST(N'2017-11-09 19:25:19.8433685' AS DateTime2), 1, N'6')
SET IDENTITY_INSERT [app].[GlobalSettings] OFF
SET IDENTITY_INSERT [app].[PaymentTerms] ON 

INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (1, N'Pago a 30 días')
INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (2, N'2% 10, pago a 30 días')
INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (3, N'Pago a 45 dias')
INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (4, N'Pago a 60 días')
INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (10, N'Pago al contado')
INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (38, N'30, 60, 90')
INSERT [app].[PaymentTerms] ([Id], [Text]) VALUES (39, N'Aplazado')
SET IDENTITY_INSERT [app].[PaymentTerms] OFF
SET IDENTITY_INSERT [app].[Provinces] ON 

INSERT [app].[Provinces] ([Id], [Text]) VALUES (1, N'Capital Federal')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (2, N'Buenos Aires')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (3, N'Catamarca')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (4, N'Chaco')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (5, N'Chubut')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (6, N'Córdoba')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (7, N'Corrientes')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (8, N'Entre Ríos')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (9, N'Formosa')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (10, N'Jujuy')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (11, N'La Pampa')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (12, N'La Rioja')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (13, N'Mendoza')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (14, N'Misiones')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (15, N'Neuquén')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (16, N'Río Negro')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (17, N'Salta')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (18, N'San Juan')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (19, N'San Luis')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (20, N'Santa Cruz')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (21, N'Santa Fe')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (22, N'Santiago del Estero')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (23, N'Tierra del Fuego, Antártida e Islas del Atlántico Sur')
INSERT [app].[Provinces] ([Id], [Text]) VALUES (24, N'Tucumán')
SET IDENTITY_INSERT [app].[Provinces] OFF
