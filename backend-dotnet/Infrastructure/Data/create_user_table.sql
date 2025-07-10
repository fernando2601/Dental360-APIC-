CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(50) NULL,
    ResetToken NVARCHAR(200) NULL,
    ResetTokenExpiry DATETIME2 NULL,
    LastLogin DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    PasswordResetToken NVARCHAR(200) NULL,
    ResetTokenExpires DATETIME2 NULL,
    RefreshToken NVARCHAR(200) NULL,
    RefreshTokenExpiryTime DATETIME2 NULL,
    PermissionId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    FOREIGN KEY (PermissionId) REFERENCES Permission(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 

-- Listar usuários por permissão
SELECT * FROM [User] WHERE PermissionId = @PermissionId;

-- Listar usuários ativos
SELECT * FROM [User] WHERE IsActive = 1; 