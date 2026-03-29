namespace Customer.Application.DTOs;

/// <summary>
/// DTO para criação de cliente
/// </summary>
public class CreateCustomerDTO
{
    public string CPF { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// DTO para atualização de cliente
/// </summary>
public class UpdateCustomerDTO
{
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de cliente
/// </summary>
public class CustomerDTO
{
    public Guid Id { get; set; }
    public string CPF { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
