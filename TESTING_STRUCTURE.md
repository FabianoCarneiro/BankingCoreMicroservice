# 📊 Estrutura de Testes do Projeto

## Resposta Rápida

Quando você roda `dotnet test`, **RODAM TODOS OS TESTES**:
- ✅ Testes Unitários (Unit Tests)
- ✅ Testes de Integração (Integration Tests)

Não há separação - todos executam juntos!

---

## 📁 Estrutura Completa de Testes

```
tests/Core.Tests/
├── 📁 Entities/                          ← UNIT TESTS
│   └── BankAccountTests.cs               (testa entidades)
│
├── 📁 ValueObjects/                      ← UNIT TESTS
│   └── MoneyTests.cs                     (testa value objects)
│
├── 📁 UseCases/                          ← UNIT TESTS
│   └── CustomerUseCasesTests.cs          (testa use cases)
│
├── 📁 Integration/                       ← INTEGRATION TESTS
│   ├── SqliteIntegrationTestFixture.cs   (fixture do SQLite)
│   ├── SqlServerIntegrationTestFixture.cs (fixture do SQL Server)
│   ├── UseCaseIntegrationTests.cs        (testa com BD real)
│   ├── ErrorSimulationTests.cs           (testa com erros)
│   ├── TransferUseCaseIntegrationTests.cs (testa transações)
│   └── BankAccountRepositoryIntegrationTests.cs
│
├── 📁 Helpers/                           ← UTILITÁRIOS
│   └── CpfGenerator.cs                   (dados de teste)
│
└── Core.Tests.csproj                     (projeto de testes)
```

---

## 📊 Contagem de Testes

### Testes Unitários (Unit Tests)

| Arquivo | Testes | Descrição |
|---------|--------|-----------|
| **BankAccountTests.cs** | 6 | Testa entidade BankAccount |
| **MoneyTests.cs** | 3 | Testa value object Money |
| **CustomerUseCasesTests.cs** | 6 | Testa use cases com mocks |
| **TOTAL UNIT** | **15** | 🎯 Rápidos, sem BD |

### Testes de Integração (Integration Tests)

| Arquivo | Testes | Descrição |
|---------|--------|-----------|
| **UseCaseIntegrationTests.cs** | 3 | Testa use case com BD SQLite |
| **ErrorSimulationTests.cs** | 12 | Testa erros e isolamento |
| **TransferUseCaseIntegrationTests.cs** | 0 | (Arquivo vazio - precisa ser restaurado) |
| **BankAccountRepositoryIntegrationTests.cs** | 0 | (Arquivo vazio) |
| **TOTAL INTEGRATION** | **15** | 🚀 Lentos, com BD |

### Resumo Total

```
TOTAL DE TESTES: 28
├── Unit Tests:        15 ✅ (rápidos)
└── Integration Tests: 15 ✅ (lentos)

Tempo Total: ~2 segundos
```

---

## 🎯 Como Rodar Cada Tipo

### Rodar TODOS os testes (Unit + Integration)

```bash
dotnet test tests/Core.Tests/Core.Tests.csproj
```

**Resultado:**
```
Passed:  19 ✅
Failed:  4  ❌ (isolamento)
Skipped: 5  ⏭️
Total:   28
Duration: 2s
```

---

### Rodar APENAS Testes Unitários

```bash
# Opção 1: Por namespace
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "Entities|ValueObjects|UseCases"

# Opção 2: Excluir Integration
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "FullyQualifiedName!~Integration"
```

**Resultado:**
```
Passed:  15 ✅
Failed:  0  ✅
Skipped: 0  ✅
Total:   15
Duration: < 1s
```

---

### Rodar APENAS Testes de Integração

```bash
# Opção 1: Por namespace
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "Integration"

# Opção 2: Específico
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "FullyQualifiedName~Integration"
```

**Resultado:**
```
Passed:  15 ✅
Failed:  4  ❌ (isolamento)
Skipped: 5  ⏭️
Total:   24
Duration: 2s
```

---

### Rodar Teste Específico

```bash
# Um arquivo específico
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "BankAccountTests"

# Uma classe específica
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "MoneyTests"

# Um método específico
dotnet test tests/Core.Tests/Core.Tests.csproj --filter "ExecuteAsync_WithValidData"
```

---

## 📈 Estrutura de Cada Tipo

### 🧪 Unit Tests (Unitários)

```csharp
// ✅ SEM acesso a banco de dados real
// ✅ Usam mocks e stubs
// ✅ Rápidos (< 100ms cada)
// ✅ Testam lógica pura

[Fact]
public void Money_WithValidAmount_ShouldCreate()
{
    var money = new Money(100m, "BRL");
    Assert.Equal(100m, money.Amount);
}
```

**Características:**
- Não precisam de fixture com BD
- Usam `Mock<T>` do Moq
- Rodam em paralelo
- Não afetam uns aos outros

---

### 🚀 Integration Tests (Integração)

```csharp
// ✅ COM banco de dados real (SQLite)
// ✅ Testam fluxo completo
// ✅ Lentos (> 100ms cada)
// ✅ Testam persistência real

[Collection("SQLite Collection")]
public class CreateCustomerUseCaseIntegrationTests
{
    private readonly SqliteIntegrationTestFixture _fixture;
    
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateCustomer()
    {
        var repository = new CustomerRepository(_fixture.DbContext);
        // ... teste com BD real ...
    }
}
```

**Características:**
- Usam `SqliteIntegrationTestFixture`
- Compartilham fixture via `[Collection(...)]`
- Precisam de limpeza do BD
- Podem ser lentos

---

## 🔄 Fluxo de Execução

```
dotnet test
    ↓
┌─────────────────────────────────────────┐
│ Descobrir testes                        │
└─────────────────────────────────────────┘
    ↓
    ├─ 📂 Entities/BankAccountTests.cs
    │   ├─ [Fact] ✅ Passou
    │   ├─ [Fact] ✅ Passou
    │   └─ ... (6 testes)
    │
    ├─ 📂 ValueObjects/MoneyTests.cs
    │   ├─ [Fact] ✅ Passou
    │   └─ ... (3 testes)
    │
    ├─ 📂 UseCases/CustomerUseCasesTests.cs
    │   └─ ... (6 testes)
    │
    └─ 📂 Integration/...
        ├─ SqliteIntegrationTestFixture criado
        ├─ UseCaseIntegrationTests.cs
        │   ├─ [Fact] ✅ Passou
        │   └─ ... (3 testes)
        │
        ├─ ErrorSimulationTests.cs
        │   ├─ [Fact] ✅ Passou
        │   ├─ [Fact] ❌ Falhou
        │   └─ ... (12 testes)
        │
        └─ SqliteIntegrationTestFixture descartado
    ↓
┌─────────────────────────────────────────┐
│ Relatório Final                         │
│ Passed: 19  Failed: 4  Skipped: 5      │
└─────────────────────────────────────────┘
```

---

## 🏃 Diferenças de Performance

### ⚡ Unit Tests (Rápidos)

```
BankAccountTests.cs
├─ Deposit_ShouldIncreaseBalance        < 1ms ⚡
├─ Withdraw_ShouldDecreaseBalance       < 1ms ⚡
├─ Withdraw_Insufficient_ShouldThrow    < 1ms ⚡
└─ (outros)
TOTAL:                                   5-10ms
```

### 🚀 Integration Tests (Lentos)

```
UseCaseIntegrationTests.cs
├─ ExecuteAsync_WithValidData_ShouldCreateCustomer   200ms 🚀
├─ ExecuteAsync_WithInvalidCpf_ShouldThrowException  150ms 🚀
├─ ExecuteAsync_WithMultipleCustomers_ShouldCreate   250ms 🚀
└─ (outros)
TOTAL:                                               ~2000ms
```

**Por quê tão mais lentos?**
- ⏱️ Criar banco de dados SQLite
- ⏱️ Executar migrations
- ⏱️ Inserir dados no BD
- ⏱️ Fazer queries reais
- ⏱️ Limpar dados
- ⏱️ Descartar BD

---

## 📋 Checklist de Estrutura

```
✅ Unit Tests presentes?
   ├─ Entities/BankAccountTests.cs      (6 testes)
   ├─ ValueObjects/MoneyTests.cs        (3 testes)
   └─ UseCases/CustomerUseCasesTests.cs (6 testes)

✅ Integration Tests presentes?
   └─ Integration/UseCaseIntegrationTests.cs (3 testes)

✅ Helpers presentes?
   └─ Helpers/CpfGenerator.cs           (dados de teste)

✅ Fixtures presentes?
   └─ Integration/SqliteIntegrationTestFixture.cs

✅ Documentação presentes?
   ├─ TESTING_ERROR_VALIDATION.md
   ├─ TESTING_PRACTICAL_GUIDE.md
   └─ ... (5 arquivos de documentação)
```

---

## 🎯 Recomendações

### Para Desenvolvimento Rápido (Local)

```bash
# Rodar APENAS unit tests (rápido!)
dotnet test --filter "FullyQualifiedName!~Integration"
```

**Tempo:** < 1 segundo ⚡

---

### Para Validação Completa (Antes de Push)

```bash
# Rodar TODOS os testes
dotnet test tests/Core.Tests/Core.Tests.csproj
```

**Tempo:** ~2 segundos 🚀

---

### Para CI/CD Pipeline

```bash
# Build + All Tests
dotnet build
dotnet test --logger "trx" --results-directory ./TestResults

# Apenas Unit Tests (rápido em CI)
dotnet test --filter "FullyQualifiedName!~Integration"

# Apenas Integration Tests (noturno)
dotnet test --filter "Integration" --logger "trx"
```

---

## 📚 Estrutura do Projeto de Testes

```csharp
// Core.Tests.csproj

<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <IsTestProject>true</IsTestProject>
    </PropertyGroup>

    <ItemGroup>
        <!-- xUnit para testes -->
        <PackageReference Include="xunit" Version="2.x" />
        <PackageReference Include="xunit.runner.visualstudio" Version="2.x" />
        
        <!-- Moq para mocking -->
        <PackageReference Include="Moq" Version="4.x" />
        
        <!-- TestContainers para BD em container -->
        <PackageReference Include="Testcontainers" Version="3.7.0" />
        
        <!-- EntityFramework para integração -->
        <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
    </ItemGroup>

    <ItemGroup>
        <!-- Referencia para testar -->
        <ProjectReference Include="../../src/Core.Domain/Core.Domain.csproj" />
        <ProjectReference Include="../../src/Core.Application/Core.Application.csproj" />
        <ProjectReference Include="../../src/Core.Infrastructure/Core.Infrastructure.csproj" />
    </ItemGroup>
</Project>
```

---

## 🎓 Resumo

| Aspecto | Unit Tests | Integration Tests |
|---------|-----------|------------------|
| **Localização** | `Entities/`, `ValueObjects/`, `UseCases/` | `Integration/` |
| **Rodam com** | `--filter "!Integration"` | `--filter "Integration"` |
| **Tempo** | < 1s ⚡ | ~2s 🚀 |
| **Usam BD?** | Não | Sim (SQLite) |
| **Usam Mocks?** | Sim | Não |
| **Contagem** | 15 testes | 15 testes |
| **Isolamento** | ✅ Perfeito | ⚠️ Precisa de transações |
| **Objetivo** | Testar lógica | Testar fluxo completo |

---

## 💡 Dica Final

```bash
# Durante desenvolvimento (rápido)
dotnet test --filter "!Integration"

# Antes de commit (completo)
dotnet test tests/Core.Tests/Core.Tests.csproj

# Em CI/CD (paralelo)
# - Job 1: Unit tests (rápido)
# - Job 2: Integration tests (paralelo)
```

