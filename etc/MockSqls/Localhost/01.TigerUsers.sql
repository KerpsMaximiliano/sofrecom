USE Tiger
GO

IF (NOT EXISTS (SELECT 1 FROM sys.syslogins where name = 'tiger_dev'))
BEGIN
     CREATE LOGIN tiger_dev WITH 
            PASSWORD = 'Autospawn!03',
            DEFAULT_DATABASE = Tiger, 
            CHECK_EXPIRATION = OFF, 
            CHECK_POLICY = OFF;
END

IF (NOT EXISTS (SELECT 1 FROM Tiger.sys.database_principals where name = 'tiger_dev' ))
BEGIN
     CREATE USER tiger_dev FOR LOGIN tiger_dev;
     EXEC sp_addrolemember N'db_datareader', N'tiger_dev';
END


IF (NOT EXISTS (SELECT 1 FROM sys.syslogins where name = 'tiger_admin'))
BEGIN
     CREATE LOGIN tiger_admin WITH 
            PASSWORD = 'Autospawn!03',
            DEFAULT_DATABASE = Tiger, 
            CHECK_EXPIRATION = OFF, 
            CHECK_POLICY = OFF;
END

IF (NOT EXISTS (SELECT 1 FROM Tiger.sys.database_principals where name = 'tiger_admin' ))
BEGIN
     CREATE USER tiger_admin FOR LOGIN tiger_admin;
     EXEC sp_addrolemember N'db_owner', N'tiger_admin';
END
