-- ============================================
-- SCRIPT COMPLETO - TODAS AS TABELAS DO DENTALSPA
-- ============================================

-- Conectar no banco 'dentalspa' primeiro:
-- \c dentalspa;

-- ============================================
-- 1. TABELA USERS (Usuários do Sistema)
-- ============================================
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

-- ============================================
-- 2. TABELA CLIENTS (Clientes)
-- ============================================
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

-- ============================================
-- 3. TABELA SERVICES (Serviços)
-- ============================================
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

-- ============================================
-- 4. TABELA STAFF (Funcionários)
-- ============================================
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

-- ============================================
-- 5. TABELA APPOINTMENTS (Agendamentos)
-- ============================================
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
    CONSTRAINT "FK_Appointments_Clients" FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Appointments_Services" FOREIGN KEY ("ServiceId") REFERENCES "Services"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Appointments_Staff" FOREIGN KEY ("StaffId") REFERENCES "Staff"("Id") ON DELETE RESTRICT
);

-- ============================================
-- 6. TABELA INVENTORIES (Estoque)
-- ============================================
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

-- ============================================
-- 7. TABELA FINANCIALTRANSACTIONS (Transações Financeiras)
-- ============================================
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
    CONSTRAINT "FK_FinancialTransactions_Clients" FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT
);

-- ============================================
-- 8. TABELA PACKAGES (Pacotes de Serviços)
-- ============================================
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

-- ============================================
-- 9. TABELA CLIENTPACKAGES (Pacotes dos Clientes)
-- ============================================
CREATE TABLE "ClientPackages" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" INTEGER NOT NULL,
    "PackageId" INTEGER NOT NULL,
    "PurchaseDate" TIMESTAMP NOT NULL,
    "ExpirationDate" TIMESTAMP,
    "Status" VARCHAR(50) DEFAULT 'active',
    "RemainingServices" INTEGER,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_ClientPackages_Clients" FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ClientPackages_Packages" FOREIGN KEY ("PackageId") REFERENCES "Packages"("Id") ON DELETE RESTRICT
);

-- ============================================
-- 10. TABELA BEFOREAFTERS (Antes e Depois)
-- ============================================
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
    CONSTRAINT "FK_BeforeAfters_Clients" FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_BeforeAfters_Services" FOREIGN KEY ("ServiceId") REFERENCES "Services"("Id") ON DELETE RESTRICT
);

-- ============================================
-- 11. TABELA LEARNINGAREAS (Área de Aprendizado)
-- ============================================
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

-- ============================================
-- 12. TABELA CLINICINFOS (Informações da Clínica)
-- ============================================
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

-- ============================================
-- 13. TABELA SUBSCRIPTIONS (Planos de Assinatura)
-- ============================================
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

-- ============================================
-- 14. TABELA CLIENTSUBSCRIPTIONS (Assinaturas dos Clientes)
-- ============================================
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
    CONSTRAINT "FK_ClientSubscriptions_Clients" FOREIGN KEY ("ClientId") REFERENCES "Clients"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_ClientSubscriptions_Subscriptions" FOREIGN KEY ("SubscriptionId") REFERENCES "Subscriptions"("Id") ON DELETE RESTRICT
);

-- ============================================
-- ÍNDICES PARA PERFORMANCE
-- ============================================
CREATE INDEX "IX_Users_Username" ON "Users"("Username");
CREATE INDEX "IX_Users_Email" ON "Users"("Email");
CREATE INDEX "IX_Clients_Email" ON "Clients"("Email");
CREATE INDEX "IX_Clients_Phone" ON "Clients"("Phone");
CREATE INDEX "IX_Staff_Email" ON "Staff"("Email");
CREATE INDEX "IX_Appointments_ScheduledDate" ON "Appointments"("ScheduledDate");
CREATE INDEX "IX_Appointments_ClientId" ON "Appointments"("ClientId");
CREATE INDEX "IX_Appointments_StaffId" ON "Appointments"("StaffId");
CREATE INDEX "IX_Appointments_Status" ON "Appointments"("Status");
CREATE INDEX "IX_FinancialTransactions_TransactionDate" ON "FinancialTransactions"("TransactionDate");
CREATE INDEX "IX_FinancialTransactions_ClientId" ON "FinancialTransactions"("ClientId");
CREATE INDEX "IX_FinancialTransactions_Type" ON "FinancialTransactions"("Type");
CREATE INDEX "IX_Inventories_Category" ON "Inventories"("Category");
CREATE INDEX "IX_Inventories_ExpirationDate" ON "Inventories"("ExpirationDate");
CREATE INDEX "IX_Services_Category" ON "Services"("Category");
CREATE INDEX "IX_Services_IsActive" ON "Services"("IsActive");
CREATE INDEX "IX_ClientPackages_ClientId" ON "ClientPackages"("ClientId");
CREATE INDEX "IX_ClientPackages_Status" ON "ClientPackages"("Status");
CREATE INDEX "IX_BeforeAfters_ClientId" ON "BeforeAfters"("ClientId");
CREATE INDEX "IX_BeforeAfters_IsPublic" ON "BeforeAfters"("IsPublic");
CREATE INDEX "IX_LearningAreas_Category" ON "LearningAreas"("Category");
CREATE INDEX "IX_LearningAreas_IsPublished" ON "LearningAreas"("IsPublished");
CREATE INDEX "IX_ClientSubscriptions_ClientId" ON "ClientSubscriptions"("ClientId");
CREATE INDEX "IX_ClientSubscriptions_Status" ON "ClientSubscriptions"("Status");

-- ============================================
-- DADOS INICIAIS (OPCIONAL)
-- ============================================

-- Usuário administrador padrão
INSERT INTO "Users" ("Username", "Password", "FullName", "Role", "Email", "IsActive") 
VALUES ('admin', '$2a$11$5EjvNKJ8UglGgCr6w6k1QOyAKu1xH9bOp0LQBaV.YZaXQ7XzQ7XzQ', 'Administrador', 'admin', 'admin@dentalspa.com', true);

-- Informações básicas da clínica
INSERT INTO "ClinicInfos" ("Name", "Address", "Phone", "Email", "OpeningHours", "Services") 
VALUES ('DentalSpa Clínica', 'Rua das Flores, 123 - Centro', '(11) 99999-9999', 'contato@dentalspa.com', 'Segunda a Sexta: 8h às 18h', 'Odontologia Geral, Estética, Harmonização Facial');

-- ============================================
-- VERIFICAÇÃO FINAL
-- ============================================
-- Para verificar se todas as tabelas foram criadas:
-- SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' ORDER BY table_name;