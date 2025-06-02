# 🦷 DentalSpa - Arquitetura DDD Final

## 📁 Estrutura Organizada

### 🏗️ Domain (Camada de Domínio)
```
Domain/
├── Entities/          # Entidades do negócio
│   ├── User.cs
│   ├── Client.cs  
│   ├── Appointment.cs
│   ├── Service.cs
│   ├── Staff.cs
│   ├── Inventory.cs
│   ├── Financial.cs
│   ├── Package.cs
│   └── BeforeAfter.cs
├── Interfaces/        # Contratos dos repositórios
│   ├── IAuthRepository.cs
│   ├── IClientRepository.cs
│   ├── IAppointmentRepository.cs
│   ├── IServiceRepository.cs
│   ├── IStaffRepository.cs
│   ├── IInventoryRepository.cs
│   ├── IFinancialRepository.cs
│   ├── IPackageRepository.cs
│   ├── IBeforeAfterRepository.cs
│   └── IAgendaRepository.cs
└── ValueObjects/      # Objetos de valor
```

### 🔧 Application (Camada de Aplicação)
```
Application/
├── Services/          # Implementações dos serviços
│   ├── AuthService.cs
│   ├── ClientService.cs
│   ├── AppointmentService.cs
│   ├── AgendaService.cs
│   ├── ServiceService.cs
│   ├── StaffService.cs
│   ├── InventoryService.cs
│   ├── FinancialService.cs
│   ├── PackageService.cs
│   └── BeforeAfterService.cs
├── Interfaces/        # Contratos dos serviços
│   ├── IAuthService.cs
│   ├── IClientService.cs
│   ├── IAppointmentService.cs
│   ├── IAgendaService.cs
│   ├── IServiceService.cs
│   ├── IStaffService.cs
│   ├── IInventoryService.cs
│   ├── IFinancialService.cs
│   ├── IPackageService.cs
│   └── IBeforeAfterService.cs
└── DTOs/              # Objetos de transferência
```

### 🗄️ Infrastructure (Camada de Infraestrutura)
```
Infrastructure/
├── Data/              # Contexto do banco
│   └── DentalSpaDbContext.cs
└── Repositories/      # Implementações dos repositórios
    ├── AuthRepository.cs
    ├── ClientRepository.cs
    ├── AppointmentRepository.cs
    ├── ServiceRepository.cs
    ├── StaffRepository.cs
    ├── InventoryRepository.cs
    ├── FinancialRepository.cs
    ├── PackageRepository.cs
    ├── BeforeAfterRepository.cs
    └── AgendaRepository.cs
```

### 🌐 Controllers (Camada de Apresentação)
```
Controllers/
├── AuthController.cs
├── ClientsController.cs
├── AppointmentsController.cs
├── AgendaController.cs
├── ServicesController.cs
├── StaffController.cs
├── InventoryController.cs
├── FinancialController.cs
├── PackageController.cs
├── BeforeAfterController.cs
└── AnalyticsController.cs
```

## ✅ Migração Concluída

### Removido:
- ❌ Todas as pastas duplicadas (Services/, Repositories/, Data/)
- ❌ Arquivos do Express.js
- ❌ Namespaces antigos (ClinicApi.*)

### Atualizado:
- ✅ Todos os namespaces para DentalSpa.*
- ✅ Program.cs com injeção de dependências DDD
- ✅ Estrutura de camadas seguindo DDD
- ✅ Repositórios na camada Infrastructure
- ✅ Serviços na camada Application
- ✅ Entidades na camada Domain

### Tecnologias:
- ✅ .NET Core 8.0
- ✅ Entity Framework Core
- ✅ PostgreSQL
- ✅ JWT Authentication
- ✅ Swagger/OpenAPI
- ✅ Arquitetura DDD com SOLID

## 🚀 Execução

Usar o script: `./start-dentalspa.sh` ou:
```bash
cd backend-dotnet
dotnet run --urls "http://0.0.0.0:5000"
```