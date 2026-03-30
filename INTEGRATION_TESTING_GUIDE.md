# How to Add More Integration Tests

Este guia mostra como adicionar novos testes de integração ao projeto.

## Template Básico

```csharp
using Core.Domain.Entities;
using Core.Domain.Ports;
using Core.Infrastructure.Adapters;
using Core.Infrastructure.Persistence;
using Core.Tests.Helpers;
using Xunit;

namespace Core.Tests.Integration;

/// <summary>
/// Descrição dos testes
/// </summary>
[Collection("SQLite Collection")]
public class MyNewIntegrationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;

    public MyNewIntegrationTests(SqliteIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task MyTest_ShouldDoSomething()
    {
        // Arrange
        var dbContext = _fixture.DbContext;
        Assert.NotNull(dbContext);
        
        var repository = new MyRepository(dbContext);
        
        // Act
        var result = await repository.DoSomethingAsync();
        
        // Assert
        Assert.NotNull(result);
    }
}
```

## Passos para Adicionar Testes

### 1. Criar Arquivo de Teste
```bash
touch tests/Core.Tests/Integration/MyFeatureIntegrationTests.cs
```

### 2. Implementar Classe
```csharp
[Collection("SQLite Collection")]
public class MyFeatureIntegrationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;
    
    public MyFeatureIntegrationTests(SqliteIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }
}
```

### 3. Adicionar Testes
```csharp
[Fact]
public async Task FeatureName_WithCondition_ExpectedResult()
{
    // Seu teste aqui
}
```

### 4. Rodar Testes
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj
```

## Exemplos de Testes

### Exemplo 1: Teste de Repositório

```csharp
[Fact]
public async Task AddAsync_WithValidEntity_ShouldPersist()
{
    // Arrange
    var dbContext = _fixture.DbContext;
    var repository = new MyRepository(dbContext);
    var entity = new MyEntity { Name = "Test" };
    
    // Act
    await repository.AddAsync(entity);
    await dbContext.SaveChangesAsync();
    
    // Assert
    var saved = await dbContext.MyEntities.FindAsync(entity.Id);
    Assert.NotNull(saved);
    Assert.Equal("Test", saved.Name);
}
```

### Exemplo 2: Teste de Use Case

```csharp
[Fact]
public async Task ExecuteAsync_WithValidInput_ShouldReturnExpectedResult()
{
    // Arrange
    var repository = new MyRepository(_fixture.DbContext);
    var useCase = new MyUseCase(repository);
    var input = new MyUseCaseInput { /* ... */ };
    
    // Act
    var result = await useCase.ExecuteAsync(input);
    
    // Assert
    Assert.NotNull(result);
    Assert.True(result.IsSuccessful);
}
```

### Exemplo 3: Teste de Validação

```csharp
[Fact]
public async Task ExecuteAsync_WithInvalidData_ShouldThrowException()
{
    // Arrange
    var repository = new MyRepository(_fixture.DbContext);
    var useCase = new MyUseCase(repository);
    
    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(
        () => useCase.ExecuteAsync(invalidInput)
    );
}
```

### Exemplo 4: Teste de Múltiplas Operações

```csharp
[Fact]
public async Task MultipleOperations_ShouldMaintainConsistency()
{
    // Arrange
    var repository = new MyRepository(_fixture.DbContext);
    
    // Act - Criar
    var entity1 = await repository.AddAsync(new MyEntity { Name = "First" });
    var entity2 = await repository.AddAsync(new MyEntity { Name = "Second" });
    await _fixture.DbContext.SaveChangesAsync();
    
    // Act - Verificar
    var result = await repository.GetAllAsync();
    
    // Assert
    Assert.Equal(2, result.Count());
}
```

## Boas Práticas

### ✅ Faça
```csharp
// ✅ Use o fixture injetado
var dbContext = _fixture.DbContext;

// ✅ Teste um comportamento por teste
[Fact]
public async Task CreateUser_WithValidData_ShouldSucceed()

// ✅ Use nomes descritivos
await Assert.ThrowsAsync<InvalidOperationException>( );

// ✅ Valide persistência
var saved = await dbContext.Entities.FindAsync(id);
Assert.NotNull(saved);
```

### ❌ Não Faça
```csharp
// ❌ Não compartilhe dados entre testes
private static List<MyEntity> _sharedData;

// ❌ Não teste múltiplos comportamentos
[Fact]
public async Task ComplexTestWithManyAssertions()

// ❌ Não ignore exceções
try { /* code */ } catch { }

// ❌ Não use sleeps
await Task.Delay(1000);
```

## Estrutura AAA (Arrange/Act/Assert)

Todos os testes devem seguir este padrão:

```csharp
[Fact]
public async Task TestName_Context_ExpectedBehavior()
{
    // ARRANGE - Setup dos dados de teste
    var repository = new MyRepository(_fixture.DbContext);
    var input = CreateValidInput();
    
    // ACT - Executar a ação
    var result = await repository.ExecuteAsync(input);
    
    // ASSERT - Validar o resultado
    Assert.NotNull(result);
    Assert.True(result.IsValid);
}
```

## Usando Test Data Builders

Para criar dados de teste complexos, use builders:

```csharp
public class MyEntityBuilder
{
    private string _name = "Default";
    
    public MyEntityBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public MyEntity Build()
    {
        return new MyEntity { Name = _name };
    }
}

// Uso:
var entity = new MyEntityBuilder()
    .WithName("Test")
    .Build();
```

## Testando com CPF

Use os CPFs pré-validados:

```csharp
using Core.Tests.Helpers;

var customer = new Customer(
    cpf: CpfGenerator.ValidCpfs.Customer1,  // CPF válido
    name: "Test Customer",
    email: "test@example.com",
    phoneNumber: "11999999999"
);
```

## Checklist para Novos Testes

- [ ] Arquivo criado em `/tests/Core.Tests/Integration/`
- [ ] Classe herda `IAsyncLifetime` ou usa fixture
- [ ] Fixture injetado no construtor
- [ ] Segue padrão AAA (Arrange/Act/Assert)
- [ ] Nome do teste descreve comportamento esperado
- [ ] Testes são independentes (sem estado compartilhado)
- [ ] Todos os testes passam: `dotnet test`
- [ ] Cobertura >80% do código testado
- [ ] Sem warnings ou errors

## Executando Apenas Seus Testes

```bash
# Rodar testes de uma classe específica
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "FullyQualifiedName~MyNewIntegrationTests"

# Rodar apenas um teste
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "Name~MySpecificTestName"

# Rodar com verbose
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --logger "console;verbosity=detailed"
```

## Debugando Testes

### No VS Code
1. Abra o arquivo de teste
2. Clique em "Debug Test" acima do método `[Fact]`
3. Use breakpoints normalmente

### Com `dotnet`
```bash
# Rodar com debug
dotnet test tests/Core.Tests/Core.Tests.csproj --verbosity detailed

# Com logs detalhados
dotnet test tests/Core.Tests/Core.Tests.csproj --logger "console;verbosity=diagnostic"
```

## Próximas Features para Testar

### 1. BankAccount Tests
```csharp
[Fact]
public async Task Deposit_WithValidAmount_ShouldIncreaseBalance()
{
    var account = new BankAccount(customerId, "0001");
    account.Deposit(1000m, "Initial");
    Assert.Equal(1000m, account.Balance.Amount);
}
```

### 2. Transfer Tests
```csharp
[Fact]
public async Task Transfer_BetweenAccounts_ShouldDebitAndCredit()
{
    var fromAccount = /* ... */;
    var toAccount = /* ... */;
    fromAccount.Transfer(toAccount, 500m);
    Assert.Equal(500m, toAccount.Balance.Amount);
}
```

### 3. Validation Tests
```csharp
[Fact]
public async Task CreateAccount_WithDuplicateNumber_ShouldThrow()
{
    // Criar primeira conta
    // Tentar criar com mesmo número
    // Verificar exceção
}
```

---

**Pro Tip**: Sempre execute `dotnet test` após fazer mudanças para garantir que nada quebrou!
