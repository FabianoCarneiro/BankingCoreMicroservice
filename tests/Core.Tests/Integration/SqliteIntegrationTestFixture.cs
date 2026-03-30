using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Core.Tests.Integration;

/// <summary>
/// Classe base para testes de integração com SQLite
/// (Mais leve, sem container - ideal para testes rápidos em CI/CD)
/// </summary>
public class SqliteIntegrationTestFixture : IAsyncLifetime
{
    private readonly string _dbPath;
    public BankingContext? DbContext { get; private set; }

    public SqliteIntegrationTestFixture()
    {
        // Usar diretório temporário para cada teste
        _dbPath = Path.Combine(Path.GetTempPath(), $"banking_test_{Guid.NewGuid()}.db");
    }

    /// <summary>
    /// Inicializa o banco SQLite
    /// </summary>
    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<BankingContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        DbContext = new BankingContext(options);

        // Criar schema
        await DbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Limpa o banco e remove arquivo
    /// </summary>
    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.DisposeAsync();
        }

        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }
}

/// <summary>
/// Coleção de testes de integração com SQLite
/// </summary>
[CollectionDefinition("SQLite Collection")]
public class SqliteIntegrationTestCollection : ICollectionFixture<SqliteIntegrationTestFixture>
{
    // Esta classe não tem implementação - apenas marca a coleção
}
