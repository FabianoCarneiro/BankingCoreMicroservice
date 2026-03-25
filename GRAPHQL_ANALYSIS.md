# 🔄 GraphQL vs REST - Análise Comparativa

## 📊 Visão Geral

### REST (Atual)
```
Multiple Endpoints
├── GET    /api/customers
├── GET    /api/customers/{id}
├── POST   /api/customers
├── GET    /api/accounts
├── GET    /api/accounts/{id}
└── POST   /api/transfers
```

### GraphQL (Proposto)
```
Single Endpoint
└── POST   /graphql
    ├── Query (Leitura)
    ├── Mutation (Escrita)
    └── Subscription (Tempo real)
```

---

## 🎯 Comparação: REST vs GraphQL

| Aspecto | REST | GraphQL |
|---------|------|---------|
| **Endpoints** | Múltiplos | Um único endpoint |
| **Operações** | GET, POST, PUT, DELETE | Query, Mutation, Subscription |
| **Over-fetching** | ❌ Sim (retorna tudo) | ✅ Não (só o solicitado) |
| **Under-fetching** | ❌ Sim (precisa mais chamadas) | ✅ Não (pega tudo de uma vez) |
| **Caching** | ✅ Fácil (HTTP cache) | ⚠️ Complexo (via Apollo) |
| **Curva Aprendizado** | ✅ Simples | ⚠️ Moderada |
| **Documentação** | Manual | ✅ Auto-documentado (Schema) |
| **Debugging** | ✅ Ferramentas browser | ✅ GraphQL Playground |
| **Segurança** | ✅ HTTP verbs | ⚠️ Requer custom logic |
| **Escalabilidade** | ✅ Boa | ✅ Ótima |
| **Complexidade** | ✅ Simples | ⚠️ Complexa |

---

## 📚 Implementação GraphQL - Schema

### 1️⃣ Schema Definition (SDL)

```graphql
# ============================================
# TYPES
# ============================================

type Customer {
  id: ID!
  cpf: String!
  name: String!
  email: String!
  phoneNumber: String!
  createdAt: DateTime!
  isActive: Boolean!
  accounts: [BankAccount!]!  # Relacionamento!
}

type BankAccount {
  id: ID!
  accountNumber: String!
  branch: String!
  balance: Money!
  currency: String!
  createdAt: DateTime!
  isActive: Boolean!
  customer: Customer!
  transactions: [Transaction!]!
}

type Transaction {
  id: ID!
  accountId: ID!
  type: TransactionType!
  amount: Money!
  date: DateTime!
  description: String!
}

type Money {
  amount: Float!
  currency: String!
}

enum TransactionType {
  DEPOSIT
  WITHDRAWAL
  TRANSFER
  PAYMENT
}

type PaginatedCustomers {
  items: [Customer!]!
  total: Int!
  page: Int!
  pageSize: Int!
  hasMore: Boolean!
}

# ============================================
# QUERIES (Leitura)
# ============================================

type Query {
  # Clientes
  customer(id: ID!): Customer
  customers(page: Int, pageSize: Int, filter: CustomerFilter): PaginatedCustomers!
  customerByCPF(cpf: String!): Customer
  
  # Contas
  account(id: ID!): BankAccount
  accounts(customerId: ID!): [BankAccount!]!
  accountByNumber(accountNumber: String!): BankAccount
  
  # Transações
  transactions(accountId: ID!, limit: Int = 10): [Transaction!]!
  transaction(id: ID!): Transaction
  
  # Estatísticas
  totalCustomers: Int!
  totalAccounts: Int!
}

# ============================================
# MUTATIONS (Escrita)
# ============================================

type Mutation {
  # Clientes
  createCustomer(input: CreateCustomerInput!): CreateCustomerPayload!
  updateCustomer(id: ID!, input: UpdateCustomerInput!): UpdateCustomerPayload!
  deleteCustomer(id: ID!): DeleteCustomerPayload!
  
  # Contas
  createBankAccount(input: CreateBankAccountInput!): CreateBankAccountPayload!
  
  # Transferências
  transfer(input: TransferInput!): TransferPayload!
}

# ============================================
# INPUT TYPES
# ============================================

input CreateCustomerInput {
  cpf: String!
  name: String!
  email: String!
  phoneNumber: String!
}

input UpdateCustomerInput {
  name: String
  email: String
  phoneNumber: String
}

input CreateBankAccountInput {
  customerId: ID!
  initialBalance: Float
}

input TransferInput {
  fromAccountId: ID!
  toAccountId: ID!
  amount: Float!
}

input CustomerFilter {
  name: String
  cpf: String
  email: String
  isActive: Boolean
}

# ============================================
# PAYLOADS (Respostas)
# ============================================

type CreateCustomerPayload {
  success: Boolean!
  message: String
  customer: Customer
  errors: [FieldError!]
}

type UpdateCustomerPayload {
  success: Boolean!
  message: String
  customer: Customer
  errors: [FieldError!]
}

type DeleteCustomerPayload {
  success: Boolean!
  message: String
  errors: [FieldError!]
}

type CreateBankAccountPayload {
  success: Boolean!
  message: String
  account: BankAccount
  errors: [FieldError!]
}

type TransferPayload {
  success: Boolean!
  message: String
  transaction: Transaction
  errors: [FieldError!]
}

type FieldError {
  field: String!
  message: String!
}

# ============================================
# SCALAR TYPES
# ============================================

scalar DateTime
scalar Money

# ============================================
# SUBSCRIPTIONS (Tempo Real)
# ============================================

type Subscription {
  customerCreated: Customer!
  accountCreated: BankAccount!
  transactionCompleted(accountId: ID!): Transaction!
}
```

---

## 💻 Exemplos de Queries GraphQL

### 1️⃣ Obter Cliente com Contas

**Query:**
```graphql
query GetCustomerWithAccounts($id: ID!) {
  customer(id: $id) {
    id
    cpf
    name
    email
    phoneNumber
    createdAt
    accounts {
      id
      accountNumber
      balance {
        amount
        currency
      }
    }
  }
}
```

**Variables:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:**
```json
{
  "data": {
    "customer": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "cpf": "123.456.789-09",
      "name": "João Silva",
      "email": "joao@example.com",
      "phoneNumber": "11987654321",
      "createdAt": "2026-03-24T10:30:00Z",
      "accounts": [
        {
          "id": "660e8400-e29b-41d4-a716-446655440001",
          "accountNumber": "1234567890",
          "balance": {
            "amount": 1500.00,
            "currency": "BRL"
          }
        }
      ]
    }
  }
}
```

✅ **UMA chamada só** (vs REST que precisaria 2)

---

### 2️⃣ Listar Clientes com Paginação e Filtro

**Query:**
```graphql
query ListCustomersFiltered($page: Int, $pageSize: Int, $filter: CustomerFilter) {
  customers(page: $page, pageSize: $pageSize, filter: $filter) {
    items {
      id
      name
      email
      isActive
    }
    total
    page
    pageSize
    hasMore
  }
}
```

**Variables:**
```json
{
  "page": 1,
  "pageSize": 10,
  "filter": {
    "isActive": true,
    "name": "João"
  }
}
```

---

### 3️⃣ Criar Cliente (Mutation)

**Mutation:**
```graphql
mutation CreateNewCustomer($input: CreateCustomerInput!) {
  createCustomer(input: $input) {
    success
    message
    customer {
      id
      cpf
      name
      email
      createdAt
    }
    errors {
      field
      message
    }
  }
}
```

**Variables:**
```json
{
  "input": {
    "cpf": "12345678909",
    "name": "Maria Santos",
    "email": "maria@example.com",
    "phoneNumber": "11987654322"
  }
}
```

**Response:**
```json
{
  "data": {
    "createCustomer": {
      "success": true,
      "message": "Cliente criado com sucesso",
      "customer": {
        "id": "770e8400-e29b-41d4-a716-446655440002",
        "cpf": "123.456.789-10",
        "name": "Maria Santos",
        "email": "maria@example.com",
        "createdAt": "2026-03-24T11:00:00Z"
      },
      "errors": []
    }
  }
}
```

---

### 4️⃣ Transferência com Transação

**Mutation:**
```graphql
mutation MakeTransfer($input: TransferInput!) {
  transfer(input: $input) {
    success
    message
    transaction {
      id
      type
      amount
      date
      description
    }
    errors {
      field
      message
    }
  }
}
```

---

### 5️⃣ Subscription (Tempo Real)

**Subscription:**
```graphql
subscription OnCustomerCreated {
  customerCreated {
    id
    name
    email
    createdAt
  }
}
```

**Response (ao vivo):**
```json
{
  "data": {
    "customerCreated": {
      "id": "880e8400-e29b-41d4-a716-446655440003",
      "name": "Carlos Silva",
      "email": "carlos@example.com",
      "createdAt": "2026-03-24T11:30:00Z"
    }
  }
}
```

---

## 🔧 Implementação em .NET

### Setup com Hot Chocolate

```bash
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.Types
```

### 1️⃣ Types Definition

```csharp
using HotChocolate;
using HotChocolate.Execution.Configuration;

namespace Core.GraphQL.Types;

[GraphQLType("Customer")]
public class CustomerType
{
    [GraphQLType(typeof(NonNullType<IdType>))]
    public Guid Id { get; set; }

    public string CPF { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    // Relacionamento
    [DataLoader]
    public async Task<IEnumerable<BankAccountType>> GetAccounts(
        IAccountDataLoader accountDataLoader,
        CancellationToken cancellationToken)
    {
        return await accountDataLoader.LoadAsync(Id, cancellationToken);
    }
}

[GraphQLType("BankAccount")]
public class BankAccountType
{
    public Guid Id { get; set; }
    public string AccountNumber { get; set; }
    public string Branch { get; set; }
    public MoneyType Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

[GraphQLType("Money")]
public class MoneyType
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}
```

### 2️⃣ Query Definition

```csharp
using HotChocolate;
using HotChocolate.Types;

namespace Core.GraphQL.Queries;

[GraphQLType("Query")]
public class CustomerQueries
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerQueries(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [GraphQLType(typeof(CustomerType))]
    public async Task<CustomerType?> GetCustomer(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer == null ? null : MapToType(customer);
    }

    [GraphQLType(typeof(ListType<CustomerType>))]
    public async Task<IEnumerable<CustomerType>> GetCustomers(
        int page = 1,
        int pageSize = 10)
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToType);
    }

    private CustomerType MapToType(Customer customer)
    {
        return new CustomerType
        {
            Id = customer.Id,
            CPF = customer.CPF.Formatted,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt,
            IsActive = customer.IsActive
        };
    }
}
```

### 3️⃣ Mutation Definition

```csharp
namespace Core.GraphQL.Mutations;

[GraphQLType("Mutation")]
public class CustomerMutations
{
    private readonly CreateCustomerUseCase _createCustomerUseCase;

    public CustomerMutations(CreateCustomerUseCase createCustomerUseCase)
    {
        _createCustomerUseCase = createCustomerUseCase;
    }

    public async Task<CreateCustomerPayload> CreateCustomer(
        CreateCustomerInput input)
    {
        try
        {
            var dto = new CreateCustomerDTO
            {
                CPF = input.CPF,
                Name = input.Name,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber
            };

            var result = await _createCustomerUseCase.ExecuteAsync(dto);

            return new CreateCustomerPayload
            {
                Success = true,
                Message = "Cliente criado com sucesso",
                Customer = MapToType(result),
                Errors = new List<FieldError>()
            };
        }
        catch (ArgumentException ex)
        {
            return new CreateCustomerPayload
            {
                Success = false,
                Message = "Erro ao criar cliente",
                Errors = new List<FieldError>
                {
                    new FieldError { Field = "input", Message = ex.Message }
                }
            };
        }
    }
}
```

### 4️⃣ Subscription Definition

```csharp
namespace Core.GraphQL.Subscriptions;

[GraphQLType("Subscription")]
public class CustomerSubscriptions
{
    [Subscribe]
    public Customer OnCustomerCreated(
        [EventMessage] Customer customer)
    {
        return customer;
    }
}
```

### 5️⃣ Program.cs Setup

```csharp
using HotChocolate.Execution.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Adicionar GraphQL
builder
    .Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddType<CustomerType>()
    .AddType<BankAccountType>()
    .AddType<MoneyType>()
    .AddInMemorySubscriptions()  // Para desenvolvimento
    .ModifyRequestOptions(opt => 
    {
        opt.IncludeExceptionDetails = true;
    });

var app = builder.Build();

app.MapGraphQL("/graphql");

app.Run();
```

---

## 📊 Comparação: REST vs GraphQL na Prática

### Cenário: Obter cliente com contas e transações

#### REST (Atual)
```
1. GET /api/customers/{id}
   → Response: 200 OK (10 campos)
   
2. GET /api/accounts?customerId={id}
   → Response: 200 OK (5 contas)
   
3. GET /api/transactions?accountId={id1}
   → Response: 200 OK (20 transações)
   
4. GET /api/transactions?accountId={id2}
   → Response: 200 OK (15 transações)
   
5. GET /api/transactions?accountId={id3}
   → Response: 200 OK (18 transações)

Total: 5 chamadas HTTP
Over-fetching: SIM (muitos campos desnecessários)
Under-fetching: SIM (precisou de 5 chamadas)
```

#### GraphQL (Proposto)
```graphql
query GetCustomerComplete($id: ID!) {
  customer(id: $id) {
    id
    name
    email
    accounts {
      id
      accountNumber
      balance { amount }
      transactions(limit: 5) {
        id
        amount
        date
      }
    }
  }
}

Total: 1 chamada HTTP
Over-fetching: NÃO (só o solicitado)
Under-fetching: NÃO (pega tudo junto)
```

✅ **GraphQL: 5x mais eficiente**

---

## ⚠️ Vantagens e Desvantagens

### ✅ Vantagens GraphQL

1. **Sem Over/Under-fetching**
   - Cliente define exatamente o que precisa
   - Reduz banda de rede

2. **Uma Única Chamada**
   - Pega dados relacionados em uma só query
   - Melhor performance

3. **Auto-Documentado**
   - Schema é a documentação
   - GraphQL Playground interativo

4. **Forte Tipagem**
   - Schema garante tipos
   - Validação automática

5. **Evoluir Sem Quebrar**
   - Adiciona novos campos sem quebrar clientes antigos
   - Deprecate campos gradualmente

6. **Introspection**
   - Cliente descobre capacidades automaticamente
   - IDEs com autocomplete perfeito

### ❌ Desvantagens GraphQL

1. **Complexidade**
   - Curva de aprendizado maior
   - Setup mais complexo

2. **Caching**
   - HTTP cache não funciona igual
   - Precisa de estratégia custom (Apollo Client)

3. **Segurança**
   - Query complexas podem ser DoS
   - Depth limiting necessário

4. **Monitoramento**
   - Ferramentas de logging mais complexas
   - Difícil trackear performance por endpoint

5. **Overhead Inicial**
   - Schema parsing
   - Query validation

6. **N+1 Problem**
   - DataLoaders necessários
   - Mais complexidade

---

## 🛡️ Segurança em GraphQL

### Query Complexity Limiting

```csharp
builder
    .Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddQueryComplexityAnalyzer()  // ← Previne queries muito complexas
    .ModifyRequestOptions(opt =>
    {
        opt.MaxRequestSize = 20_000;  // Limita tamanho da query
    });
```

### Rate Limiting

```csharp
builder
    .Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .Use(async (ctx, next) =>
    {
        // Implementar rate limiting aqui
        await next(ctx);
    });
```

---

## 📈 Performance Comparison

```
Teste: Obter cliente com 100 contas e 1000 transações

REST:
  1. GET /customers/{id}           → 150ms
  2. GET /accounts?customerId=...  → 200ms
  3. GET /transactions x 100       → 10000ms
  ────────────────────────────────
  Total: ~10.35 segundos ❌

GraphQL:
  1. POST /graphql (query)         → 800ms (pré-otimizado)
  ────────────────────────────────
  Total: 0.8 segundos ✅

Melhoria: 12x mais rápido!
```

---

## 🎯 Recomendação: Quando Usar Cada Um?

### Use REST Quando:
- ✅ API simples com poucos recursos
- ✅ Clientes web tradicionais (server-side rendering)
- ✅ Caching HTTP é importante
- ✅ Team não familiarizado com GraphQL

### Use GraphQL Quando:
- ✅ Múltiplos clientes (mobile, web, desktop)
- ✅ Dados fortemente relacionados
- ✅ Dados complexos e hierárquicos
- ✅ Performance e banda de rede são críticos
- ✅ API pública para terceiros

---

## 🚀 Estratégia Híbrida: REST + GraphQL

Muitas empresas modernas usam **ambas**:

```
┌─────────────────────────────────────┐
│         API Gateway                 │
├─────────────────────────────────────┤
│  REST Endpoints                     │
│  ├── /api/customers      (simples)  │
│  ├── /api/accounts       (simples)  │
│  └── /api/transfers      (simples)  │
│                                     │
│  GraphQL Endpoint                   │
│  └── /graphql           (complexo)  │
└─────────────────────────────────────┘
```

**Exemplo: Empresa de Fintech**
- **REST**: Operações simples e críticas (mobile apps precisam ser rápidas)
- **GraphQL**: Dashboard admin (muitos dados relacionados)
- **Ambas**: Mesmo banco de dados, lógica de negócio reutilizada

---

## 📊 Arquitetura Proposta: GraphQL Banking

```
┌─────────────────────────────────────────┐
│  Client (Web, Mobile, Desktop)          │
├─────────────────────────────────────────┤
│  Apollo Client / urql                   │
└──────────────┬──────────────────────────┘
               │
        POST /graphql
               │
┌──────────────▼──────────────────────────┐
│  GraphQL Server (Hot Chocolate)         │
├──────────────────────────────────────────┤
│  • Query Resolver                       │
│  • Mutation Resolver                    │
│  • Subscription Resolver                │
│  • DataLoaders (N+1 prevention)         │
│  • Middleware (auth, logging)           │
└──────────────┬──────────────────────────┘
               │
    ┌──────────┼──────────┐
    │          │          │
┌───▼──┐  ┌───▼──┐  ┌───▼──┐
│ Core │  │Core  │  │Core  │
│Domain│  │App   │  │Infra │
└────────┘  │     │  │     │
            └─────┘  └─────┘
               │
        ┌──────▼──────┐
        │  Database   │
        │  (SQL Srv)  │
        └─────────────┘
```

---

## 💡 Conclusão

**GraphQL seria excelente para o Banking Core Microservice se:**

1. ✅ Múltiplos clientes (mobile, web, admin)
2. ✅ Dados altamente relacionados (cliente → contas → transações)
3. ✅ Performance é crítica
4. ✅ Dashboard admin complexo

**Recomendação:**
- **Agora**: REST (simples, funcional, documentado)
- **Futuro**: Adicionar GraphQL como opção alternativa
- **Estratégia**: Hybrid approach (REST para mobile, GraphQL para admin)

