# 🗄️ Scripts de Criação do Banco de Dados - DentalSpa

## 📋 INFORMAÇÕES DO BANCO

### **PostgreSQL (Principal):**
- **Nome do Banco:** `dentalspa`
- **Usuário:** `postgres`
- **Senha:** `postgres`
- **Host:** `localhost`
- **Porta:** `5432`

### **SQL Server (Secundário):**
- **Nome do Banco:** `DentalSpa_SqlServer`
- **Autenticação:** Windows (Trusted Connection)
- **Host:** `localhost`
- **Porta:** `1433`

---

## 🐘 SCRIPTS POSTGRESQL

### **1. Criar o Banco de Dados:**
```sql
-- Conecte no PostgreSQL como superuser e execute:
CREATE DATABASE dentalspa;

-- Conceder permissões
GRANT ALL PRIVILEGES ON DATABASE dentalspa TO postgres;
```

### **2. Conectar no banco e criar as tabelas:**
```sql
-- Conecte no banco 'dentalspa' e execute as tabelas abaixo:
\c dentalspa;

-- Tabela Users
CREATE TABLE "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(50) NOT NULL UNIQUE,
    "Password" VARCHAR(255) NOT NULL,
    "FullName" VARCHAR(200) NOT NULL,
    "Role" VARCHAR(50) NOT NULL DEFAULT 'staff',
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Phone" VARCHAR(20),
    "ResetToken" VARCHAR(255),
    "ResetTokenExpiry" TIMESTAMP,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "LastLogin" TIMESTAMP
);

-- Tabela Clients
CREATE TABLE "Clients" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Phone" VARCHAR(20) NOT NULL,
    "Address" TEXT,
    "BirthDate" DATE,
    "Gender" VARCHAR(20),
    "Profession" VARCHAR(100),
    "MaritalStatus" VARCHAR(50),
    "EmergencyContact" VARCHAR(200),
    "EmergencyPhone" VARCHAR(20),
    "Observations" TEXT,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela Services
CREATE TABLE "Services" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Category" VARCHAR(100) NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "Duration" INTEGER DEFAULT 60,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela Staff
CREATE TABLE "Staff" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "Phone" VARCHAR(20) NOT NULL,
    "Position" VARCHAR(100) NOT NULL,
    "Specialization" VARCHAR(200),
    "Schedule" TEXT,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela Appointments
CREATE TABLE "Appointments" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "ServiceId" INTEGER NOT NULL,
    "StaffId" INTEGER NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "ScheduledDate" TIMESTAMP NOT NULL,
    "Duration" INTEGER DEFAULT 60,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'scheduled',
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("ServiceId") REFERENCES "Services"("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("StaffId") REFERENCES "Staff"("Id") ON DELETE RESTRICT
);

-- Tabela Inventory
CREATE TABLE "Inventories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Category" VARCHAR(100) NOT NULL,
    "Quantity" INTEGER NOT NULL DEFAULT 0,
    "MinimumStock" INTEGER DEFAULT 10,
    "Price" DECIMAL(18,2),
    "Supplier" VARCHAR(200),
    "ExpirationDate" DATE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela FinancialTransactions
CREATE TABLE "FinancialTransactions" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" INTEGER,
    "Type" VARCHAR(50) NOT NULL,
    "Amount" DECIMAL(18,2) NOT NULL,
    "Description" TEXT,
    "PaymentMethod" VARCHAR(50) NOT NULL,
    "TransactionDate" TIMESTAMP NOT NULL,
    "Status" VARCHAR(50) DEFAULT 'completed',
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT
);

-- Tabela Packages
CREATE TABLE "Packages" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Price" DECIMAL(18,2) NOT NULL,
    "DiscountPrice" DECIMAL(18,2),
    "Services" TEXT,
    "Duration" INTEGER,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela ClientPackages
CREATE TABLE "ClientPackages" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "PackageId" INTEGER NOT NULL,
    "PurchaseDate" TIMESTAMP NOT NULL,
    "ExpirationDate" TIMESTAMP,
    "Status" VARCHAR(50) DEFAULT 'active',
    "RemainingServices" INTEGER,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("PackageId") REFERENCES "Packages"("Id") ON DELETE RESTRICT
);

-- Tabela BeforeAfters
CREATE TABLE "BeforeAfters" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "ServiceId" INTEGER NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "BeforeImage" TEXT,
    "AfterImage" TEXT,
    "TreatmentDate" TIMESTAMP,
    "IsPublic" BOOLEAN DEFAULT false,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("ServiceId") REFERENCES "Services"("Id") ON DELETE RESTRICT
);

-- Tabela LearningAreas
CREATE TABLE "LearningAreas" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(200) NOT NULL,
    "Content" TEXT NOT NULL,
    "Category" VARCHAR(100) NOT NULL,
    "Type" VARCHAR(50) DEFAULT 'article',
    "Author" VARCHAR(200),
    "Tags" TEXT,
    "IsPublished" BOOLEAN DEFAULT true,
    "Views" INTEGER DEFAULT 0,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela ClinicInfos
CREATE TABLE "ClinicInfos" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Address" VARCHAR(500) NOT NULL,
    "Phone" VARCHAR(20) NOT NULL,
    "Email" VARCHAR(100) NOT NULL,
    "Website" VARCHAR(200),
    "SocialMedia" VARCHAR(500),
    "OpeningHours" VARCHAR(200) NOT NULL,
    "Services" VARCHAR(1000) NOT NULL,
    "About" TEXT,
    "Mission" VARCHAR(500),
    "Vision" VARCHAR(500),
    "Values" VARCHAR(500),
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela Subscriptions
CREATE TABLE "Subscriptions" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Price" DECIMAL(18,2) NOT NULL,
    "Duration" INTEGER NOT NULL,
    "Features" TEXT,
    "IsActive" BOOLEAN DEFAULT true,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela ClientSubscriptions
CREATE TABLE "ClientSubscriptions" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "SubscriptionId" INTEGER NOT NULL,
    "StartDate" TIMESTAMP NOT NULL,
    "EndDate" TIMESTAMP NOT NULL,
    "Status" VARCHAR(50) DEFAULT 'active',
    "Amount" DECIMAL(18,2) NOT NULL,
    "AutoRenewal" BOOLEAN DEFAULT false,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("SubscriptionId") REFERENCES "Subscriptions"("Id") ON DELETE RESTRICT
);

-- Criar índices para performance
CREATE INDEX idx_appointments_date ON "Appointments"("ScheduledDate");
CREATE INDEX idx_appointments_client ON "Appointments"("ClientId");
CREATE INDEX idx_appointments_staff ON "Appointments"("StaffId");
CREATE INDEX idx_financial_date ON "FinancialTransactions"("TransactionDate");
CREATE INDEX idx_financial_client ON "FinancialTransactions"("ClientId");
CREATE INDEX idx_clients_email ON "Clients"("Email");
CREATE INDEX idx_staff_email ON "Staff"("Email");
```

---

## 🔷 SCRIPTS SQL SERVER

### **1. Criar o Banco de Dados:**
```sql
-- Conecte no SQL Server Management Studio e execute:
CREATE DATABASE DentalSpa_SqlServer;
GO

USE DentalSpa_SqlServer;
GO
```

### **2. Criar as tabelas:**
```sql
-- Tabela Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'staff',
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20),
    ResetToken NVARCHAR(255),
    ResetTokenExpiry DATETIME2,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    LastLogin DATETIME2
);

-- Tabela Clients
CREATE TABLE Clients (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    Address NVARCHAR(MAX),
    BirthDate DATE,
    Gender NVARCHAR(20),
    Profession NVARCHAR(100),
    MaritalStatus NVARCHAR(50),
    EmergencyContact NVARCHAR(200),
    EmergencyPhone NVARCHAR(20),
    Observations NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela Services
CREATE TABLE Services (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Category NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Duration INT DEFAULT 60,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela Staff
CREATE TABLE Staff (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    Specialization NVARCHAR(200),
    Schedule NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela Appointments
CREATE TABLE Appointments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClientId INT NOT NULL,
    ServiceId INT NOT NULL,
    StaffId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    ScheduledDate DATETIME2 NOT NULL,
    Duration INT DEFAULT 60,
    Status NVARCHAR(50) NOT NULL DEFAULT 'scheduled',
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    FOREIGN KEY (ServiceId) REFERENCES Services(Id),
    FOREIGN KEY (StaffId) REFERENCES Staff(Id)
);

-- Tabela Inventories
CREATE TABLE Inventories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Category NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    MinimumStock INT DEFAULT 10,
    Price DECIMAL(18,2),
    Supplier NVARCHAR(200),
    ExpirationDate DATE,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela FinancialTransactions
CREATE TABLE FinancialTransactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClientId INT,
    Type NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(MAX),
    PaymentMethod NVARCHAR(50) NOT NULL,
    TransactionDate DATETIME2 NOT NULL,
    Status NVARCHAR(50) DEFAULT 'completed',
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClientId) REFERENCES Clients(Id)
);

-- Tabela Packages
CREATE TABLE Packages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    DiscountPrice DECIMAL(18,2),
    Services NVARCHAR(MAX),
    Duration INT,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela ClientPackages
CREATE TABLE ClientPackages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClientId INT NOT NULL,
    PackageId INT NOT NULL,
    PurchaseDate DATETIME2 NOT NULL,
    ExpirationDate DATETIME2,
    Status NVARCHAR(50) DEFAULT 'active',
    RemainingServices INT,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    FOREIGN KEY (PackageId) REFERENCES Packages(Id)
);

-- Tabela BeforeAfters
CREATE TABLE BeforeAfters (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClientId INT NOT NULL,
    ServiceId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    BeforeImage NVARCHAR(MAX),
    AfterImage NVARCHAR(MAX),
    TreatmentDate DATETIME2,
    IsPublic BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    FOREIGN KEY (ServiceId) REFERENCES Services(Id)
);

-- Tabela LearningAreas
CREATE TABLE LearningAreas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50) DEFAULT 'article',
    Author NVARCHAR(200),
    Tags NVARCHAR(MAX),
    IsPublished BIT DEFAULT 1,
    Views INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela ClinicInfos
CREATE TABLE ClinicInfos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Website NVARCHAR(200),
    SocialMedia NVARCHAR(500),
    OpeningHours NVARCHAR(200) NOT NULL,
    Services NVARCHAR(1000) NOT NULL,
    About NVARCHAR(MAX),
    Mission NVARCHAR(500),
    Vision NVARCHAR(500),
    Values NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela Subscriptions
CREATE TABLE Subscriptions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    Duration INT NOT NULL,
    Features NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabela ClientSubscriptions
CREATE TABLE ClientSubscriptions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClientId INT NOT NULL,
    SubscriptionId INT NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    Status NVARCHAR(50) DEFAULT 'active',
    Amount DECIMAL(18,2) NOT NULL,
    AutoRenewal BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClientId) REFERENCES Clients(Id),
    FOREIGN KEY (SubscriptionId) REFERENCES Subscriptions(Id)
);

-- Criar índices para performance
CREATE INDEX IX_Appointments_ScheduledDate ON Appointments(ScheduledDate);
CREATE INDEX IX_Appointments_ClientId ON Appointments(ClientId);
CREATE INDEX IX_Appointments_StaffId ON Appointments(StaffId);
CREATE INDEX IX_FinancialTransactions_TransactionDate ON FinancialTransactions(TransactionDate);
CREATE INDEX IX_FinancialTransactions_ClientId ON FinancialTransactions(ClientId);
```

---

## 🔗 STRINGS DE CONEXÃO

### **Para uso no código (.NET):**

**PostgreSQL:**
```
Host=localhost;Database=dentalspa;Username=postgres;Password=postgres;Port=5432
```

**SQL Server:**
```
Server=localhost;Database=DentalSpa_SqlServer;Trusted_Connection=true;TrustServerCertificate=true;
```

---

## 📝 RESUMO DOS BANCOS

**PostgreSQL (Principal):**
- Nome: `dentalspa`
- 14 tabelas criadas
- Chaves estrangeiras configuradas
- Índices para performance

**SQL Server (Secundário):**
- Nome: `DentalSpa_SqlServer`
- 14 tabelas criadas (mesma estrutura)
- Compatível para sincronização

Execute primeiro os scripts do PostgreSQL, depois configure o SQL Server se quiser usar ambos os bancos simultaneamente.