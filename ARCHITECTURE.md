# Arquitetura Hexagonal - Banking Core Microservice

## 📐 Visão Geral da Arquitetura Hexagonal (Ports & Adapters)

A Arquitetura Hexagonal desacopla a lógica de negócio das dependências externas através de interfaces (Portas) e implementações (Adaptadores).

```
┌─────────────────────────────────────────────────────────┐
│                 CAMADA DE APRESENTAÇÃO                  │
│              REST API / Web Controllers                 │
│  POST /customers  │  POST /accounts  │  POST /transfer  │
└───────────────────────────────────┬─────────────────────┘
                                    │
                    [PORTA - ICustomerRepository]
                    [PORTA - IBankAccountRepository]
                    [PORTA - INotificationService]
                                    │
┌───────────────────────────────────▼─────────────────────┐
│                 CAMADA DE APLICAÇÃO                     │
│     Use Cases / Serviços de Aplicação                   │
│  • CreateCustomerUseCase                                │
│  • CreateBankAccountUseCase                             │
│  • TransferUseCase                                      │
└───────────────────────────────────┬─────────────────────┘
                                    │
                        [PORTAS DO DOMÍNIO]
                                    │
┌───────────────────────────────────▼─────────────────────┐
│                 CAMADA DE DOMÍNIO                       │
│     Entidades, Value Objects, Lógica de Negócio        │
│  • Customer (Agregado)                                  │
│  • BankAccount (Agregado)                               │
│  • Transaction (Entidade)                               │
│  • Money, CPF (Value Objects)                           │
└───────────────────────────────────┬─────────────────────┘
                                    │
                    [PORTA - ICustomerRepository]
                    [PORTA - IBankAccountRepository]
                    [PORTA - INotificationService]
                                    │
┌───────────────────────────────────▼─────────────────────┐
│               CAMADA DE INFRAESTRUTURA                  │
│              Adaptadores e Implementações               │
│  • CustomerRepository (EF Core)                         │
│  • BankAccountRepository (EF Core)                      │
│  • EmailNotificationService                             │
│  • BankingContext (DbContext)                           │
└─────────────────────────────────────────────────────────┘
        │
        └─────────► SQL Server / LocalDB
```

## 🎯 Principais Conceitos

### 1. **Agregados** (Root Aggregates)
```
Customer (Agregado Raiz)
├── CPF (Value Object)
├── Name (String)
├── Email (String)
└── PhoneNumber (String)

BankAccount (Agregado Raiz)
├── AccountNumber (String)
├── Branch (String)
├── Balance (Money - Value Object)
├── Transactions[] (Entidades)
└── Money.Currency (String)
```

### 2. **Portas** (Interfaces)
Definem contratos que os adaptadores devem implementar:
- `ICustomerRepository` - Persistência de clientes
- `IBankAccountRepository` - Persistência de contas
- `INotificationService` - Envio de notificações

### 3. **Adaptadores** (Implementações)
Implementam as portas com tecnologias específicas:
- `CustomerRepository` - Entity Framework Core
- `BankAccountRepository` - Entity Framework Core
- `EmailNotificationService` - Logging (exemplo)

### 4. **Casos de Uso** (Application Services)
Orquestram a lógica de negócio:
- `CreateCustomerUseCase` - Cadastro de cliente (KYC)
- `CreateBankAccountUseCase` - Abertura de conta
- `TransferUseCase` - Transferências (TED/PIX)

## 🔄 Fluxo de Uma Transferência

```
1. Request HTTP POST /accounts/transfer
   ├─ FromAccountNumber: "1234567890"
   ├─ ToAccountNumber: "9876543210"
   └─ Amount: 150.00

2. AccountsController.Transfer()
   └─> Chama TransferUseCase

3. TransferUseCase.Execute()
   ├─> Busca contas via IBankAccountRepository
   ├─> Valida se contas existem
   └─> Chama BankAccount.Transfer() [LÓGICA DE DOMÍNIO]

4. BankAccount.Transfer()
   ├─> Valida se contas ativas
   ├─> Valida se há saldo
   ├─> Subtrai saldo da origem
   ├─> Adiciona saldo no destino
   └─> Cria transações

5. Repository.Update() (Adaptador)
   ├─> Persiste em BankingContext (EF Core)
   └─> Salva no banco de dados

6. INotificationService.SendTransferConfirmation()
   └─> Envia notificação de confirmação

7. Response HTTP 204 No Content
```

## 💾 Value Objects (Invariantes)

### Money
```csharp
var money = new Money(100.50m, "BRL");
money.Add(new Money(50, "BRL"));        // OK
money.Add(new Money(50, "USD"));        // ERRO - Moedas diferentes
```

### CPF
```csharp
var cpf = new CPF("123.456.789-09");    // OK
var cpf = new CPF("123.456.789-00");    // ERRO - Inválido
```

## 🧪 Testes

### Unit Tests (Domínio)
```csharp
[Fact]
public void Transfer_ShouldUpdateBothAccounts()
{
    var account1 = new BankAccount(customerId1);
    var account2 = new BankAccount(customerId2);
    
    account1.Deposit(100);
    account1.Transfer(account2, 30);
    
    Assert.Equal(70, account1.Balance.Amount);
    Assert.Equal(30, account2.Balance.Amount);
}
```

## 🔐 Benefícios da Arquitetura Hexagonal

✅ **Testabilidade**: Fácil mockar as portas
✅ **Independência**: Domínio não depende de frameworks
✅ **Flexibilidade**: Trocar implementações facilmente
✅ **Clareza**: Separação clara de responsabilidades
✅ **Manutenibilidade**: Código organizado e coeso

## 🚀 Próximas Implementações

- [ ] Autenticação (JWT)
- [ ] Autorização (RBAC)
- [ ] CQRS Pattern para separar leitura/escrita
- [ ] Event Sourcing
- [ ] Saga Pattern para transações distribuídas
- [ ] Circuit Breaker
- [ ] Métricas (Prometheus)
- [ ] Observabilidade (Jaeger)
- [ ] Containerização (Docker)
- [ ] Kubernetes manifests

---

**Versão**: 1.0.0 | **Arquiteto**: Referência de Projeto | **Data**: 2026-03-23
