using Core.Application.DTOs;
using Core.Application.UseCases;
using Core.Domain.Entities;
using Core.Domain.Ports;
using Core.Infrastructure.Adapters;
using Core.Tests.Helpers;
using Xunit;

namespace Core.Tests.Integration;

/// <summary>
/// Integration tests for CreateCustomerUseCase using SQLite database
/// Tests the complete flow from use case to repository to database persistence
/// </summary>
[Collection("SQLite Collection")]
public class CreateCustomerUseCaseIntegrationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;

    public CreateCustomerUseCaseIntegrationTests(SqliteIntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateCustomer()
    {
        // Arrange
        var dbContext = _fixture.DbContext;
        Assert.NotNull(dbContext);
        
        ICustomerRepository repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = CpfGenerator.ValidCpfs.Customer1,
            Name = "João Silva",
            Email = "joao@example.com",
            PhoneNumber = "11987654321"
        };
        
        // Act
        var customerDto = await useCase.ExecuteAsync(input);
        
        // Assert
        Assert.NotNull(customerDto);
        Assert.NotEqual(Guid.Empty, customerDto.Id);
        Assert.Equal("João Silva", customerDto.Name);
        Assert.Equal("joao@example.com", customerDto.Email);
        
        // Verify persistence
        var savedCustomer = await dbContext.Customers.FindAsync(customerDto.Id);
        Assert.NotNull(savedCustomer);
        Assert.Equal("João Silva", savedCustomer.Name);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCpf_ShouldThrowException()
    {
        // Arrange
        var dbContext = _fixture.DbContext;
        Assert.NotNull(dbContext);
        
        ICustomerRepository repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var input = new CreateCustomerDTO
        {
            CPF = "12345678900", // Invalid CPF
            Name = "Invalid Customer",
            Email = "invalid@example.com",
            PhoneNumber = "11987654321"
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(input)
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleCustomers_ShouldCreateAllCorrectly()
    {
        // Arrange
        var dbContext = _fixture.DbContext;
        Assert.NotNull(dbContext);
        
        ICustomerRepository repository = new CustomerRepository(dbContext);
        var useCase = new CreateCustomerUseCase(repository);
        
        var inputs = new[]
        {
            new CreateCustomerDTO
            {
                CPF = CpfGenerator.ValidCpfs.Customer1,
                Name = "Customer 1",
                Email = "customer1@example.com",
                PhoneNumber = "11987654321"
            },
            new CreateCustomerDTO
            {
                CPF = CpfGenerator.ValidCpfs.Customer2,
                Name = "Customer 2",
                Email = "customer2@example.com",
                PhoneNumber = "11987654322"
            },
            new CreateCustomerDTO
            {
                CPF = CpfGenerator.ValidCpfs.Customer3,
                Name = "Customer 3",
                Email = "customer3@example.com",
                PhoneNumber = "11987654323"
            }
        };
        
        // Act
        var results = new List<(Guid Id, string Name)>();
        foreach (var input in inputs)
        {
            var result = await useCase.ExecuteAsync(input);
            results.Add((result.Id, result.Name));
        }
        
        // Assert
        Assert.Equal(3, results.Count);
        
        // Verify all customers persisted
        var customersInDb = dbContext.Customers.ToList();
        Assert.True(customersInDb.Count >= 3);
        
        foreach (var (id, name) in results)
        {
            var savedCustomer = await dbContext.Customers.FindAsync(id);
            Assert.NotNull(savedCustomer);
            Assert.Equal(name, savedCustomer.Name);
        }
    }
}
