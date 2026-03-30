# Como Simular Erros para Validar Testes de Integração

Este guia mostra como injetar falhas propositais para verificar se seus testes estão realmente testando o que você pensa que estão testando.

## 1. Validar que o Teste Falha com Dados Inválidos

### Estratégia: Quebrar a Assertion

Modifique temporariamente o teste para verificar que ele realmente falha quando algo está errado:

```csharp
[Fact]
public async Task ExecuteAsync_WithValidData_ShouldCreateCustomer()
{
    // ... seu teste ...
    
    // VALIDAÇÃO: Mudar a assertion para um valor impossível
    Assert.Equal("Wrong Name", customerDto.Name); // ❌ Vai falhar!
    // Assert.Equal("João Silva", customerDto.Name); // ✅ Correto
}
```

**Como rodar:**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "ExecuteAsync_WithValidData"
```

**Esperado:**
```
FAILED Core.Tests.Integration.CreateCustomerUseCaseIntegrationTests.ExecuteAsync_WithValidData_ShouldCreateCustomer
Expected string: "Wrong Name"
Actual string: "João Silva"
```

---

## 2. Validar que o Teste Realmente Acessa o Banco

### Estratégia: Remover a Persistência

Comente o `SaveChangesAsync()` para verificar se o teste realmente valida a persistência:

```csharp
[Fact]
public async Task ExecuteAsync_WithValidData_ShouldCreateCustomer_ValidatePersistence()
{
    // Arrange
    var dbContext = _fixture.DbContext;
    ICustomerRepository repository = new CustomerRepository(dbContext);
    
    // Criar customer
    var customer = new Customer(
        cpf: CpfGenerator.ValidCpfs.Customer1,
        name: "Test",
        email: "test@example.com",
        phoneNumber: "11999999999"
    );
    
    dbContext.Customers.Add(customer);
    // await dbContext.SaveChangesAsync(); // ❌ Comentado!
    
    // Se o teste passar aqui, ele NÃO está validando persistência real!
    var savedCustomer = await dbContext.Customers.FindAsync(customer.Id);
    Assert.NotNull(savedCustomer); // ❌ Vai falhar porque não salvou!
}
```

**Esperado:** Teste falha porque não há dados no banco.

---

## 3. Validar que o Teste Realmente Usa o Fixture Correto

### Estratégia: Usar DbContext Nulo

Injete um erro para verificar se o teste depende realmente do fixture:

```csharp
[Fact]
public async Task ValidateFixtureDependency()
{
    // Se você conseguir passar um DbContext nulo, o teste não depende do fixture
    BankingContext? nullContext = null; // ❌ Erro proposital
    ICustomerRepository repository = new CustomerRepository(nullContext!);
    
    // Deve falhar aqui
    var result = await repository.GetByIdAsync(Guid.NewGuid());
}
```

**Esperado:** `NullReferenceException` ou `ArgumentNullException`

---

## 4. Validar Isolamento entre Testes

### Estratégia: Verificar que dados não vazam entre testes

```csharp
private static int _testCounter = 0;

[Fact]
public async Task FirstTest_ShouldCreateOneCustomer()
{
    _testCounter++;
    var dbContext = _fixture.DbContext;
    var initialCount = dbContext.Customers.Count();
    
    // ... criar customer ...
    
    var finalCount = dbContext.Customers.Count();
    Assert.Equal(initialCount + 1, finalCount);
}

[Fact]
public async Task SecondTest_ShouldHaveCleanDatabase()
{
    _testCounter++;
    var dbContext = _fixture.DbContext;
    
    // Se o banco não foi limpo, o contador será > 1
    var count = dbContext.Customers.Count();
    Assert.Equal(0, count); // ❌ Falha se testes compartilham dados
}
```

**Como rodar:**
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "FirstTest|SecondTest"
```

---

## 5. Simular Erros de Validação

### Estratégia: Verificar que o teste falha com dados inválidos

```csharp
[Fact]
public async Task ValidateInputValidation()
{
    var dbContext = _fixture.DbContext;
    ICustomerRepository repository = new CustomerRepository(dbContext);
    var useCase = new CreateCustomerUseCase(repository);
    
    // Teste 1: CPF vazio deve falhar
    var input1 = new CreateCustomerDTO
    {
        CPF = "", // ❌ Inválido
        Name = "Test",
        Email: "test@example.com",
        PhoneNumber: "11999999999"
    };
    
    await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(input1));
    
    // Teste 2: CPF inválido deve falhar
    var input2 = new CreateCustomerDTO
    {
        CPF = "12345678901", // ❌ Checksum inválido
        Name: "Test",
        Email: "test@example.com",
        PhoneNumber: "11999999999"
    };
    
    await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(input2));
    
    // Teste 3: Email vazio deve falhar
    var input3 = new CreateCustomerDTO
    {
        CPF = CpfGenerator.ValidCpfs.Customer1,
        Name: "Test",
        Email: "", // ❌ Inválido
        PhoneNumber: "11999999999"
    };
    
    await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(input3));
}
```

---

## 6. Simular Erro de Persistência

### Estratégia: Fazer o SaveChangesAsync falhar

```csharp
[Fact]
public async Task ValidateDatabaseErrorHandling()
{
    var dbContext = _fixture.DbContext;
    
    // Simular erro de banco de dados
    var mockRepository = new Mock<ICustomerRepository>();
    mockRepository
        .Setup(r => r.AddAsync(It.IsAny<Customer>()))
        .ThrowsAsync(new InvalidOperationException("Database error!"));
    
    var useCase = new CreateCustomerUseCase(mockRepository.Object);
    var input = new CreateCustomerDTO
    {
        CPF = CpfGenerator.ValidCpfs.Customer1,
        Name = "Test",
        Email: "test@example.com",
        PhoneNumber: "11999999999"
    };
    
    // Deve falhar com erro do banco
    await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
}
```

---

## 7. Checklist de Validação de Testes

Use este checklist para garantir que seus testes estão realmente testando:

```csharp
/// <summary>
/// Checklist para validar que um teste de integração está correto
/// </summary>
public class TestValidationChecklist
{
    [Fact]
    public async Task Checklist_ValidateIntegrationTest()
    {
        // ✅ 1. O fixture está sendo injetado?
        Assert.NotNull(_fixture);
        
        // ✅ 2. O DbContext está disponível?
        Assert.NotNull(_fixture.DbContext);
        
        // ✅ 3. O banco é limpo entre testes?
        var initialCount = _fixture.DbContext.Customers.Count();
        Assert.Equal(0, initialCount);
        
        // ✅ 4. O teste falha com dados inválidos?
        await Assert.ThrowsAsync<ArgumentException>(
            () => CreateInvalidCustomer()
        );
        
        // ✅ 5. O teste passa com dados válidos?
        var customer = await CreateValidCustomer();
        Assert.NotNull(customer);
        
        // ✅ 6. Os dados persistem no banco?
        var saved = await _fixture.DbContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(saved);
        
        // ✅ 7. O teste é isolado (não afeta outros testes)?
        // Será verificado após rodar múltiplos testes
    }
    
    private async Task<Customer> CreateValidCustomer()
    {
        var customer = new Customer(
            cpf: CpfGenerator.ValidCpfs.Customer1,
            name: "Valid Customer",
            email: "valid@example.com",
            phoneNumber: "11999999999"
        );
        
        _fixture.DbContext.Customers.Add(customer);
        await _fixture.DbContext.SaveChangesAsync();
        return customer;
    }
    
    private Task CreateInvalidCustomer()
    {
        return Task.Run(() =>
        {
            new Customer(
                cpf: "12345678901", // ❌ Inválido
                name: "Invalid",
                email: "invalid@example.com",
                phoneNumber: "11999999999"
            );
        });
    }
}
```

---

## 8. Simular Erro com Dados Duplicados

### Estratégia: Verificar que o teste falha com violação de restrição única

```csharp
[Fact]
public async Task ValidateDuplicateDetection()
{
    var dbContext = _fixture.DbContext;
    var repository = new CustomerRepository(dbContext);
    var useCase = new CreateCustomerUseCase(repository);
    
    // Criar primeiro cliente
    var input1 = new CreateCustomerDTO
    {
        CPF = CpfGenerator.ValidCpfs.Customer1,
        Name: "First",
        Email: "first@example.com",
        PhoneNumber: "11999999999"
    };
    
    var customer1 = await useCase.ExecuteAsync(input1);
    Assert.NotNull(customer1);
    
    // Tentar criar segundo cliente com mesmo CPF
    var input2 = new CreateCustomerDTO
    {
        CPF = CpfGenerator.ValidCpfs.Customer1, // ❌ Mesmo CPF
        Name: "Second",
        Email: "second@example.com",
        PhoneNumber: "11988888888"
    };
    
    // Deve falhar com erro de violação de constraint
    await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input2));
}
```

---

## 9. Rodar Testes com Diferentes Modos

### Modo 1: Rodar um teste específico
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "ExecuteAsync_WithValidData"
```

### Modo 2: Rodar testes de uma classe
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "CreateCustomerUseCaseIntegrationTests"
```

### Modo 3: Rodar com output detalhado
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --logger "console;verbosity=detailed" \
  --filter "ExecuteAsync"
```

### Modo 4: Rodar e parar no primeiro erro
```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --no-build --logger "console;verbosity=diagnostic"
```

---

## 10. Exemplo Completo: Teste com Simulação de Erro

```csharp
[Collection("SQLite Collection")]
public class ErrorSimulationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;

    public ErrorSimulationTests(SqliteIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Simula error removendo a persistência
    /// </summary>
    [Fact]
    public async Task SimulateError_MissingPersistence()
    {
        var dbContext = _fixture.DbContext;
        var customer = new Customer(
            cpf: CpfGenerator.ValidCpfs.Customer1,
            name: "Test",
            email: "test@example.com",
            phoneNumber: "11999999999"
        );

        dbContext.Customers.Add(customer);
        // NÃO salvar no banco!
        
        // Este teste DEVE FALHAR
        var saved = await dbContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(saved); // ❌ Falha aqui - não há dados no banco
    }

    /// <summary>
    /// Simula erro com assertion errada
    /// </summary>
    [Fact]
    public async Task SimulateError_WrongAssertion()
    {
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name: "João",
            Email: "joao@example.com",
            PhoneNumber: "11999999999"
        };
        
        var result = await useCase.ExecuteAsync(input);
        
        // ❌ Assertion incorreta - vai falhar
        Assert.Equal("Maria", result.Name); // Esperado "João"!
    }

    /// <summary>
    /// Simula erro com exceção inesperada
    /// </summary>
    [Fact]
    public async Task SimulateError_UnexpectedException()
    {
        var mockRepository = new Mock<ICustomerRepository>();
        
        // Simular exceção no repositório
        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ThrowsAsync(new TimeoutException("Banco de dados indisponível"));
        
        var useCase = new CreateCustomerUseCase(mockRepository.Object);
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name: "Test",
            Email: "test@example.com",
            PhoneNumber: "11999999999"
        };
        
        // ❌ Deve falhar com TimeoutException
        await Assert.ThrowsAsync<TimeoutException>(() => useCase.ExecuteAsync(input));
    }
}
```

---

## Fluxo Recomendado para Validação

1. **Escrever o teste** com assertions corretas
2. **Rodar o teste** - deve passar ✅
3. **Simular erro #1** - quebrar a persistência
   - Comentar `SaveChangesAsync()`
   - Rodar teste - deve FALHAR ❌
4. **Simular erro #2** - assertion errada
   - Mudar valor esperado
   - Rodar teste - deve FALHAR ❌
5. **Simular erro #3** - dados inválidos
   - Usar valores inválidos
   - Rodar teste - deve FALHAR ❌
6. **Restaurar código original** - teste deve passar ✅
7. **Rodar suite completa** - validar isolamento

---

## Conclusão

✅ **Teste está correto se:**
- Passa com dados válidos
- Falha com dados inválidos
- Falha quando persistência é quebrada
- Falha com assertions incorretas
- Não é afetado por outros testes
- Usa corretamente o fixture

❌ **Teste está fraco se:**
- Passa mesmo com dados inválidos
- Não valida persistência real
- Passa com assertions incorretas
- Compartilha estado com outros testes
- Não depende do banco de dados

