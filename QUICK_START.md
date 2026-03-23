# 🚀 Quick Start - Banking Core Microservice

## 📦 Requisitos

- **.NET 8** ou superior
- **SQL Server** (LocalDB ou servidor remoto)
- **Git** (opcional)

## ⚡ Primeiros Passos

### 1. Restaurar Dependências

```bash
cd ~/BankingCoreMicroservice
dotnet restore
```

### 2. Build do Projeto

```bash
dotnet build
```

### 3. Executar Testes

```bash
dotnet test
```

### 4. Iniciar a API

```bash
dotnet run --project src/Core.API
```

A API estará disponível em: `https://localhost:5001`

**Swagger/OpenAPI**: `https://localhost:5001/swagger/index.html`

## 📝 Estrutura do Projeto

```
BankingCoreMicroservice/
├── src/
│   ├── Core.Domain/              # Entidades, Value Objects, Portas (Interfaces)
│   ├── Core.Application/         # Use Cases, DTOs, Mappers
│   ├── Core.Infrastructure/      # Implementações, Repositórios, DB Context
│   └── Core.API/                 # Controladores REST, Configuração
├── tests/
│   └── Core.Tests/               # Testes Unitários (xUnit)
├── README.md                     # Documentação principal
├── ARCHITECTURE.md               # Detalhes de arquitetura hexagonal
├── API_EXAMPLES.md               # Exemplos de requisições HTTP
└── QUICK_START.md                # Este arquivo
```

## 🎯 Recursos Implementados

### ✅ Cadastro de Clientes (KYC)
```bash
POST /api/customers
```
- Validação de CPF
- Armazenamento seguro de dados

### ✅ Contas Correntes
```bash
POST /api/accounts
GET /api/accounts/{accountNumber}/balance
GET /api/accounts/{accountNumber}/statement
```
- Criação de contas
- Consulta de saldo
- Visualização de extrato

### ✅ Transferências (TED/PIX)
```bash
POST /api/accounts/transfer
```
- Transferências entre contas
- Notificações por email
- Auditoria de transações

## 🧪 Testar com curl

### Criar Cliente
```bash
curl -X POST https://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "12345678909",
    "name": "João Silva",
    "email": "joao@example.com",
    "phoneNumber": "11987654321"
  }'
```

### Criar Conta
```bash
curl -X POST https://localhost:5001/api/accounts \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "{ID_CLIENTE_AQUI}"
  }'
```

### Consultar Saldo
```bash
curl https://localhost:5001/api/accounts/{ACCOUNT_NUMBER}/balance
```

### Transferir
```bash
curl -X POST https://localhost:5001/api/accounts/transfer \
  -H "Content-Type: application/json" \
  -d '{
    "fromAccountNumber": "123456",
    "toAccountNumber": "789012",
    "amount": 100.00
  }'
```

## 🏗️ Padrões Utilizados

- **Arquitetura Hexagonal** (Ports & Adapters)
- **Domain-Driven Design** (DDD)
- **Repository Pattern**
- **Value Objects**
- **Agregados**

## 🔑 Value Objects Implementados

### Money
- Encapsula valor monetário com validação
- Operações: Add, Subtract
- Múltiplas moedas suportadas

### CPF
- Validação completa de CPF brasileiro
- Cálculo de dígitos verificadores
- Formatação automática

## 📊 Entidades

### Customer (Agregado)
- ID, CPF, Nome, Email, Telefone
- Data de criação, Status ativo

### BankAccount (Agregado)
- ID, Número da Conta, Agência
- Saldo (Money), Status ativo
- Histórico de Transações

### Transaction (Entidade)
- ID, Tipo (Depósito/Saque/Transferência)
- Valor (Money), Descrição
- Data de criação

## 🗄️ Banco de Dados

### Configuração
- **SQL Server** via Entity Framework Core
- **LocalDB** padrão (alterar em `appsettings.json`)
- Migrations automáticas na primeira execução

### Connection String
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BankingCore;Trusted_Connection=true;"
```

## 📚 Referências Importantes

- **README.md** - Visão geral do projeto
- **ARCHITECTURE.md** - Detalhes técnicos da arquitetura
- **API_EXAMPLES.md** - Exemplos completos de requisições

## 🐛 Troubleshooting

### Erro de Banco de Dados
- Verificar se LocalDB está instalado
- Conferir connection string em `appsettings.json`
- Executar `dotnet ef database update` se necessário

### Erro de Porta (5001)
- Verificar se porta 5001 está disponível
- Alterar em `Properties/launchSettings.json`

### Erro de Dependências
- Executar `dotnet restore`
- Limpar cache: `dotnet nuget locals all --clear`

## 🚀 Próximas Etapas

1. **Autenticação/Autorização** - Adicionar JWT
2. **Criptografia** - Dados sensíveis em repouso
3. **Rate Limiting** - Proteção contra abuso
4. **Logging Avançado** - Serilog com Elasticsearch
5. **Containerização** - Docker e Docker Compose
6. **CI/CD** - GitHub Actions ou Azure Pipelines

## 📞 Suporte

Para dúvidas sobre:
- **Arquitetura**: Consulte `ARCHITECTURE.md`
- **API**: Consulte `API_EXAMPLES.md`
- **Código**: Veja comentários nos arquivos `.cs`

---

**Criado**: 2026-03-23  
**Versão**: 1.0.0  
**Status**: ✅ Pronto para desenvolvimento
