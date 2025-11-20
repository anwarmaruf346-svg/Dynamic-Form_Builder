-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DynamicFormBuilderDB')
BEGIN
    CREATE DATABASE DynamicFormBuilderDB;
END
GO

USE DynamicFormBuilderDB;
GO

-- Create Forms Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Forms')
BEGIN
    CREATE TABLE Forms
    (
        FormId INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(100) NOT NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME NULL
    );
END
GO

-- Create DropdownFields Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DropdownFields')
BEGIN
    CREATE TABLE DropdownFields
    (
        FieldId INT PRIMARY KEY IDENTITY(1,1),
        FormId INT NOT NULL,
        Label NVARCHAR(255) NOT NULL,
        IsRequired BIT NOT NULL DEFAULT 0,
        SelectedValue NVARCHAR(MAX),
        OptionList NVARCHAR(MAX),
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_DropdownFields_Forms FOREIGN KEY (FormId) REFERENCES Forms(FormId) ON DELETE CASCADE
    );
END
GO

-- Create Index
CREATE INDEX IX_DropdownFields_FormId ON DropdownFields(FormId);
GO

-- ============================================
-- STORED PROCEDURES
-- ============================================

-- SP: Create Form
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreateForm')
    DROP PROCEDURE sp_CreateForm;
GO

CREATE PROCEDURE sp_CreateForm
    @Title NVARCHAR(100),
    @FormId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO Forms (Title, CreatedDate)
        VALUES (@Title, GETUTCDATE());
        
        SET @FormId = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: Create Dropdown Field
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreateDropdownField')
    DROP PROCEDURE sp_CreateDropdownField;
GO

CREATE PROCEDURE sp_CreateDropdownField
    @FormId INT,
    @Label NVARCHAR(255),
    @IsRequired BIT,
    @SelectedValue NVARCHAR(MAX),
    @OptionList NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO DropdownFields (FormId, Label, IsRequired, SelectedValue, OptionList, CreatedDate)
        VALUES (@FormId, @Label, @IsRequired, @SelectedValue, @OptionList, GETUTCDATE());
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: Get All Forms
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllForms')
    DROP PROCEDURE sp_GetAllForms;
GO

CREATE PROCEDURE sp_GetAllForms
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT FormId, Title, CreatedDate
        FROM Forms
        ORDER BY CreatedDate DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: Get Form By ID
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetFormById')
    DROP PROCEDURE sp_GetFormById;
GO

CREATE PROCEDURE sp_GetFormById
    @FormId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT FormId, Title, CreatedDate
        FROM Forms
        WHERE FormId = @FormId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: Get Form Fields
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetFormFields')
    DROP PROCEDURE sp_GetFormFields;
GO

CREATE PROCEDURE sp_GetFormFields
    @FormId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT FieldId, FormId, Label, IsRequired, SelectedValue, OptionList
        FROM DropdownFields
        WHERE FormId = @FormId
        ORDER BY FieldId ASC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: Get Forms Count (for pagination)
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetFormsCount')
    DROP PROCEDURE sp_GetFormsCount;
GO

CREATE PROCEDURE sp_GetFormsCount
    @SearchValue NVARCHAR(255) = ''
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT COUNT(*)
        FROM Forms
        WHERE (@SearchValue = '' OR Title LIKE '%' + @SearchValue + '%');
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: Get Forms Paged
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetFormsPaged')
    DROP PROCEDURE sp_GetFormsPaged;
GO

CREATE PROCEDURE sp_GetFormsPaged
    @Start INT = 0,
    @Length INT = 10,
    @SearchValue NVARCHAR(255) = ''
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT FormId, Title, CreatedDate
        FROM Forms
        WHERE (@SearchValue = '' OR Title LIKE '%' + @SearchValue + '%')
        ORDER BY CreatedDate DESC
        OFFSET @Start ROWS
        FETCH NEXT @Length ROWS ONLY;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- Sample Data (Optional)
-- INSERT INTO Forms (Title, CreatedDate) VALUES ('Sample Form 1', GETUTCDATE());
-- INSERT INTO Forms (Title, CreatedDate) VALUES ('Sample Form 2', GETUTCDATE());

PRINT 'Database schema and stored procedures created successfully!';
