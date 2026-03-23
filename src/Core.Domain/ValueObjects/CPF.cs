namespace Core.Domain.ValueObjects;

/// <summary>
/// Value Object para validar e encapsular CPF
/// </summary>
public class CPF : IEquatable<CPF>
{
    public string Value { get; }

    public CPF(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CPF é obrigatório", nameof(value));

        var cleanValue = value.Replace(".", "").Replace("-", "");

        if (!IsValid(cleanValue))
            throw new ArgumentException("CPF inválido", nameof(value));

        Value = cleanValue;
    }

    private static bool IsValid(string cpf)
    {
        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            return false;

        // Verifica se todos os dígitos são iguais
        if (cpf.Distinct().Count() == 1)
            return false;

        // Calcula primeiro dígito verificador
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        var firstDigit = 11 - (sum % 11);
        firstDigit = firstDigit > 9 ? 0 : firstDigit;

        if (int.Parse(cpf[9].ToString()) != firstDigit)
            return false;

        // Calcula segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        var secondDigit = 11 - (sum % 11);
        secondDigit = secondDigit > 9 ? 0 : secondDigit;

        return int.Parse(cpf[10].ToString()) == secondDigit;
    }

    public string Formatted => $"{Value[..3]}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value[9..]}";

    public bool Equals(CPF? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as CPF);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Formatted;
}
