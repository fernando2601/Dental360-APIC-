DentalSpa API - .NET Core 8.0
Sistema de gestão para clínica odontológica e de harmonização facial construído com .NET Core 8.0, ADO.NET e PostgreSQL, seguindo arquitetura Domain-Driven Design (DDD). 

Estrutura do Projeto
backend-dotnet/
├── Controllers/          # Controllers da API
├── Application/          # Camada de aplicação (serviços e interfaces)
│   ├── Interfaces/       # Interfaces dos serviços
│   ├── Services/         # Implementação dos serviços
│   └── Mappings/         # Mapeamentos de objetos
├── Domain/              # Camada de domínio
│   ├── Entities/         # Entidades do domínio
│   └── Interfaces/       # Interfaces dos repositórios
├── Infrastructure/       # Camada de infraestrutura
│   ├── Data/            # Scripts SQL e configurações
│   └── Repositories/     # Implementação dos repositórios
├── Properties/          # Configurações do projeto
├── DentalSpa.API.csproj # Arquivo de projeto
├── Program.cs           # Ponto de entrada da aplicação
├── appsettings.json     # Configurações gerais
└── appsettings.Development.json # Configurações de desenvolvimento
Pré-requisitos
.NET 8.0 SDK
PostgreSQL
Visual Studio 2022 ou Visual Studio Code
Como Executar
Usando Visual Studio 2022
Abra o arquivo DentalSpa.sln no Visual Studio
Configure a string de conexão no appsettings.json
Pressione F5 para executar
Usando Visual Studio Code
Abra o diretório backend-dotnet no VS Code
Configure a string de conexão no appsettings.json
Execute: dotnet run
Usando linha de comando
cd backend-dotnet
dotnet restore
dotnet run
Configuração do Banco de Dados
Configure a string de conexão PostgreSQL no arquivo appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dental_spa;Username=seu_usuario;Password=sua_senha;"
  }
}
Endpoints da API
A API estará disponível em:

HTTP: http://localhost:5000
Swagger UI: http://localhost:5000/api-docs
Módulos Implementados
✅ Autenticação JWT
✅ Gestão de Pacientes
✅ Agendamentos
✅ Serviços
✅ Inventário
✅ Financeiro
✅ Equipe
✅ Pacotes
✅ Antes e Depois
✅ Área de Aprendizado
✅ Informações da Clínica
✅ Assinaturas
✅ Orçamentos
✅ Usuários
Arquitetura
O projeto segue a arquitetura Domain-Driven Design (DDD) com as seguintes camadas:

Domain: Contém as entidades de negócio e interfaces dos repositórios
Application: Contém a lógica de aplicação, serviços e interfaces
Infrastructure: Contém a implementação dos repositórios usando ADO.NET
Controllers: Contém os endpoints da API REST
Tecnologias Utilizadas
.NET Core 8.0
ADO.NET (Npgsql para PostgreSQL)
PostgreSQL
JWT Authentication
Swagger/OpenAPI
BCrypt para hash de senhas
Arquitetura DDD
