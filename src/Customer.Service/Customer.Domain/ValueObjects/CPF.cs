namespace Customer.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um CPF
/// Implementa validação de formato
/// </summary>
public class CPF
{
    public string Value { get; }

    public CPF(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "CPF não pode ser vazio");

        // Remover caracteres especiais
        var cleanCpf = value.Replace(".", "").Replace("-", "");

        if (cleanCpf.Length != 11)
            throw new ArgumentException("CPF deve ter 11 dígitos", nameof(value));

        if (!cleanCpf.All(char.IsDigit))
            throw new ArgumentException("CPF deve conter apenas números", nameof(value));

        Value = cleanCpf;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CPF other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
}
