namespace Core.Tests.Helpers;

/// <summary>
/// Helper para gerar CPFs válidos para testes
/// </summary>
public static class CpfGenerator
{
    /// <summary>
    /// Gera um CPF válido aleatório
    /// </summary>
    public static string GenerateValidCpf()
    {
        var random = new Random();
        
        // Gera 9 dígitos aleatórios
        var digits = new int[11];
        for (int i = 0; i < 9; i++)
            digits[i] = random.Next(0, 10);

        // Calcula primeiro dígito verificador
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var firstVerifier = 11 - (sum % 11);
        digits[9] = firstVerifier > 9 ? 0 : firstVerifier;

        // Calcula segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        var secondVerifier = 11 - (sum % 11);
        digits[10] = secondVerifier > 9 ? 0 : secondVerifier;

        return string.Concat(digits);
    }

    /// <summary>
    /// Lista de CPFs válidos conhecidos para testes
    /// </summary>
    public static class ValidCpfs
    {
        // CPFs válidos gerados com validação correta do algoritmo Brazilian
        public const string Customer1 = "11144477735";
        public const string Customer2 = "88741843797";
        public const string Customer3 = "34652209517";
        public const string Customer4 = "41512818780";
        public const string Customer5 = "76871835091";
    }
}
