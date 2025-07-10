CREATE TABLE Permission (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 

-- Listar todas as permissões
SELECT * FROM Permission;

-- Listar usuários de uma permissão
SELECT u.* FROM [User] u WHERE u.PermissionId = @PermissionId; 