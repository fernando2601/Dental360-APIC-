#!/bin/bash

echo "Iniciando migração Angular/.NET..."

# Instalar dependências do Angular
echo "Instalando dependências do Angular..."
cd frontend-angular
npm install

# Iniciar backend .NET em background
echo "Iniciando backend .NET Core..."
cd ../backend-dotnet
dotnet run --urls="https://localhost:5001;http://localhost:5000" &
DOTNET_PID=$!

# Aguardar o backend .NET inicializar
sleep 5

# Iniciar frontend Angular
echo "Iniciando frontend Angular..."
cd ../frontend-angular
ng serve --host 0.0.0.0 --port 4200 &
ANGULAR_PID=$!

echo "Sistemas iniciados:"
echo "- Backend .NET: https://localhost:5001"
echo "- Frontend Angular: http://localhost:4200"
echo "- Sistema atual React: http://localhost:5000"

# Manter os processos rodando
wait $DOTNET_PID $ANGULAR_PID