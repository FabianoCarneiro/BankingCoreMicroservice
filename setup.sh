#!/bin/bash

# Script de Inicialização - Banking Core Microservice
# Suporta SQLite e SQL Server

set -e

echo "================================"
echo "Banking Core Microservice Setup"
echo "================================"
echo ""

# Cores para output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Verificar .NET SDK
echo -e "${BLUE}Verificando .NET SDK...${NC}"
DOTNET_VERSION=$(dotnet --version 2>&1)
echo -e "${GREEN}✓ .NET SDK encontrado: $DOTNET_VERSION${NC}"
echo ""

# Escolher banco de dados
echo -e "${BLUE}Escolha o banco de dados:${NC}"
echo "1) SQLite (padrão - recomendado para desenvolvimento)"
echo "2) SQL Server (requer instalação)"
read -p "Selecione (1 ou 2): " -r DB_CHOICE

if [ "$DB_CHOICE" = "2" ]; then
    DB_TYPE="sqlserver"
    echo -e "${YELLOW}Usando SQL Server${NC}"
    export DATABASE_TYPE=sqlserver
    
    # Pedir connection string
    read -p "Digite a connection string (pressione Enter para usar localdb): " CONNECTION_STR
    if [ -z "$CONNECTION_STR" ]; then
        CONNECTION_STR="Server=(localdb)\\mssqllocaldb;Database=BankingCore;Integrated Security=true;"
    fi
else
    DB_TYPE="sqlite"
    CONNECTION_STR="Data Source=banking.db"
    echo -e "${YELLOW}Usando SQLite${NC}"
fi

echo ""
echo -e "${BLUE}Limpando construções anteriores...${NC}"
dotnet clean 2>/dev/null || true
echo -e "${GREEN}✓ Limpeza concluída${NC}"
echo ""

echo -e "${BLUE}Restaurando dependências...${NC}"
dotnet restore
echo -e "${GREEN}✓ Dependências restauradas${NC}"
echo ""

echo -e "${BLUE}Compilando projeto...${NC}"
dotnet build
echo -e "${GREEN}✓ Projeto compilado com sucesso${NC}"
echo ""

# Se for SQL Server, aplicar migrations
if [ "$DB_TYPE" = "sqlserver" ]; then
    echo -e "${BLUE}Aplicando migrations do Entity Framework...${NC}"
    dotnet ef database update --project src/Core.Infrastructure 2>/dev/null || echo -e "${YELLOW}⚠ Migrations podem não ter sido aplicadas${NC}"
    echo ""
fi

# Se for SQLite, remover arquivo antigo (opcional)
if [ "$DB_TYPE" = "sqlite" ]; then
    if [ -f "banking.db" ]; then
        read -p "Arquivo 'banking.db' existe. Deseja recriar? (s/n): " -r RECREATE
        if [[ $RECREATE =~ ^[Ss]$ ]]; then
            rm -f banking.db
            echo -e "${GREEN}✓ Banco de dados anterior removido${NC}"
        fi
    fi
fi

echo ""
echo -e "${GREEN}================================${NC}"
echo -e "${GREEN}✓ Setup Concluído!${NC}"
echo -e "${GREEN}================================${NC}"
echo ""

echo -e "${BLUE}Para iniciar a API, execute:${NC}"
echo ""
echo "  dotnet run --project src/Core.API/Core.API.csproj"
echo ""
echo -e "${BLUE}Depois acesse:${NC}"
echo "  http://localhost:5000/swagger/index.html"
echo ""

# Oferecer para executar agora
read -p "Deseja executar a API agora? (s/n): " -r RUN_NOW
if [[ $RUN_NOW =~ ^[Ss]$ ]]; then
    echo ""
    echo -e "${BLUE}Iniciando API...${NC}"
    echo ""
    dotnet run --project src/Core.API/Core.API.csproj
fi
