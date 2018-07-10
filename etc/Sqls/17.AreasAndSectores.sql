SET IDENTITY_INSERT [app].[Modules] ON 

INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1027, 1, N'Areas', N'AREAS')
INSERT [app].[Modules] ([Id], [Active], [Description], [Code]) VALUES (1028, 1, N'Sectores', N'SECTO')

SET IDENTITY_INSERT [app].[Modules] OFF


SET IDENTITY_INSERT [app].[Functionalities] ON 

INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1143, 1, N'HABIN', N'Habilitar / Deshabiltiar Areas', 1027)
INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1144, 1, N'ALTA', N'Listado Areas', 1027)
INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1145, 1, N'EDIT', N'Alta Categorias', 1027)
INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1146, 1, N'QUERY', N'Editar Categorias', 1027)

INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1147, 1, N'HABIN', N'Habilitar / Deshabiltiar Sectores', 1028)
INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1148, 1, N'ALTA', N'Listado Sectores', 1028)
INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1149, 1, N'EDIT', N'Alta Sectores', 1028)
INSERT [app].[Functionalities] ([Id], [Active], [Code], [Description], [ModuleId]) VALUES (1150, 1, N'QUERY', N'Editar Sectores', 1028)

SET IDENTITY_INSERT [app].[Functionalities] OFF
