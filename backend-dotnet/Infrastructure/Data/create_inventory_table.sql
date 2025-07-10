CREATE TABLE Inventory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    MinQuantity INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    ClinicId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedByUserId INT NULL,
    UpdatedByUserId INT NULL,
    FOREIGN KEY (ProductId) REFERENCES Product(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ClinicId) REFERENCES ClinicInfo(Id) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedByUserId) REFERENCES [User](Id),
    FOREIGN KEY (UpdatedByUserId) REFERENCES [User](Id)
); 

-- Listar estoque de uma clínica
SELECT * FROM Inventory WHERE ClinicId = @ClinicId;

-- Listar estoque de um produto
SELECT * FROM Inventory WHERE ProductId = @ProductId;

-- Listar produtos com estoque baixo
SELECT * FROM Inventory WHERE Quantity < MinQuantity; 