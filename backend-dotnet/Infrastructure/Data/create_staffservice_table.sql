CREATE TABLE StaffService (
    StaffId INT NOT NULL,
    ServiceId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    PRIMARY KEY (StaffId, ServiceId),
    FOREIGN KEY (StaffId) REFERENCES Staff(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ServiceId) REFERENCES Service(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 