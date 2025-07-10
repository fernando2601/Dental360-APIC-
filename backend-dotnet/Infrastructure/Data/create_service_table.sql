CREATE TABLE Service (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Price DECIMAL(18,2) NOT NULL,
    Duration INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    ClinicId INT NOT NULL,
    FOREIGN KEY (ClinicId) REFERENCES ClinicInfo(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 

-- Listar serviços de uma clínica
SELECT * FROM Service WHERE ClinicId = @ClinicId;

-- Listar funcionários de um serviço
SELECT s.* FROM Staff s
INNER JOIN StaffService ss ON s.Id = ss.StaffId
WHERE ss.ServiceId = @ServiceId; 