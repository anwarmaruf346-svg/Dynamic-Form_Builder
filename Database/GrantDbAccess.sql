-- Grant DB access to the Windows account that runs the application
-- Run this script in SQL Server Management Studio (connect as an admin/sysadmin)

-- If the database does not exist, first run DynamicFormBuilderDB.sql
USE master;
GO

-- Create a login for the Windows account (replace domain\user if different)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'DESKTOP-QCGPDH2\MNT USER')
BEGIN
    CREATE LOGIN [DESKTOP-QCGPDH2\MNT USER] FROM WINDOWS;
END
GO

-- Create database user and assign db_owner on DynamicFormBuilderDB
USE [DynamicFormBuilderDB];
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'DESKTOP-QCGPDH2\MNT USER')
BEGIN
    CREATE USER [DESKTOP-QCGPDH2\MNT USER] FOR LOGIN [DESKTOP-QCGPDH2\MNT USER];
END
GO

-- Grant db_owner role (for development only). For production, grant minimum required permissions.
EXEC sp_addrolemember N'db_owner', N'DESKTOP-QCGPDH2\MNT USER';
GO

PRINT 'Login and database user created and granted db_owner on DynamicFormBuilderDB.';
