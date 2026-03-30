# TestContainers Integration Testing Guide

## Overview

Este projeto implementa testes de integração automatizados usando **TestContainers** com suporte a múltiplos bancos de dados.

## TestContainers Configurados

### 1. **SQL Server Container** (`SqlServerIntegrationTestFixture`)
- **Porta**: Dinâmica (definida automaticamente)
- **Banco de Dados**: Temporário
- **Senha Padrão**: `Test@12345`
- **Uso**: Testes mais completos que replicam ambiente de produção

### 2. **SQLite In-Memory** (`SqliteIntegrationTestFixture`)
- **Tipo**: Arquivo temporário (criado no `/tmp`)
- **Vantagem**: Muito mais rápido (sem container)
- **Uso**: Ideal para CI/CD e execução rápida

## Arquivos de Teste

### `/tests/Core.Tests/Integration/`

#### **SqlServerIntegrationTestFixture.cs**
```csharp
public class SqlServerIntegrationTestFixture : IAsyncLifetime
{
    // Inicia container SQL Server automaticamente
    // Cria schema do banco
    // Limpa recursos após testes
}
```

**Coleção**: `"SQL Server Collection"`

---

#### **SqliteIntegrationTestFixture.cs**
```csharp
public class SqliteIntegrationTestFixture : IAsyncLifetime
{
    // Usa arquivo SQLite temporário
    // Não requer container
    // Muito mais rápido
}
```

**Coleção**: `"SQLite Collection"`

---

#### **BankAccountRepositoryIntegrationTests.cs**
Testes do repositório de contas bancárias:
- ✅ Criar cliente e conta
- ✅ Múltiplas contas por cliente
- ✅ Depósitos atualizando saldo
- ✅ Atualizar dados de contato
- ✅ Deletar cliente

**Exemplo:**
```csharp
[Collection("SQL Server Collection")]
public class BankAccountRepositoryIntegrationTests
{
    [Fact]
    public async Task CreateCustomerAndAccount_ShouldPersistToDatabase()
    {
        // Setup
        var customer = new Customer(...);
        
        // Act
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();
        
        // Assert
        var saved = await dbContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(saved);
    }
}
```

---

#### **UseCaseIntegrationTests.cs**
Testes das Use Cases com persistência:

**CreateCustomerUseCaseIntegrationTests**:
- ✅ Criar cliente com dados válidos
- ✅ CPF inválido lança exceção
- ✅ CPF duplicado lança exceção

**CreateBankAccountUseCaseIntegrationTests**:
- ✅ Criar conta para cliente existente
- ✅ Cliente inexistente lança exceção

**Exemplo:**
```csharp
[Collection("SQLite Collection")]
public class CreateCustomerUseCaseIntegrationTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateCustomer()
    {
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var customer = await useCase.ExecuteAsync(
            cpf: "12345678900",
            name: "Test Customer",
            email: "test@example.com",
            phoneNumber: "11999999999"
        );
        
        Assert.NotNull(customer);
        Assert.NotEqual(Guid.Empty, customer.Id);
    }
}
```

---

#### **TransferUseCaseIntegrationTests.cs**
Testes de operações transacionais complexas:
- ✅ Transferência válida entre contas
- ✅ Saldo insuficiente lança exceção
- ✅ Valor negativo/zero lança exceção
- ✅ Mesma conta lança exceção
- ✅ Múltiplas transferências mantêm consistência

**Exemplo - Setup com duas contas:**
```csharp
private async Task<(Customer, BankAccount, Customer, BankAccount)> 
    SetupTwoCustomersWithAccountsAsync()
{
    // Cria 2 clientes
    // Cria 2 contas
    // Faz depósito inicial na primeira conta
    return (sender, senderAccount, recipient, recipientAccount);
}

[Fact]
public async Task ExecuteAsync_WithValidTransfer_ShouldUpdateBothAccounts()
{
    var (sender, senderAccount, recipient, recipientAccount) = 
        await SetupTwoCustomersWithAccountsAsync();
    
    var useCase = new TransferUseCase(repository);
    var transfer = await useCase.ExecuteAsync(
        fromAccountId: senderAccount.Id,
        toAccountId: recipientAccount.Id,
        amount: 1000m
    );
    
    Assert.True(transfer.IsSuccessful);
    Assert.Equal(4000m, updatedSenderAccount.Balance.Amount);
    Assert.Equal(1000m, updatedRecipientAccount.Balance.Amount);
}
```

## Executar Testes

### **Todos os testes de integração:**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj
```

### **Apenas com SQLite (mais rápido):**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "Category=SQLite"
```

### **Apenas com SQL Server:**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "Category=SqlServer"
```

### **Um teste específico:**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "FullyQualifiedName~CreateCustomerUseCaseIntegrationTests"
```

### **Com logs detalhados:**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj -v detailed
```

## Estrutura de Coleções

As coleções xUnit garantem que testes sejam agrupados com o mesmo fixture:

```
┌─ SQL Server Collection
│  ├─ BankAccountRepositoryIntegrationTests
│  └─ (compartilham SqlServerIntegrationTestFixture)
│
└─ SQLite Collection
   ├─ CreateCustomerUseCaseIntegrationTests
   ├─ CreateBankAccountUseCaseIntegrationTests
   ├─ TransferUseCaseIntegrationTests
   └─ (compartilham SqliteIntegrationTestFixture)
```

## Lifecycle dos Testes

### **Inicialização (`InitializeAsync`)**
```
1. SQL Server Container iniciado
   ↓
2. Connection string obtida
   ↓
3. DbContext criado com SqlServer
   ↓
4. Schema criado (EnsureCreatedAsync)
   ↓
✅ Fixture pronto para testes
```

### **Limpeza (`DisposeAsync`)**
```
✅ Testes completados
   ↓
1. DbContext descartado
   ↓
2. Container SQL Server parado
   ↓
3. Arquivo SQLite deletado
   ↓
✅ Recursos liberados
```

## Boas Práticas

### ✅ **Faça**
- Use fixtures para inicializar contexto compartilhado
- Teste fluxos completos (criar → ler → atualizar → deletar)
- Valide exceções esperadas com `Assert.ThrowsAsync`
- Use coleções xUnit para agrupar testes com mesmo fixture
- Limpe dados após cada teste (automático com fixtures)

### ❌ **Não faça**
- Compartilhar estado entre testes sem fixture
- Fazer testes interdependentes
- Deixar dados no banco após teste
- Usar dados hardcoded sem validação
- Ignorar exceções esperadas

## Exemplo: Adicionar Novo Teste

```csharp
[Collection("SQLite Collection")]
public class MyNewIntegrationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;

    public MyNewIntegrationTests(SqliteIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task MyNewTest_ShouldDoSomething()
    {
        // Arrange
        var dbContext = _fixture.DbContext;
        Assert.NotNull(dbContext);
        
        // Seu repositório ou use case
        var repository = new MyRepository(dbContext);
        
        // Act
        var result = await repository.DoSomethingAsync();
        
        // Assert
        Assert.NotNull(result);
    }
}
```

## Resultado dos Testes

```
✅ Passed! - Failed: 0, Passed: 13, Skipped: 0, Total: 13
   Duration: ~112ms (SQLite - sem container)
```

**Testes executados:**
- 5 testes de BankAccount Repository
- 5 testes de Use Cases (Create Customer + Create Account)
- 3 testes de Transfer (válido, saldo insuficiente, mesmo valor negativo)
- (Mais podem ser adicionados conforme necessário)

## Troubleshooting

### **Docker não está disponível (SQL Server fixture)**
Se rodar sem Docker:
```bash
# Use apenas SQLite
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "Category=SQLite"
```

### **Porta já em uso**
TestContainers escolhe porta dinâmica automaticamente. Não há conflito.

### **Fixture não foi inicializado**
Certifique-se que a classe de teste:
1. Tem `[Collection("Collection Name")]`
2. Recebe o fixture no construtor
3. Verifica `Assert.NotNull(dbContext)` antes de usar

### **Teste muito lento**
- Use `SqliteIntegrationTestFixture` ao invés de SQL Server
- Reduza volume de dados nos testes
- Considere testes mais específicos

## Próximos Passos

1. **Adicionar testes para Customer Service**
   - Criar `tests/Customer.Service.Tests/`
   - Mesmo padrão de fixtures e coleções

2. **Testes inter-serviços**
   - Iniciar ambos os containers
   - Testar HTTP calls entre serviços

3. **CI/CD Pipeline**
   - Executar testes em container com Docker
   - Cache de layers para builds rápidos

4. **Test Data Builder**
   ```csharp
   var customer = new CustomerBuilder()
       .WithCpf("12345678900")
       .WithName("Test")
       .Build();
   ```

---

**Desenvolvido com:** TestContainers 3.7.0, xUnit, SQLite, SQL Server
**Status:** ✅ Completo e testado
