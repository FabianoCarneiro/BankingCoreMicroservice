# 🎉 Implementação Completa: SQLite no Banking Core Microservice

## ✅ O que foi implementado

### 1. **Suporte a SQLite** (Principal)
- ✅ Adicionado pacote NuGet `Microsoft.EntityFrameworkCore.Sqlite`
- ✅ Configuração automática de banco SQLite
- ✅ Arquivo `banking.db` criado automaticamente

### 2. **Suporte a SQL Server** (Opcional)
- ✅ Mantém compatibilidade com SQL Server (original)
- ✅ Seleção via variável de ambiente `DATABASE_TYPE`

### 3. **Inicialização do Banco de Dados**
- ✅ Middleware personalizado para criar banco na primeira execução
- ✅ Seed automático com 4 clientes de teste
- ✅ CPFs válidos gerados aleatoriamente
- ✅ Contas bancárias associadas criadas automaticamente

### 4. **Documentação e Scripts**
- ✅ `DATABASE_SETUP.md` - Guia completo
- ✅ `setup.sh` - Script para Linux/macOS
- ✅ `setup.bat` - Script para Windows
- ✅ Classe `BankingContextSeed` para população de dados

---

## 🚀 Como Usar (Comece Aqui!)

### Opção 1: Inicialização Rápida (SQLite)

```bash
# 1. Compilar
dotnet build

# 2. Executar
dotnet run --project src/Core.API/Core.API.csproj

# 3. Acessar Swagger
# Abra no navegador: http://localhost:5000/swagger/index.html
```

**Pronto!** O SQLite `banking.db` será criado automaticamente com dados de teste.

### Opção 2: Usar Script de Setup

**macOS / Linux:**
```bash
chmod +x setup.sh
./setup.sh
```

**Windows:**
```cmd
setup.bat
```

O script irá perguntar se você quer usar SQLite ou SQL Server.

---

## 📊 Dados de Teste

Quando a API é executada, 4 clientes são criados automaticamente:

```bash
# GET http://localhost:5000/api/customers

[
  {
    "id": "fc3ae787-acd8-44b5-b2a8-07a64981c040",
    "cpf": "435.565.190-89",
    "name": "João Silva",
    "email": "joao@example.com",
    "phoneNumber": "11999999999",
    "createdAt": "2026-03-25T01:02:56.848654",
    "isActive": true
  },
  {
    "id": "b760e222-32bf-4338-97a4-459713f4f3e5",
    "cpf": "508.833.694-27",
    "name": "Pedro Oliveira",
    "email": "pedro@example.com",
    "phoneNumber": "11977777777",
    "createdAt": "2026-03-25T01:02:56.848887",
    "isActive": true
  },
  {
    "id": "3f73af91-5dd8-429d-bb86-8d68e8f52f15",
    "cpf": "870.059.236-69",
    "name": "Maria Santos",
    "email": "maria@example.com",
    "phoneNumber": "11988888888",
    "createdAt": "2026-03-25T01:02:56.84888",
    "isActive": true
  },
  {
    "id": "9dc0c458-8271-4c9c-9793-05f5f52348e2",
    "cpf": "542.986.166-90",
    "name": "Ana Costa",
    "email": "ana@example.com",
    "phoneNumber": "11966666666",
    "createdAt": "2026-03-25T01:02:56.848892",
    "isActive": true
  }
]
```

---

## 🧪 Testando a API

### 1. Listar todos os clientes
```bash
curl http://localhost:5000/api/customers
```

### 2. Obter cliente por ID
```bash
curl http://localhost:5000/api/customers/fc3ae787-acd8-44b5-b2a8-07a64981c040
```

### 3. Criar novo cliente
```bash
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Novo Cliente",
    "cpf": "12345678901",
    "email": "novo@example.com",
    "phoneNumber": "11912345678"
  }'
```

### 4. Testar via Swagger
```
http://localhost:5000/swagger/index.html
```

---

## 📁 Onde o Banco SQLite é Criado

O arquivo `banking.db` é criado no diretório de execução do projeto:

```
/Users/fabianocarneiro/BankingCoreMicroservice/src/Core.API/banking.db
```

**Para mudar o local**, edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/caminho/customizado/banking.db"
  }
}
```

---

## 🔄 Mudar entre SQLite e SQL Server

### Para SQLite (Padrão)
```bash
# Não precisa fazer nada, SQLite é o padrão
dotnet run --project src/Core.API/Core.API.csproj
```

### Para SQL Server
```bash
# macOS / Linux
export DATABASE_TYPE=sqlserver

# Windows (PowerShell)
$env:DATABASE_TYPE="sqlserver"

# Depois executar
dotnet run --project src/Core.API/Core.API.csproj
```

**Nota:** Para SQL Server, configure a connection string em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BankingCore;Integrated Security=true;"
  }
}
```

---

## 🗑️ Resetar o Banco de Dados

### SQLite
```bash
# Parar a API (Ctrl+C)
rm src/Core.API/banking.db

# A próxima execução criará um novo banco vazio e populará com dados de teste
dotnet run --project src/Core.API/Core.API.csproj
```

### SQL Server
```bash
# Criar novo banco
dotnet ef database drop --project src/Core.Infrastructure
dotnet ef database create --project src/Core.Infrastructure

# Aplicar migrations
dotnet ef database update --project src/Core.Infrastructure
```

---

## 📊 Estrutura das Tabelas SQLite

### Customers
```sql
CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CPF TEXT UNIQUE NOT NULL,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL,
    PhoneNumber TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME,
    IsActive INTEGER NOT NULL
);
```

### BankAccounts
```sql
CREATE TABLE BankAccounts (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    AccountNumber TEXT NOT NULL,
    Branch TEXT NOT NULL,
    Balance TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    ClosedAt DATETIME,
    IsActive INTEGER NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
```

### Transactions
```sql
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    Type INTEGER NOT NULL,
    Amount TEXT NOT NULL,
    Description TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    IsProcessed INTEGER NOT NULL,
    FOREIGN KEY (AccountId) REFERENCES BankAccounts(Id)
);
```

---

## 🛠️ Componentes Implementados

### 1. **Core.Infrastructure.csproj**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
```

### 2. **Program.cs** (Configuração Dinâmica)
```csharp
var databaseType = Environment.GetEnvironmentVariable("DATABASE_TYPE") ?? "sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BankingContext>(options =>
{
    if (databaseType.ToLower() == "sqlite")
    {
        var sqliteConnection = connectionString ?? "Data Source=banking.db";
        options.UseSqlite(sqliteConnection);
    }
    else
    {
        // SQL Server config...
    }
});
```

### 3. **DatabaseInitializationMiddleware** (Novo)
```csharp
// Arquivo: src/Core.API/Middlewares/DatabaseInitializationMiddleware.cs
// Responsável por:
// - Criar tabelas na primeira execução
// - Preencher dados de teste
// - Validar banco de dados
```

### 4. **BankingContextSeed** (Novo)
```csharp
// Arquivo: src/Core.Infrastructure/Persistence/BankingContextSeed.cs
// Responsável por:
// - Gerar CPFs válidos aleatoriamente
// - Criar 4 clientes de teste
// - Criar contas bancárias associadas
```

---

## 📋 Verificação do Build

```bash
$ dotnet build
...
Build succeeded. 0 Error(s), 47 Warning(s)
Time Elapsed 00:00:07.50
```

✅ **Build com sucesso!**

---

## 🎯 Próximos Passos

1. ✅ **Banco de dados funcionando** (SQLite)
2. ⏭️ Implementar endpoints PUT (atualizar cliente)
3. ⏭️ Implementar endpoints DELETE (deletar cliente)
4. ⏭️ Adicionar paginação e filtros
5. ⏭️ Implementar autenticação/autorização
6. ⏭️ Deploy em produção

---

## 🐛 Troubleshooting

### "Erro: no such table: Customers"
```bash
# O banco não foi inicializado
# Solução: Reinicie a API
# A primeira requisição disparará a inicialização
```

### "SQLite Error 1: 'database is locked'"
```bash
# Múltiplos processos acessando o banco
# Solução: Feche a API e todos os processos
pkill -f "dotnet run"

# Remova o arquivo antigo
rm src/Core.API/banking.db

# Reinicie
dotnet run --project src/Core.API/Core.API.csproj
```

### "Connection string inválida"
```bash
# Verifique appsettings.json
# A string padrão é: "Data Source=banking.db"
# Para diretório customizado: "Data Source=/path/to/banking.db"
```

---

## 📚 Referências

- [Entity Framework Core - SQLite](https://docs.microsoft.com/ef/core/providers/sqlite)
- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [Banking Core Microservice - README.md](./README.md)
- [Guia Completo - DATABASE_SETUP.md](./DATABASE_SETUP.md)

---

## ✨ Resumo

| Aspecto | Status | Detalhes |
|--------|--------|----------|
| SQLite | ✅ | Funcionando perfeitamente |
| SQL Server | ✅ | Suportado via variável de ambiente |
| Dados de Teste | ✅ | 4 clientes com CPFs válidos |
| Contas Bancárias | ✅ | Criadas automaticamente |
| Scripts Setup | ✅ | Linux/macOS e Windows |
| Documentação | ✅ | Completa e detalhada |
| Build | ✅ | 0 erros |
| API Rodando | ✅ | Respondendo requisições |

**Parabéns! 🎉 Seu Banking Core Microservice está 100% funcional com SQLite!**
