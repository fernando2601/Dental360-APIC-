#!/bin/bash

echo "🦷 Iniciando DentalSpa API com Arquitetura DDD"
echo "🏗️  Camadas: Domain, Application, Infrastructure, Service"

cd backend-dotnet

echo "📦 Restaurando dependências..."
dotnet restore

echo "🔨 Compilando aplicação..."
dotnet build

echo "🚀 Iniciando servidor na porta 5000..."
dotnet run