using Core.Application.DTOs;
using Core.Application.UseCases;
using Core.Domain.Entities;
using Core.Domain.Ports;
using Core.Domain.ValueObjects;
using Xunit;
using Moq;

namespace Core.Tests.UseCases;

public class GetCustomerByIdUseCaseTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly GetCustomerByIdUseCase _useCase;

    public GetCustomerByIdUseCaseTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _useCase = new GetCustomerByIdUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new Customer(
            cpf: "12345678909",
            name: "João Silva",
            email: "joao@example.com",
            phoneNumber: "11987654321"
        );
        customer.GetType().GetProperty("Id")?.SetValue(customer, customerId);

        _mockRepository.Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        // Act
        var result = await _useCase.ExecuteAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal("João Silva", result.Name);
        Assert.Equal("joao@example.com", result.Email);
        _mockRepository.Verify(x => x.GetByIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(Guid.Empty));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mockRepository.Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _useCase.ExecuteAsync(customerId);

        // Assert
        Assert.Null(result);
    }
}

public class ListAllCustomersUseCaseTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly ListAllCustomersUseCase _useCase;

    public ListAllCustomersUseCaseTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _useCase = new ListAllCustomersUseCase(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer("12345678909", "João Silva", "joao@example.com", "11987654321"),
            new Customer("98765432100", "Maria Santos", "maria@example.com", "11987654322")
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(customers);

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        var customerList = result.ToList();
        Assert.Equal(2, customerList.Count);
        Assert.Equal("João Silva", customerList[0].Name);
        Assert.Equal("Maria Santos", customerList[1].Name);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoCustomers_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Customer>());

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
