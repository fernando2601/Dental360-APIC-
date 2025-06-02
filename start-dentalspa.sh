#!/bin/bash

echo "🦷 Iniciando DentalSpa - Sistema de Gestão para Clínica Dental"
echo "================================================================"

# Navegar para o diretório do backend .NET
cd backend-dotnet

echo "🔧 Verificando dependências do .NET..."
dotnet restore

echo "🏗️  Compilando aplicação..."
dotnet build

echo "🚀 Iniciando servidor na porta 5000..."
dotnet run --urls "http://0.0.0.0:5000"