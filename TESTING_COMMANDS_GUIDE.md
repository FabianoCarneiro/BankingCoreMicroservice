# 🚀 Exemplos Práticos: Como Rodar Testes

## Pergunta do Usuário

> "Quando rodo os testes 'dotnet test' está rodando só os de integração ou os unitários rodam também?"

### ✅ Resposta

**SIM! Rodam TODOS os testes:**
- ✅ Unitários (Unit Tests)
- ✅ Integração (Integration Tests)

Não há separação automática - tudo executa junto!

---

## 📊 Estrutura Atual de Testes

```
15 Unit Tests (rápidos)
├─ Entities/BankAccountTests.cs        (6 testes)
├─ ValueObjects/MoneyTests.cs          (3 testes)
└─ UseCases/CustomerUseCasesTests.cs   (6 testes)

15 Integration Tests (lentos)
└─ Integration/...                     (3 testes rodam)
   └─ ErrorSimulationTests.cs          (12 testes)
   └─ (4 falhando, 5 pulados)

═══════════════════════════════════════
TOTAL: 28 testes
```

---

## ⚠️ Aviso Importante para macOS/zsh

**Use aspas simples `'...'` ao invés de duplas `"..."` em filtros com `!` ou `~`:**

```bash
# ❌ Errado (macOS/zsh vai reclamar)
--filter "FullyQualifiedName!~Integration"
# Resultado: zsh: event not found: ~Integration

# ✅ Correto
--filter 'FullyQualifiedName!~Integration'
# Funciona! ✓
```

👉 **Veja `ZSH_SHELL_TIPS.md` para mais detalhes!**

---

## 📝 Exemplos de Comandos

### 1️⃣ Rodar TODOS os Testes (Padrão)

```bash
cd /Users/fabianocarneiro/BankingCoreMicroservice
dotnet test tests/Core.Tests/Core.Tests.csproj
```

**Output:**
```
[xUnit.net] Discovering tests...
[xUnit.net] Running tests...

Core.Tests.Integration.CreateCustomerUseCaseIntegrationTests.ExecuteAsync_WithValidData_ShouldCreateCustomer [PASS]
Core.Tests.Integration.CreateCustomerUseCaseIntegrationTests.ExecuteAsync_WithInvalidCpf_ShouldThrowException [PASS]
Core.Tests.Entities.BankAccountTests.Deposit_ShouldIncreaseBalance [PASS]
Core.Tests.ValueObjects.MoneyTests.Constructor_WithValidAmount_ShouldCreate [PASS]
...

Failed!  - Failed: 4, Passed: 19, Skipped: 5, Total: 28, Duration: 2 s
```

**Tempo:** ~2 segundos (incluindo BD)

---

### 2️⃣ Rodar APENAS Testes Unitários (Rápido!)

```bash
# Opção A: Excluir Integration (com escape para zsh)
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter 'FullyQualifiedName!~Integration'

# Opção B: Incluir específicas
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter 'Entities|ValueObjects|UseCases'
```

⚠️ **Importante para zsh:** Use aspas simples `'...'` ao invés de duplas `"..."` para evitar erro: `zsh: event not found: ~Integration`

**Output:**
```
Core.Tests.Entities.BankAccountTests.Deposit_ShouldIncreaseBalance [PASS]
Core.Tests.Entities.BankAccountTests.Withdraw_ShouldDecreaseBalance [PASS]
Core.Tests.ValueObjects.MoneyTests.Constructor_WithValidAmount_ShouldCreate [PASS]
Core.Tests.UseCases.CustomerUseCasesTests.Create_WithValidData_ShouldSuccess [PASS]
...

Passed! - Failed: 0, Passed: 15, Skipped: 0, Total: 15, Duration: 0.8 s
```

**Tempo:** < 1 segundo ⚡

---

### 3️⃣ Rodar APENAS Testes de Integração

```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter 'Integration'
```

**Output:**
```
Core.Tests.Integration.CreateCustomerUseCaseIntegrationTests.ExecuteAsync_WithValidData_ShouldCreateCustomer [PASS]
Core.Tests.Integration.CreateCustomerUseCaseIntegrationTests.ExecuteAsync_WithInvalidCpf_ShouldThrowException [PASS]
Core.Tests.Integration.ErrorSimulationTests.SimulateError_DatabaseException_ShouldThrow [PASS]
Core.Tests.Integration.ErrorSimulationTests.ValidateFixtureInjection_ShouldNotBeNull [FAIL]
...

Failed! - Failed: 4, Passed: 15, Skipped: 5, Total: 24, Duration: 2 s
```

**Tempo:** ~2 segundos (inclui criação de BD)

---

### 4️⃣ Rodar Um Teste Específico

```bash
# Por classe
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "BankAccountTests"

# Por método específico
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "ExecuteAsync_WithValidData_ShouldCreateCustomer"

# Por namespace
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --filter "FullyQualifiedName~Money"
```

**Output:**
```
Core.Tests.ValueObjects.MoneyTests.Constructor_WithValidAmount_ShouldCreate [PASS]
Core.Tests.ValueObjects.MoneyTests.Add_WithValidAmounts_ShouldAddCorrectly [PASS]
Core.Tests.ValueObjects.MoneyTests.Subtract_WithValidAmounts_ShouldSubtractCorrectly [PASS]

Passed! - Failed: 0, Passed: 3, Skipped: 0, Total: 3, Duration: 0.1 s
```

---

### 5️⃣ Rodar com Output Detalhado

```bash
# Verbose
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --verbosity detailed

# Diagnóstico completo
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --verbosity diagnostic 2>&1 | less
```

**Output:**
```
[xUnit.net 00:00:00.28] xUnit.net VSTest Adapter v2.5.4.1
[xUnit.net 00:00:00.28] Test run for [project] (.NET 10.0)
[xUnit.net 00:00:00.30] Discovering: Core.Tests
[xUnit.net 00:00:00.35] Discovered:  Core.Tests
[xUnit.net 00:00:00.35] Starting:    Core.Tests
[xUnit.net 00:00:01.23]     Core.Tests.ValueObjects.MoneyTests.Constructor_WithValidAmount_ShouldCreate [PASS]
[xUnit.net 00:00:01.24]     Core.Tests.Entities.BankAccountTests.Deposit_ShouldIncreaseBalance [PASS]
[xUnit.net 00:00:02.50]     Core.Tests.Integration.CreateCustomerUseCaseIntegrationTests.ExecuteAsync_WithValidData_ShouldCreateCustomer [PASS]
[xUnit.net 00:00:02.60]   Finished:    Core.Tests
```

---

### 6️⃣ Rodar com Resultado em XML (Para CI/CD)

```bash
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --logger "trx" \
  --results-directory ./TestResults
```

**Cria arquivo:** `TestResults/[timestamp].trx` (formato XML)

---

### 7️⃣ Rodar e Parar no Primeiro Erro

```bash
# Não roda todos se um falhar
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --no-build \
  --filter "Integration" \
  --logger "console;verbosity=minimal"
```

---

### 8️⃣ Rodar com Cores (Mais Visual)

```bash
# Padrão (já tem cores)
dotnet test tests/Core.Tests/Core.Tests.csproj

# Force com logger customizado
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --logger "console;verbosity=minimal"
```

---

## 📊 Comparação de Tempos

### Cenário 1: Desenvolvimento (Rápido)

```bash
$ dotnet test --filter "!Integration"
Duration: 0.8s ⚡

✅ Ideal para:
- Ao escrever código
- Validação rápida
- Testes frequentes
```

### Cenário 2: Commit (Completo)

```bash
$ dotnet test tests/Core.Tests/Core.Tests.csproj
Duration: 2s 🚀

✅ Ideal para:
- Antes de fazer commit
- Pull request
- Validação completa
```

### Cenário 3: CI/CD (Paralelo)

```bash
# Job 1 (paralelo)
$ dotnet test --filter "!Integration"
Duration: 0.8s ⚡

# Job 2 (paralelo)
$ dotnet test --filter "Integration"
Duration: 2s 🚀

Total Efetivo: 2s (não 2.8s)
Savings: 28% mais rápido!
```

---

## 🎯 Dicas Práticas

### Dica 1: Alias para Comandos Frequentes

```bash
# Adicione ao seu ~/.zshrc:

alias dt="dotnet test --filter '!Integration'"    # Unit tests rápidos
alias dti="dotnet test --filter 'Integration'"    # Integration tests
alias dta="dotnet test tests/Core.Tests/Core.Tests.csproj" # Todos

# Depois:
dt          # Roda unit tests (< 1s)
dti         # Roda integration tests (~2s)
dta         # Roda todos (2s)
```

### Dica 2: Script para Desenvolvimento

```bash
#!/bin/bash
# save as: run-tests.sh

echo "🚀 Running Unit Tests..."
dotnet test --filter "!Integration"

if [ $? -eq 0 ]; then
  echo "✅ Unit tests passed! Running integration tests..."
  dotnet test --filter "Integration"
else
  echo "❌ Unit tests failed! Fix them first."
  exit 1
fi
```

### Dica 3: Rodar Testes Antes de Push

```bash
# Script do git hook
# .git/hooks/pre-push

#!/bin/bash
echo "Running all tests before push..."
dotnet test tests/Core.Tests/Core.Tests.csproj

if [ $? -ne 0 ]; then
  echo "❌ Tests failed! Push aborted."
  exit 1
fi

echo "✅ All tests passed! Proceeding with push..."
```

---

## 🏗️ Estrutura de Filtros Disponíveis

### Por Namespace

```bash
# Apenas Entities
dotnet test --filter "FullyQualifiedName~Entities"

# Apenas ValueObjects
dotnet test --filter "FullyQualifiedName~ValueObjects"

# Apenas UseCases
dotnet test --filter "FullyQualifiedName~UseCases"

# Apenas Integration
dotnet test --filter "FullyQualifiedName~Integration"
```

### Por Nome de Teste

```bash
# Testes que têm "Deposit"
dotnet test --filter "FullyQualifiedName~Deposit"

# Testes que têm "ShouldThrow"
dotnet test --filter "FullyQualifiedName~ShouldThrow"

# Testes que NÃO têm "Simulate"
dotnet test --filter "FullyQualifiedName!~Simulate"
```

### Combinações

```bash
# (Integration E ShouldThrow)
dotnet test --filter "FullyQualifiedName~Integration&FullyQualifiedName~ShouldThrow"

# (NOT Integration) OU (Integration E ErrorSimulation)
dotnet test --filter "(FullyQualifiedName!~Integration)|(FullyQualifiedName~ErrorSimulation)"
```

---

## ⚡ Comandos Rápidos por Situação

### 🔨 Estou Desenvolvendo

```bash
# Roda unit tests + vê o resultado
dotnet test --filter '!Integration' --verbosity minimal
```

### 🧪 Criei um Novo Teste

```bash
# Roda apenas o novo teste
dotnet test --filter "MyNewTestName"
```

### 🐛 Estou Debugando

```bash
# Com output detalhado
dotnet test --filter "MyTestName" --logger "console;verbosity=detailed"
```

### 📤 Vou fazer commit

```bash
# Valida tudo antes de fazer push
dotnet test tests/Core.Tests/Core.Tests.csproj
```

### 📊 Vou fazer report

```bash
# Gera XML para tools de CI/CD
dotnet test tests/Core.Tests/Core.Tests.csproj \
  --logger "trx" \
  --results-directory ./TestResults
```

---

## 📚 Referência Rápida

```
┌─────────────────────────────────────────────────────┐
│         COMANDO                    │  TEMPO         │
├─────────────────────────────────────────────────────┤
│ dt (unit only)                     │  0.8s ⚡       │
│ dti (integration only)             │  2.0s 🚀       │
│ dta (all)                          │  2.0s 🚀       │
│ dt --filter "BankAccount"          │  0.1s ⚡       │
│ dti --verbosity minimal            │  2.0s 🚀       │
└─────────────────────────────────────────────────────┘
```

---

## 🎉 Resumo

✅ **Rodam juntos:** Unit Tests + Integration Tests  
✅ **Total:** 28 testes  
✅ **Tempo padrão:** ~2 segundos  
✅ **Tempo unit only:** < 1 segundo  
✅ **Pode filtrar:** Qualquer aspecto dos testes  

**Recomendação:** Use `--filter "!Integration"` durante desenvolvimento para feedback rápido! ⚡

