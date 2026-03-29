# 🏗️ Arquitetura de Microserviços - Banking Core

## 📋 Visão Geral

O projeto foi transformado em uma **arquitetura de microserviços** com dois serviços independentes:

1. **Customer Service** 👥 - Gerenciamento de Clientes
2. **Banking Core Service** 💰 - Operações Bancárias

---

## 🏢 Estrutura de Microserviços

### 1️⃣ Customer Service (Porta 5001)

**Responsabilidade**: Gerenciar clientes e dados de contato

```
src/Customer.Service/
├── Customer.Domain/           # Entidades e portos
│   ├── Entities/              # Customer
│   ├── ValueObjects/          # CPF
│   └── Ports/                 # ICustomerRepository
├── Customer.Application/      # Lógica de negócio
│   ├── UseCases/
│   │   ├── CreateCustomerUseCase
│   │   ├── GetCustomerByIdUseCase
│   │   ├── ListAllCustomersUseCase
│   │   ├── UpdateCustomerUseCase
│   │   └── DeleteCustomerUseCase
│   └── DTOs/                  # CustomerDTOs
├── Customer.Infrastructure/   # Persistência
│   ├── Adapters/              # CustomerRepository
│   └── Persistence/           # CustomerContext
└── Customer.API/              # API REST
    ├── Controllers/           # CustomersController
    ├── Middlewares/           # SeedDatabaseMiddleware
    ├── Program.cs
    └── appsettings.json
```

**Endpoints**:
```
POST   /api/customers              - Criar cliente
GET    /api/customers              - Listar clientes
GET    /api/customers/{id}         - Obter cliente por ID
PUT    /api/customers/{id}         - Atualizar cliente
DELETE /api/customers/{id}         - Deletar cliente
```

**Banco de Dados**: `customer.db` (SQLite)

---

### 2️⃣ Banking Core Service (Porta 5000)

**Responsabilidade**: Operações bancárias (contas, transferências)

```
src/Core/
├── Core.Domain/               # Entidades e portos
│   ├── Entities/
│   │   ├── Customer (deprecado - usar Customer Service)
│   │   ├── BankAccount
│   │   └── Transaction
│   ├── ValueObjects/
│   │   ├── Money
│   │   └── CPF (deprecado)
│   └── Ports/
├── Core.Application/          # Lógica de negócio
│   ├── UseCases/
│   │   ├── CreateBankAccountUseCase
│   │   ├── TransferUseCase
│   │   └── ...
│   └── DTOs/
├── Core.Infrastructure/       # Persistência
│   ├── Adapters/
│   └── Persistence/
└── Core.API/                  # API REST
    ├── Controllers/
    ├── Middlewares/
    ├── Program.cs
    └── appsettings.json
```

**Endpoints**:
```
POST /api/accounts            - Criar conta
POST /api/transfers           - Fazer transferência
GET  /api/accounts            - Listar contas
...
```

**Banco de Dados**: `banking.db` (SQLite)

---

### 3️⃣ Shared Libraries 📦

```
src/Shared/
├── Shared.DTOs/               # DTOs compartilhadas
│   └── CustomerDTO            # Usado na comunicação inter-serviços
└── Shared.HttpClients/        # Clientes HTTP
    └── CustomerServiceClient  # Cliente para chamar Customer Service
```

---

## 🔄 Comunicação entre Microserviços

### Padrão HTTP Síncrono

O **Banking Core Service** se comunica com o **Customer Service** via HTTP:

```csharp
// Exemplo: Banking Core precisa validar cliente
var customerServiceClient = new CustomerServiceClient(httpClient);
customerServiceClient.BaseUrl = "http://localhost:5001";

var customer = await customerServiceClient.GetCustomerByIdAsync(customerId);
if (customer == null)
    throw new InvalidOperationException("Cliente não encontrado");
```

### Fluxo de Criação de Conta

```
1. Cliente chama: POST /api/accounts (Banking Core)
2. Banking Core chama: GET /api/customers/{id} (Customer Service)
3. Customer Service retorna dados do cliente
4. Banking Core cria conta associada ao cliente
5. Retorna 201 Created
```

---

## 🐳 Execução com Docker Compose

### Executar os dois microserviços

```bash
docker-compose up -d
```

### Verificar status

```bash
docker-compose ps
```

### Logs

```bash
docker-compose logs -f customer-service
docker-compose logs -f banking-core-service
```

### Parar

```bash
docker-compose down
```

---

## 🚀 Execução Local

### Terminal 1: Customer Service

```bash
cd src/Customer.Service/Customer.API
dotnet run
# Acessa: http://localhost:5001/swagger
```

### Terminal 2: Banking Core Service

```bash
cd src/Core.API
# Configurar URL do Customer Service
export CUSTOMER_SERVICE_URL=http://localhost:5001
dotnet run
# Acessa: http://localhost:5000/swagger
```

---

## 📊 Banco de Dados

### Customer Service (SQLite)

**Tabela: Customers**
```sql
CREATE TABLE Customers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CPF VARCHAR(11) UNIQUE NOT NULL,
    Name VARCHAR(MAX) NOT NULL,
    Email VARCHAR(MAX) NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME,
    IsActive BIT NOT NULL
);
```

### Banking Core Service (SQLite)

**Tabelas**:
- `BankAccounts` - Contas bancárias
- `Transactions` - Histórico de transações

---

## 🏗️ Diagrama de Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                    Clientes (API)                       │
└──────────────┬──────────────────────────────┬───────────┘
               │                              │
               ▼                              ▼
    ┌──────────────────┐          ┌──────────────────────┐
    │ Customer Service │          │ Banking Core Service │
    │   (Porta 5001)   │          │   (Porta 5000)       │
    ├──────────────────┤          ├──────────────────────┤
    │ • Clientes       │  HTTP    │ • Contas             │
    │ • Validação CPF  │◄────────►│ • Transferências     │
    │ • Contato        │          │ • Transações         │
    └──────────────────┘          └──────────────────────┘
           │                              │
           ▼                              ▼
    ┌──────────────────┐          ┌──────────────────────┐
    │  customer.db     │          │    banking.db        │
    │    (SQLite)      │          │    (SQLite)          │
    └──────────────────┘          └──────────────────────┘
```

---

## ✅ Benefícios da Arquitetura

| Benefício | Descrição |
|-----------|-----------|
| **Independência** | Cada serviço pode ser desenvolvido, testado e deployado isoladamente |
| **Escalabilidade** | Escalar Customer Service sem afetar Banking Core |
| **Resiliência** | Falha em um serviço não derruba o outro |
| **Flexibilidade** | Trocar tecnologia de um serviço sem afetar outros |
| **Manutenibilidade** | Código organizado e responsabilidades claras |
| **Equipes Autônomas** | Teams podem trabalhar em serviços diferentes |

---

## ⚠️ Desafios e Soluções

### 1. Consistência de Dados

**Problema**: Dados duplicados entre serviços (Customer está em ambos)

**Solução**: 
- Remove Customer do Core.Domain (deprecado)
- Core.Domain referencia apenas Customer via DTOs
- Única fonte da verdade: Customer Service

### 2. Latência de Comunicação

**Problema**: HTTP síncrono pode ser lento

**Soluções Futuras**:
- Cache local no Banking Core
- Message Queue (RabbitMQ, Azure Service Bus)
- Event Sourcing

### 3. Tratamento de Falhas

**Problema**: E se Customer Service cair?

**Soluções**:
- Circuit Breaker (Polly)
- Retry com backoff exponencial
- Fallback aos dados em cache

---

## 🔧 Configuração para SQL Server

### Customer Service

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CustomerService;Integrated Security=true;"
  },
  "DatabaseSettings": {
    "Type": "sqlserver"
  }
}
```

### Banking Core

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankingCore;Integrated Security=true;"
  },
  "DatabaseSettings": {
    "Type": "sqlserver"
  }
}
```

Executar:
```bash
export DATABASE_TYPE=sqlserver
dotnet run
```

---

## 📈 Próximos Passos

1. ✅ **Message Queue** - Implementar async com RabbitMQ
2. ✅ **API Gateway** - Adicionar Kong ou Ocelot
3. ✅ **Service Discovery** - Consul ou Eureka
4. ✅ **Circuit Breaker** - Implementar com Polly
5. ✅ **Observabilidade** - ELK Stack ou Application Insights
6. ✅ **Autenticação/Autorização** - OAuth2 com Identity Server
7. ✅ **Testes de Integração** - TestContainers

---

## 📚 Documentação Referenciada

- [Shared DTOs](../src/Shared/Shared.DTOs)
- [Customer Service Client](../src/Shared/Shared.HttpClients)
- [Docker Compose](../docker-compose.yml)
- [Dockerfiles](../src/Customer.Service/Customer.API/Dockerfile)

---

## 🎯 Conclusão

Parabéns! 🎉 Você agora tem uma **arquitetura de microserviços profissional** com:
- ✅ Dois serviços independentes
- ✅ Comunicação HTTP
- ✅ Bancos de dados isolados
- ✅ Docker Compose para orquestração
- ✅ DTOs compartilhadas
- ✅ Cliente HTTP para inter-serviços

Próximo passo: Implementar comunicação assíncrona com Message Queue!
