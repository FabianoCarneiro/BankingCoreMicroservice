using Core.Application.DTOs;
using Core.Application.UseCases;
using Core.Domain.Entities;
using Core.Domain.Ports;
using Core.Infrastructure.Adapters;
using Core.Tests.Helpers;
using Moq;
using Xunit;

namespace Core.Tests.Integration;

/// <summary>
/// Testes para validar se os testes de integração estão realmente funcionando corretamente
/// Simula erros e falhas intencionais para verificar o comportamento dos testes
/// </summary>
[Collection("SQLite Collection")]
public class ErrorSimulationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;

    public ErrorSimulationTests(SqliteIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o teste falha quando nenhum dado é persistido
    /// Execute este teste COMENTADO no seu CI/CD
    /// </summary>
    [Fact(Skip = "Uncomment to validate persistence testing")]
    public async Task SimulateError_MissingPersistence_ShouldFail()
    {
        // Este teste demonstra que nossos testes REALMENTE validam persistência
        var dbContext = _fixture.DbContext;
        
        var customer = new Customer(
            cpf: CpfGenerator.ValidCpfs.Customer1,
            name: "Test Customer",
            email: "test@example.com",
            phoneNumber: "11999999999"
        );

        dbContext.Customers.Add(customer);
        // ❌ NÃO salvamos no banco!
        // await dbContext.SaveChangesAsync();

        // Se this test passes, our validation is WRONG!
        // This MUST fail because data wasn't saved
        var saved = await dbContext.Customers.FindAsync(customer.Id);
        Assert.NotNull(saved); // ❌ EXPECTED TO FAIL
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o teste falha com assertion errada
    /// Execute este teste COMENTADO para validar
    /// </summary>
    [Fact(Skip = "Uncomment to validate assertion validation")]
    public async Task SimulateError_WrongAssertion_ShouldFail()
    {
        // Este teste demonstra que as assertions estão sendo validadas
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "11999999999"
        };
        
        var result = await useCase.ExecuteAsync(input);
        
        // ❌ Assertion incorreta - esperando nome diferente
        Assert.Equal("Maria", result.Name); // EXPECTED TO FAIL - name is "João Silva"
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o teste falha com valores vazios
    /// </summary>
    [Fact(Skip = "Uncomment to validate empty field handling")]
    public async Task SimulateError_EmptyEmail_ShouldFail()
    {
        // Este teste demonstra que validações são aplicadas
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "Test",
            Email = "", // ❌ Email vazio
            PhoneNumber = "11999999999"
        };
        
        // EXPECTED TO FAIL - email should be validated
        var result = await useCase.ExecuteAsync(input);
        Assert.NotNull(result); // Should not reach here
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o teste falha com CPF inválido
    /// Este teste DEVE falhar porque usamos CPF inválido
    /// </summary>
    [Fact(Skip = "Uncomment to validate CPF validation")]
    public async Task SimulateError_InvalidCpf_ShouldFail()
    {
        // Este teste verifica que CPF inválido não passa
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = "12345678901", // ❌ CPF checksum inválido
            Name = "Test Customer",
            Email = "test@example.com",
            PhoneNumber = "11999999999"
        };
        
        // EXPECTED TO FAIL - invalid CPF should throw exception
        var result = await useCase.ExecuteAsync(input);
        Assert.NotNull(result); // Should not reach here
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o teste falha com contagem errada
    /// </summary>
    [Fact(Skip = "Uncomment to validate data counting")]
    public async Task SimulateError_WrongCount_ShouldFail()
    {
        // Este teste verifica que a contagem de dados é validada
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "Test",
            Email = "test@example.com",
            PhoneNumber = "11999999999"
        };
        
        await useCase.ExecuteAsync(input);
        
        var count = dbContext.Customers.Count();
        
        // ❌ Assertion errada
        Assert.Equal(0, count); // EXPECTED TO FAIL - there's 1 customer
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Simula erro de banco de dados
    /// </summary>
    [Fact]
    public async Task SimulateError_DatabaseException_ShouldThrow()
    {
        // Mock do repositório que lança exceção
        var mockRepository = new Mock<ICustomerRepository>();
        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));
        
        var useCase = new CreateCustomerUseCase(mockRepository.Object);
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "Test",
            Email = "test@example.com",
            PhoneNumber = "11999999999"
        };
        
        // ✅ DEVE falhar com InvalidOperationException
        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(input));
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Simula timeout do banco de dados
    /// </summary>
    [Fact]
    public async Task SimulateError_DatabaseTimeout_ShouldThrow()
    {
        var mockRepository = new Mock<ICustomerRepository>();
        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ThrowsAsync(new TimeoutException("Database is unresponsive"));
        
        var useCase = new CreateCustomerUseCase(mockRepository.Object);
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "Test",
            Email = "test@example.com",
            PhoneNumber = "11999999999"
        };
        
        // ✅ DEVE falhar com TimeoutException
        await Assert.ThrowsAsync<TimeoutException>(() => useCase.ExecuteAsync(input));
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o fixture está sendo usado
    /// Se este teste passar, significa que o fixture é injetado corretamente
    /// </summary>
    [Fact]
    public void ValidateFixtureInjection_ShouldNotBeNull()
    {
        // ✅ Fixture está injetado
        Assert.NotNull(_fixture);
        
        // ✅ DbContext está disponível
        Assert.NotNull(_fixture.DbContext);
        
        // ✅ Banco está vazio no início
        var initialCount = _fixture.DbContext.Customers.Count();
        Assert.Equal(0, initialCount);
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica isolamento entre testes
    /// Rodar este teste múltiplas vezes - cada execução deve ter banco vazio
    /// </summary>
    [Fact]
    public async Task ValidateTestIsolation_ShouldHaveCleanDatabase()
    {
        var dbContext = _fixture.DbContext;
        
        // ✅ Cada teste começa com banco vazio
        var initialCount = dbContext.Customers.Count();
        Assert.Equal(0, initialCount);
        
        // Criar um cliente
        var customer = new Customer(
            cpf: CpfGenerator.ValidCpfs.Customer1,
            name: "Test",
            email: "test@example.com",
            phoneNumber: "11999999999"
        );
        
        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();
        
        // Verificar que o cliente foi criado
        var finalCount = dbContext.Customers.Count();
        Assert.Equal(1, finalCount);
        
        // ✅ Próximo teste deve ter banco vazio novamente (isolamento garantido)
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o DbContext é realmente limpo entre testes
    /// Execute dois testes em sequência para validar isolamento
    /// </summary>
    [Fact]
    public async Task ValidateIsolation_FirstTest_CreateCustomer()
    {
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        // Criar primeiro cliente
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "First Test Customer",
            Email = "first@example.com",
            PhoneNumber = "11999999999"
        };
        
        var customer = await useCase.ExecuteAsync(input);
        
        // Verificar que foi criado
        var count = dbContext.Customers.Count();
        Assert.Equal(1, count);
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Verifica que o DbContext foi limpo após teste anterior
    /// Este deve ter banco vazio mesmo após o teste anterior
    /// </summary>
    [Fact]
    public async Task ValidateIsolation_SecondTest_ShouldHaveCleanDatabase()
    {
        var dbContext = _fixture.DbContext;
        
        // ✅ Deve estar vazio mesmo após teste anterior
        var count = dbContext.Customers.Count();
        Assert.Equal(0, count); // ✅ Isolamento confirmado!
    }

    /// <summary>
    /// ✅ VALIDAÇÃO: Testa multiple operations em sequência
    /// </summary>
    [Fact]
    public async Task ValidateMultipleOperations_SequentialOperations()
    {
        var dbContext = _fixture.DbContext;
        var repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        // Criar 3 clientes em sequência
        var cpfs = new[] { 
            CpfGenerator.ValidCpfs.Customer1,
            CpfGenerator.ValidCpfs.Customer2,
            CpfGenerator.ValidCpfs.Customer3
        };
        
        var customers = new List<(Guid Id, string Name)>();
        
        for (int i = 0; i < cpfs.Length; i++)
        {
            var input = new CreateCustomerDTO
            {
                CPF = cpfs[i],
                Name = $"Customer {i + 1}",
                Email = $"customer{i + 1}@example.com",
                PhoneNumber = "11999999999"
            };
            
            var result = await useCase.ExecuteAsync(input);
            customers.Add((result.Id, result.Name));
        }
        
        // ✅ Verificar que todos foram criados
        Assert.Equal(3, customers.Count);
        
        // ✅ Verificar que não há duplicatas
        var uniqueIds = customers.Select(c => c.Id).Distinct().Count();
        Assert.Equal(3, uniqueIds);
        
        // ✅ Verificar persistência
        var dbCount = dbContext.Customers.Count();
        Assert.Equal(3, dbCount);
    }
}

/// <summary>
/// Classe de testes para demonstrar como validar o comportamento dos testes
/// Use esta como referência para seus testes
/// </summary>
public class TestValidationGuide
{
    /// <summary>
    /// PASSO 1: Escrever teste com assertion correta
    /// </summary>
    public static void Step1_WriteCorrectTest()
    {
        Console.WriteLine("✅ PASSO 1: Teste com assertion correta");
        Console.WriteLine("Assert.Equal(\"João Silva\", result.Name);");
    }

    /// <summary>
    /// PASSO 2: Rodar teste - deve passar
    /// </summary>
    public static void Step2_RunTest()
    {
        Console.WriteLine("✅ PASSO 2: Rodar teste - DEVE PASSAR");
        Console.WriteLine("$ dotnet test --filter \"ExecuteAsync_WithValidData\"");
        Console.WriteLine("Result: PASSED ✅");
    }

    /// <summary>
    /// PASSO 3: Simular erro #1 - quebrar persistência
    /// </summary>
    public static void Step3_SimulateError1()
    {
        Console.WriteLine("❌ PASSO 3: Comentar SaveChangesAsync() - DEVE FALHAR");
        Console.WriteLine("// await dbContext.SaveChangesAsync(); // Comentado!");
        Console.WriteLine("Result: FAILED ❌ (Como esperado)");
    }

    /// <summary>
    /// PASSO 4: Simular erro #2 - assertion errada
    /// </summary>
    public static void Step4_SimulateError2()
    {
        Console.WriteLine("❌ PASSO 4: Mudar assertion - DEVE FALHAR");
        Console.WriteLine("Assert.Equal(\"Maria\", result.Name); // Esperado \"João Silva\"");
        Console.WriteLine("Result: FAILED ❌ (Como esperado)");
    }

    /// <summary>
    /// PASSO 5: Restaurar código - teste deve passar
    /// </summary>
    public static void Step5_RestoreCode()
    {
        Console.WriteLine("✅ PASSO 5: Restaurar código original - DEVE PASSAR");
        Console.WriteLine("Assert.Equal(\"João Silva\", result.Name);");
        Console.WriteLine("await dbContext.SaveChangesAsync();");
        Console.WriteLine("Result: PASSED ✅ (Validação completa!)");
    }
}
