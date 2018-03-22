IF (NOT EXISTS (SELECT 1 FROM sys.syslogins where name = 'GapsApp'))
BEGIN
     CREATE LOGIN GapsApp WITH 
            PASSWORD = 'Autospawn!01',
            DEFAULT_DATABASE = Sofco, 
            CHECK_EXPIRATION = OFF, 
            CHECK_POLICY = OFF;
END

IF (NOT EXISTS (SELECT 1 FROM sofco.sys.database_principals where name = 'GapsApp'))
BEGIN
     CREATE USER GapsApp FOR LOGIN GapsApp WITH DEFAULT_SCHEMA = app;
END

GRANT SELECT ON SCHEMA ::app TO GapsApp
GRANT INSERT ON SCHEMA ::app TO GapsApp
GRANT UPDATE ON SCHEMA ::app TO GapsApp
GRANT DELETE ON SCHEMA ::app TO GapsApp
