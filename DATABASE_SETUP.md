# Configuração de Banco de Dados - Banking Core Microservice

## Visão Geral

Este projeto agora suporta **dois bancos de dados**:

1. **SQLite** (Padrão) ✅ - Perfeito para desenvolvimento e testes
2. **SQL Server** (Opcional) - Para produção

## 🎯 Opção 1: SQLite (Recomendado para Começar)

### Vantagens
- ✅ Sem instalação necessária
- ✅ Arquivo local (`banking.db`)
- ✅ Perfeito para desenvolvimento
- ✅ Fácil de resetar (delete do arquivo)
- ✅ Pequeno e rápido

### Como Usar

#### 1. Build do projeto
```bash
dotnet build
```

#### 2. Executar a API
```bash
dotnet run --project src/Core.API/Core.API.csproj
```

O SQLite será usado automaticamente! O arquivo `banking.db` será criado na pasta raiz.

#### 3. Testar via Swagger
```
http://localhost:5000/swagger/index.html
```

#### 4. Resetar o banco de dados
```bash
rm banking.db
# A próxima execução criará um novo banco vazio
```

### Arquivo de Configuração
`src/Core.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=banking.db"
  }
}
```

---

## 🔧 Opção 2: SQL Server

### Pré-requisitos
- SQL Server localdb ou remoto instalado
- Conexão configurada

### Como Usar

#### 1. Definir variável de ambiente
```bash
# macOS / Linux
export DATABASE_TYPE=sqlserver

# Windows (PowerShell)
$env:DATABASE_TYPE="sqlserver"

# Windows (Command Prompt)
set DATABASE_TYPE=sqlserver
```

#### 2. Configurar connection string em `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BankingCore;Integrated Security=true;"
  }
}
```

#### 3. Criar migrations (se necessário)
```bash
dotnet ef migrations add InitialCreate --project src/Core.Infrastructure
dotnet ef database update --project src/Core.Infrastructure
```

#### 4. Executar
```bash
dotnet run --project src/Core.API/Core.API.csproj
```

---

## 📊 Estrutura de Dados

### Tabela: Customers
```sql
CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CPF VARCHAR(11) UNIQUE NOT NULL,
    Name VARCHAR(MAX) NOT NULL,
    Email VARCHAR(MAX) NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL
);
```

### Tabela: BankAccounts
```sql
CREATE TABLE BankAccounts (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    AccountNumber VARCHAR(20) NOT NULL,
    Balance VARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
```

### Tabela: Transactions
```sql
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SourceAccountId UNIQUEIDENTIFIER NOT NULL,
    DestinationAccountId UNIQUEIDENTIFIER NOT NULL,
    Amount VARCHAR(50) NOT NULL,
    TransactionDate DATETIME NOT NULL,
    FOREIGN KEY (SourceAccountId) REFERENCES BankAccounts(Id),
    FOREIGN KEY (DestinationAccountId) REFERENCES BankAccounts(Id)
);
```

---

## 🚀 Teste Rápido (SQLite)

### 1. Iniciar a API
```bash
dotnet run --project src/Core.API/Core.API.csproj
```

Você verá logs como:
```
[11:23:45 INF] Usando SQLite: Data Source=banking.db
[11:23:46 INF] Iniciando aplicação Banking Core Microservice
[11:23:46 INF] Now listening on: http://localhost:5000
```

### 2. Criar um Cliente
```bash
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "cpf": "12345678901",
    "email": "joao@example.com",
    "phoneNumber": "11999999999"
  }'
```

Resposta:
```json
{
  "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "cpf": "12345678901",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11999999999"
}
```

### 3. Listar Clientes
```bash
curl http://localhost:5000/api/customers
```

### 4. Obter Cliente por ID
```bash
curl http://localhost:5000/api/customers/a1b2c3d4-e5f6-7890-1234-567890abcdef
```

---

## 📝 Scripts Úteis

### Reset Completo (SQLite)
```bash
#!/bin/bash
rm -f banking.db
dotnet build
dotnet run --project src/Core.API/Core.API.csproj
```

### Executar Testes
```bash
dotnet test
```

### Ver Arquivo SQLite
```bash
# Instalar sqlite3 (se não tiver)
# macOS: brew install sqlite
# Ubuntu: sudo apt-get install sqlite3

sqlite3 banking.db ".tables"
sqlite3 banking.db "SELECT * FROM Customers;"
```

---

## 🔄 Migração entre Bancos

### SQLite → SQL Server

1. **Exportar dados do SQLite**
```bash
sqlite3 banking.db ".mode json" ".output data.json" "SELECT * FROM Customers;"
```

2. **Configurar SQL Server em appsettings.json**

3. **Executar migrations**
```bash
dotnet ef migrations add InitialCreate --project src/Core.Infrastructure
dotnet ef database update --project src/Core.Infrastructure
```

4. **Importar dados**
Use ferramentas como SQL Server Management Studio ou scripts customizados.

---

## 🐛 Troubleshooting

### SQLite
**Erro: "Database is locked"**
- Feche todos os processos usando o banco
- Delete `banking.db` e recrie

**Erro: "Cannot create file banking.db"**
- Verifique permissões na pasta raiz
- Mude a pasta: `Data Source=/tmp/banking.db`

### SQL Server
**Erro: "Login failed"**
- Verifique connection string em appsettings.json
- Certifique-se que SQL Server está rodando

**Erro: "Database does not exist"**
```bash
dotnet ef database create --project src/Core.Infrastructure
```

---

## 📌 Recomendações

| Cenário | Recomendação |
|---------|--------------|
| Desenvolvimento Local | SQLite ✅ |
| Testes Automatizados | SQLite ✅ |
| Produção | SQL Server / Postgres / MySQL |
| Demo / Prototipagem | SQLite ✅ |
| Múltiplos Usuários | SQL Server |

---

## 🎯 Próximos Passos

1. ✅ Banco de dados funcionando
2. ⏭️ Implementar CRUD completo (PUT, DELETE)
3. ⏭️ Adicionar validações e error handling
4. ⏭️ Implementar autenticação/autorização
5. ⏭️ Deploy em produção

---

Alguma dúvida? Consulte a documentação oficial:
- [Entity Framework Core](https://docs.microsoft.com/ef/)
- [SQLite](https://www.sqlite.org/docs.html)
