CREATE TABLE PatientClinic (
    PatientId INT NOT NULL,
    ClinicId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    PRIMARY KEY (PatientId, ClinicId),
    FOREIGN KEY (PatientId) REFERENCES Patient(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ClinicId) REFERENCES ClinicInfo(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 