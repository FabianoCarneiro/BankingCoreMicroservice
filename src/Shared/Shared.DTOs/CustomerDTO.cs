namespace Shared.DTOs.Customer;

/// <summary>
/// DTO compartilhada para comunicação entre microserviços
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
