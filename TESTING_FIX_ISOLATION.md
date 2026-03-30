# Solução: Implementar Isolamento de Testes com Transações

## 🎯 Problema

Os testes não são isolados - dados vazam entre testes:

```
Test 1 creates 1 customer → Count = 1
Test 2 starts with Count = 1 (esperava 0!)
Test 2 creates 1 customer → Count = 2
Test 3 starts with Count = 2 (esperava 0!)
```

## ✅ Solução: Usar Transações que são Revertidas

### Passo 1: Atualizar o SQLite Fixture

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage;
using Core.Infrastructure.Persistence;
using Xunit;

namespace Core.Tests.Integration;

public class SqliteIntegrationTestFixture : IAsyncLifetime
{
    private readonly string _dbPath = Path.Combine(
        Path.GetTempPath(), 
        $"banking_test_{Guid.NewGuid()}.db"
    );
    
    public BankingContext? DbContext { get; private set; }
    private IDbContextTransaction? _transaction;

    public async Task InitializeAsync()
    {
        DbContext = new BankingContext("Data Source=" + _dbPath);
        
        // Criar schema
        await DbContext.Database.EnsureCreatedAsync();
        
        // ✅ NOVO: Iniciar transação para este teste
        _transaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        // ✅ NOVO: Reverter transação (desfaz todas as mudanças)
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        DbContext?.Dispose();
        
        // Limpar arquivo de banco
        try { File.Delete(_dbPath); }
        catch { /* ignored */ }
    }
}
```

### Passo 2: O que Acontece Agora

```
Test 1 Executa:
├─ InitializeAsync() → Start Transaction
├─ Create 1 customer
├─ Assert passes
└─ DisposeAsync() → Rollback Transaction ← ✅ Desfaz tudo!

Test 2 Executa:
├─ InitializeAsync() → Start NEW Transaction
├─ Count = 0 ✅ (banco está limpo!)
├─ Create 1 customer
├─ Assert passes
└─ DisposeAsync() → Rollback Transaction ← ✅ Desfaz tudo!

Test 3 Executa:
├─ InitializeAsync() → Start NEW Transaction
├─ Count = 0 ✅ (banco está limpo!)
└─ ...
```

---

## 📊 Comparação: Antes vs Depois

### ❌ ANTES (Sem Transações)

```csharp
[Collection("SQLite Collection")]
public class Tests
{
    [Fact]
    public async Task Test1()
    {
        // Fixture criado
        var count1 = _fixture.DbContext.Customers.Count(); // 0
        // Criar cliente
        var count2 = _fixture.DbContext.Customers.Count(); // 1
        // ✅ Teste passa
        // Fixture descartado (mas dados permanecem no banco!)
    }

    [Fact]
    public async Task Test2()
    {
        // Novo fixture criado (banco AINDA tem dados!)
        var count1 = _fixture.DbContext.Customers.Count(); // 1 ❌
        // ❌ Teste FALHA - esperava 0!
    }
}
```

### ✅ DEPOIS (Com Transações)

```csharp
[Collection("SQLite Collection")]
public class Tests
{
    [Fact]
    public async Task Test1()
    {
        // Fixture criado + Transação iniciada
        var count1 = _fixture.DbContext.Customers.Count(); // 0
        // Criar cliente
        var count2 = _fixture.DbContext.Customers.Count(); // 1
        // ✅ Teste passa
        // Fixture descartado + Transação revertida ← Dados apagados!
    }

    [Fact]
    public async Task Test2()
    {
        // Novo fixture criado + Nova transação
        var count1 = _fixture.DbContext.Customers.Count(); // 0 ✅
        // ✅ Teste passa - banco está limpo!
    }
}
```

---

## 🧪 Teste a Solução

### 1. Atualize o Fixture

Substitua o conteúdo de `tests/Core.Tests/Integration/SqliteIntegrationTestFixture.cs`:

```csharp
using Microsoft.EntityFrameworkCore.Storage;
using Core.Infrastructure.Persistence;
using Xunit;

namespace Core.Tests.Integration;

[CollectionDefinition("SQLite Collection")]
public class SqliteCollection : ICollectionFixture<SqliteIntegrationTestFixture>
{
    // This class has no code, just uses ICollectionFixture
    // to define the collection
}

public class SqliteIntegrationTestFixture : IAsyncLifetime
{
    private readonly string _dbPath = Path.Combine(
        Path.GetTempPath(), 
        $"banking_test_{Guid.NewGuid()}.db"
    );
    
    public BankingContext? DbContext { get; private set; }
    private IDbContextTransaction? _transaction;

    public async Task InitializeAsync()
    {
        // Criar DbContext com caminho único
        DbContext = new BankingContext("Data Source=" + _dbPath);
        
        // Criar schema do banco
        await DbContext.Database.EnsureCreatedAsync();
        
        // ✅ NOVO: Iniciar transação
        _transaction = await DbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        try
        {
            // ✅ NOVO: Reverter transação (desfaz todas as mudanças do teste)
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }
        finally
        {
            DbContext?.Dispose();
            
            // Limpar arquivo de banco
            try { File.Delete(_dbPath); }
            catch { /* ignored */ }
        }
    }
}
```

### 2. Execute os Testes

```bash
# Antes da mudança - isolamento falha
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "ValidateIsolation"
# Resultado: FALHOU (4 registros encontrados)

# Depois da mudança - isolamento passa!
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "ValidateIsolation"
# Resultado: PASSA (0 registros encontrados)
```

### 3. Valide os Resultados

```bash
# Rodar todos os testes - devem passar
dotnet test tests/Core.Tests/Core.Tests.csproj

# Esperado:
# Passed: 21
# Failed: 0
```

---

## 🎯 Benefícios da Solução

✅ **Isolamento Total**: Cada teste começa com banco limpo
✅ **Performance**: Sem necessidade de criar/deletar arquivos
✅ **Confiabilidade**: Testes não afetam uns aos outros
✅ **Simplicidade**: Uma linha de código (`BeginTransactionAsync`)

---

## 🔄 Como Funciona (Técnico)

### Transações ACID

```
┌─────────────────────────────────────────┐
│ Test 1 - Transação                      │
├─────────────────────────────────────────┤
│ BEGIN TRANSACTION                       │
│   INSERT customer (John)                │
│   INSERT customer (Jane)                │
│ ROLLBACK ← Desfaz INSERT!               │
└─────────────────────────────────────────┘
         ↓ Banco volta ao estado anterior
┌─────────────────────────────────────────┐
│ Test 2 - Nova Transação                 │
├─────────────────────────────────────────┤
│ BEGIN TRANSACTION                       │
│   Banco está limpo! ✅                  │
│   INSERT customer (Alice)               │
│ ROLLBACK ← Desfaz INSERT!               │
└─────────────────────────────────────────┘
```

### SQL Equivalente

```sql
-- Test 1
BEGIN TRANSACTION;
INSERT INTO Customers VALUES (1, 'John', ...);
INSERT INTO Customers VALUES (2, 'Jane', ...);
ROLLBACK;  -- ← Desfaz tudo!

-- Test 2
BEGIN TRANSACTION;
INSERT INTO Customers VALUES (1, 'Alice', ...);
-- Banco começa limpo! ✅
SELECT COUNT(*) FROM Customers; -- 1
ROLLBACK;  -- ← Desfaz tudo!
```

---

## ⚠️ Casos Especiais

### 1. E se meu teste precisar de dados?

Sem problema! Os dados são criados dentro da transação:

```csharp
[Fact]
public async Task MyTest_WithData()
{
    // Transaction iniciada
    
    // Criar dados de teste
    var customer = new Customer(...);
    _fixture.DbContext.Customers.Add(customer);
    await _fixture.DbContext.SaveChangesAsync();
    
    // Dados disponíveis para o teste
    Assert.NotNull(customer.Id);
    
    // DisposeAsync() → ROLLBACK
    // Dados são descartados automaticamente ✅
}
```

### 2. E se eu precisar de dados persistentes?

Use uma tabela separada ou um banco de teste:

```csharp
[Fact]
public async Task MyTest_SkipTransaction()
{
    // Temporariamente desabilitar rollback
    // (não recomendado, mas possível)
}
```

### 3. E testes paralelos?

SQLite tem uma transação **por teste**, então cada teste é isolado:

```
Thread 1: Test 1 com Transação 1
Thread 2: Test 2 com Transação 2  ← Isoladas!
Thread 3: Test 3 com Transação 3  ← Sem interferência!
```

---

## 📋 Checklist de Implementação

- [ ] Atualizar `SqliteIntegrationTestFixture.cs` com transações
- [ ] Adicionar `using Microsoft.EntityFrameworkCore.Storage`
- [ ] Testar isolamento: `dotnet test --filter "ValidateIsolation"`
- [ ] Verificar que todos os testes passam
- [ ] Documentar mudança no git
- [ ] Celebrar! 🎉

---

## 📚 Referências

- [EntityFramework Transactions](https://docs.microsoft.com/en-us/ef/core/miscellaneous/transactions)
- [xUnit Test Isolation](https://xunit.net/docs/getting-started/netfx/collection-fixtures)
- [SQLite Transactions](https://www.sqlite.org/lang_transaction.html)
- [Test Fixtures Best Practices](https://xunit.net/docs/getting-started/netfx/fixtures)

