# DentalSpa API - .NET Core 8.0

Sistema de gestão para clínica odontológica e de harmonização facial construído com .NET Core 8.0, Dapper ORM e PostgreSQL.

## Estrutura do Projeto

```
backend-dotnet/
├── Controllers/          # Controllers da API
├── Models/              # Modelos de dados
├── Services/            # Lógica de negócio
├── Properties/          # Configurações do projeto
├── DentalSpa.API.csproj # Arquivo de projeto
├── Program.cs           # Ponto de entrada da aplicação
├── appsettings.json     # Configurações gerais
└── appsettings.Development.json # Configurações de desenvolvimento
```

## Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL
- Visual Studio 2022 ou Visual Studio Code

## Como Executar

### Usando Visual Studio 2022
1. Abra o arquivo `DentalSpa.sln` no Visual Studio
2. Configure a string de conexão no `appsettings.json`
3. Pressione F5 para executar

### Usando Visual Studio Code
1. Abra o diretório `backend-dotnet` no VS Code
2. Configure a string de conexão no `appsettings.json`
3. Execute: `dotnet run`

### Usando linha de comando
```bash
cd backend-dotnet
dotnet restore
dotnet run
```

## Configuração do Banco de Dados

Configure a string de conexão PostgreSQL no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dental_spa;Username=seu_usuario;Password=sua_senha;"
  }
}
```

## Endpoints da API

A API estará disponível em:
- HTTP: http://localhost:5001
- HTTPS: https://localhost:7001
- Swagger UI: http://localhost:5001/swagger

## Módulos Implementados

- ✅ Autenticação JWT
- ✅ Gestão de Pacientes
- ✅ Agendamentos
- ✅ Serviços
- ✅ Inventário
- ✅ Financeiro
- ✅ Equipe
- ✅ Pacotes
- ✅ Antes e Depois
- ✅ Área de Aprendizado
- ✅ Informações da Clínica
- ✅ Assinaturas

## Tecnologias Utilizadas

- .NET Core 8.0
- Dapper ORM
- PostgreSQL
- JWT Authentication
- Swagger/OpenAPI
- BCrypt para hash de senhas