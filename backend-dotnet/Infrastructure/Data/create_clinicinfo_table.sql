CREATE TABLE ClinicInfo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 

-- Listar todas as clínicas
SELECT * FROM ClinicInfo;

-- Listar pacientes de uma clínica
SELECT p.* FROM Patient p
INNER JOIN PatientClinic pc ON p.Id = pc.PatientId
WHERE pc.ClinicId = @ClinicId;

-- Listar funcionários de uma clínica
SELECT s.* FROM Staff s
INNER JOIN StaffClinic sc ON s.Id = sc.StaffId
WHERE sc.ClinicId = @ClinicId; 