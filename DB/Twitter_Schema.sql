-- =====================================================
-- Twitter API - Script de Base de Datos
-- =====================================================
-- Ejecutar en SQL Server
-- =====================================================

USE TwitterDb; -- Cambiar por el nombre de tu BD
GO

-- =====================================================
-- TABLAS
-- =====================================================

-- 1. Users
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
    PRINT ' tabla dbo.Users';
END
GO

-- 2. Roles
IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles (
        RoleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(50) NOT NULL UNIQUE,
        Description NVARCHAR(255) NULL,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
    PRINT ' tabla dbo.Roles';
END
GO

-- 3. UserRoles
IF OBJECT_ID('dbo.UserRoles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles (
        UserRoleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        AssignedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE,
        FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId) ON DELETE CASCADE
    );
    PRINT ' tabla dbo.UserRoles';
END
GO

-- 4. Posts
IF OBJECT_ID('dbo.Posts', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Posts (
        PostId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        IsPublished BIT DEFAULT 1,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId) ON DELETE CASCADE
    );
    PRINT ' tabla dbo.Posts';
END
GO

-- 5. EmailTemplates
IF OBJECT_ID('dbo.EmailTemplates', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmailTemplates (
        EmailTemplateId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL UNIQUE,
        Subject NVARCHAR(255) NOT NULL,
        Body NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE()
    );
    PRINT ' tabla dbo.EmailTemplates';
END
GO

-- =====================================================
-- ÍNDICES
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserRoles_UserId')
    CREATE INDEX IX_UserRoles_UserId ON dbo.UserRoles(UserId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserRoles_RoleId')
    CREATE INDEX IX_UserRoles_RoleId ON dbo.UserRoles(RoleId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Posts_UserId')
    CREATE INDEX IX_Posts_UserId ON dbo.Posts(UserId);
GO

-- =====================================================
-- INSERTS - ROLES
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO dbo.Roles (Name, Description, IsActive)
    VALUES 
        ('Admin', 'Administrador del sistema', 1),
        ('User', 'Usuario estándar', 1);
    PRINT ' inserts de Roles';
END
GO

-- =====================================================
-- INSERTS - EMAIL TEMPLATES
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM dbo.EmailTemplates WHERE Name = 'Welcome')
BEGIN
    INSERT INTO dbo.EmailTemplates (Name, Subject, Body, CreatedAt)
    VALUES 
    (
        'Welcome',
        '¡Bienvenido a Twitter, {fullName}!',
        '<h1>¡Hola, {fullName}!</h1>
        <p>Nos alegra mucho que te hayas unido a Twitter.</p>
        <p>Ahora puedes:</p>
        <ul>
            <li>Crear y compartir tus pensamientos</li>
            <li>Seguir a otros usuarios</li>
            <li>Interactuar con publicaciones</li>
        </ul>
        <p>¡Empieza a explorar y conectar con personas!</p>
        <p>Saludos,<br>El equipo de Twitter</p>',
        GETUTCDATE()
    ),
    (
        'PasswordReset',
        'Recupera tu contraseña - Código de verificación',
        '<h1>Recuperación de contraseña</h1>
        <p>Hola {fullName},</p>
        <p>Has solicitado recuperar tu contraseña.</p>
        <p>Tu código de verificación es:</p>
        <h2 style="background: #f0f0f0; padding: 15px; text-align: center; letter-spacing: 5px;">{otp}</h2>
        <p>Este código expira en <strong>15 minutos</strong>.</p>
        <p>Si no solicitaste este cambio, puedes ignorar este email.</p>',
        GETUTCDATE()
    ),
    (
        'PasswordChanged',
        'Tu contraseña ha sido cambiada',
        '<h1>Notificación de seguridad</h1>
        <p>Hola {fullName},</p>
        <p>Tu contraseña ha sido cambiada exitosamente.</p>
        <p>Si no fuiste tú quien realizó este cambio, por favor contacta con soporte inmediatamente.</p>
        <p>Saludos,<br>El equipo de Twitter</p>',
        GETUTCDATE()
    ),
    (
        'VerifyEmail',
        'Confirma tu correo electrónico',
        '<h1>Bienvenido a Twitter</h1>
        <p>Hola {fullName},</p>
        <p>Para completar tu registro, por favor verifica tu correo electrónico.</p>
        <p>Tu código de verificación es:</p>
        <h2 style="background: #f0f0f0; padding: 15px; text-align: center; letter-spacing: 5px;">{otp}</h2>
        <p>Este código expira en <strong>24 horas</strong>.</p>',
        GETUTCDATE()
    );
    PRINT ' inserts de EmailTemplates';
END
GO

-- =====================================================
-- USUARIO DE PRUEBA (-admin@twitter.com / Admin123!)
-- =====================================================

DECLARE @AdminRoleId UNIQUEIDENTIFIER;
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();

SELECT @AdminRoleId = RoleId FROM dbo.Roles WHERE Name = 'Admin';

-- PasswordHash: Admin123! (hash bcrypt)
-- NOTA: Generar tu propio hash real para producción
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = 'admin@twitter.com')
BEGIN
    INSERT INTO dbo.Users (UserId, FullName, Email, PasswordHash, IsActive, CreatedAt)
    VALUES 
        (@AdminUserId, 'Administrator', 'admin@twitter.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5JJo7vRpKlC4S', 1, GETUTCDATE());

    INSERT INTO dbo.UserRoles (UserRoleId, UserId, RoleId, AssignedAt)
    VALUES (NEWID(), @AdminUserId, @AdminRoleId, GETUTCDATE());

    PRINT ' usuario admin de prueba';
END
GO

PRINT '==========================================';
PRINT ' Script de BD completado exitosamente';
PRINT '==========================================';
GO