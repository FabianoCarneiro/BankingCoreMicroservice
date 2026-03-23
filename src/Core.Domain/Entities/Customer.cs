using Core.Domain.ValueObjects;

namespace Core.Domain.Entities;

/// <summary>
/// Entidade que representa um cliente do banco (agregado raiz)
/// </summary>
public class Customer
{
    public Guid Id { get; private set; }
    public CPF CPF { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public Customer(string cpf, string name, string email, string phoneNumber)
    {
        Id = Guid.NewGuid();
        CPF = new CPF(cpf);
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    // EF Core
    private Customer() { }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContact(string email, string phoneNumber)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        UpdatedAt = DateTime.UtcNow;
    }
}
