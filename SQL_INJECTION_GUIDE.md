# 🚨 SQL Injection - Explicação e Exemplos

## O que é SQL Injection?

SQL Injection é uma vulnerabilidade crítica onde um atacante insere código SQL malicioso através de campos de entrada, permitindo acessar, modificar ou deletar dados não autorizados.

---

## ❌ VULNERÁVEL: O Método Fictício Adicionado

```csharp
public async Task<BankAccount?> GetByAccountNumberVulnerableAsync(string accountNumber)
{
    // ❌ NUNCA faça isso!
    var query = $"SELECT * FROM BankAccounts WHERE AccountNumber = '{accountNumber}'";
    
    var result = await _context.BankAccounts
        .FromSqlRaw(query)
        .FirstOrDefaultAsync();
    
    return result;
}
```

### Por que é perigoso?

A entrada do usuário é **concatenada diretamente na query SQL**. Se o usuário inserir:

```sql
' OR '1'='1
```

A query fica:
```sql
SELECT * FROM BankAccounts WHERE AccountNumber = '' OR '1'='1'
```

**Resultado:** Retorna TODAS as contas bancárias (o `'1'='1'` sempre é verdadeiro)!

---

## 🎯 Exemplos de Ataques

### Ataque 1: Bypassar Autenticação

**Entrada:** `admin' --`

**Query Original:**
```sql
SELECT * FROM Users WHERE Username = 'admin' AND Password = 'senha123'
```

**Query com Injection:**
```sql
SELECT * FROM Users WHERE Username = 'admin' --' AND Password = 'senha123'
-- O comentário "--" anula a verificação de senha!
```

**Resultado:** Login sem senha! 🔓

---

### Ataque 2: Deletar Dados

**Entrada:** `'; DROP TABLE BankAccounts; --`

**Query Original:**
```sql
SELECT * FROM BankAccounts WHERE AccountNumber = '123456'
```

**Query com Injection:**
```sql
SELECT * FROM BankAccounts WHERE AccountNumber = ''; DROP TABLE BankAccounts; --'
```

**Resultado:** Tabela deletada! 💥

---

### Ataque 3: Extrair Dados Sensíveis

**Entrada:** `' UNION SELECT CardNumber, CVV FROM CreditCards --`

**Query Original:**
```sql
SELECT AccountNumber FROM BankAccounts WHERE AccountNumber = '123456'
```

**Query com Injection:**
```sql
SELECT AccountNumber FROM BankAccounts WHERE AccountNumber = '' 
UNION SELECT CardNumber, CVV FROM CreditCards --'
```

**Resultado:** Números de cartão vazados! 💳

---

## ✅ SEGURO: O Método Corrigido

```csharp
public async Task<BankAccount?> GetByAccountNumberSafeAsync(string accountNumber)
{
    // ✅ CORRETO: Usando parameterized query
    var result = await _context.BankAccounts
        .FromSqlInterpolated($"SELECT * FROM BankAccounts WHERE AccountNumber = {accountNumber}")
        .FirstOrDefaultAsync();
    
    return result;
}
```

### Por que é seguro?

Com **parameterized queries**, a entrada do usuário é **tratada como dado**, não como SQL:

```sql
DECLARE @accountNumber nvarchar(max) = ''  -- O valor é um PARÂMETRO
SELECT * FROM BankAccounts WHERE AccountNumber = @accountNumber
```

**Resultado:** Mesmo que o usuário insira `' OR '1'='1'`, será procurado literalmente por uma conta com esse nome (não existe).

---

## 🛡️ 3 Formas de Prevenir SQL Injection

### 1. **Parameterized Queries** ⭐ (RECOMENDADO)

```csharp
// ✅ Entity Framework (LINQ)
var account = await _context.BankAccounts
    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

// ✅ Interpolated Queries
var result = await _context.BankAccounts
    .FromSqlInterpolated($"SELECT * FROM BankAccounts WHERE AccountNumber = {accountNumber}")
    .ToListAsync();

// ✅ Raw SQL com parâmetros
var result = await _context.BankAccounts
    .FromSqlRaw("SELECT * FROM BankAccounts WHERE AccountNumber = @account", 
        new SqlParameter("@account", accountNumber))
    .ToListAsync();
```

### 2. **Validação de Entrada**

```csharp
// Validar formato antes de usar
if (!Guid.TryParse(accountNumber, out var guid))
{
    throw new ArgumentException("Account number inválido");
}
```

### 3. **Princípio do Menor Privilégio**

- Use contas de banco com **permissões mínimas**
- Banco de testes ≠ Banco de produção
- Limite acesso ao que é necessário

---

## 🔍 Como Testar Vulnerabilidades

### Teste 1: Bypassing WHERE Clause

```csharp
[Test]
public void GetByAccountNumber_WithSqlInjection_ShouldNotReturnOtherAccounts()
{
    // Arrange
    var injection = "' OR '1'='1";
    
    // Act
    var result = await repository.GetByAccountNumberVulnerableAsync(injection);
    
    // Assert - FALHA: Retorna contas indevidas
    Assert.IsNull(result); // ❌ Falhará em código vulnerável
}
```

### Teste 2: Verificar Proteção

```csharp
[Test]
public void GetByAccountNumberSafe_WithSqlInjection_ShouldReturnNull()
{
    // Arrange
    var injection = "' OR '1'='1";
    
    // Act
    var result = await repository.GetByAccountNumberSafeAsync(injection);
    
    // Assert - PASSA: Nenhuma conta encontrada
    Assert.IsNull(result); // ✅ Passa com código seguro
}
```

---

## 📊 Comparação

| Aspecto | ❌ Vulnerável | ✅ Seguro |
|---------|--------|--------|
| **Concatenação** | String concatenation | Parameterized |
| **Input vs SQL** | Misturado | Separado |
| **SQL Injection** | ✗ Possível | ✓ Impossível |
| **Performance** | Sem cache | Com cache |
| **Manutenibilidade** | Difícil | Fácil |

---

## 🎓 Lições Aprendidas

1. **NUNCA** use string concatenation em SQL queries
2. **SEMPRE** use parameterized queries (LINQ ou `FromSqlInterpolated`)
3. **VALIDE** entrada de usuário antes de usar
4. **TESTE** aplicação com payloads maliciosos
5. **CONFIGURE** banco de dados com privilégios mínimos

---

## 🔗 Referências

- [OWASP Top 10 - A03:2021 Injection](https://owasp.org/Top10/A03_2021-Injection/)
- [Entity Framework Core - Mitigating SQL Injection](https://learn.microsoft.com/en-us/ef/core/querying/raw-sql)
- [SonarCloud SQL Injection Detection](https://rules.sonarsource.com/csharp/type/Vulnerability/RSPEC-2065)

---

## ⚠️ DISCLAIMER

Este documento é **apenas educacional**. Os métodos vulneráveis no `BankAccountRepository.cs` são para fins de aprendizado e demostração de vulnerabilidades. **NUNCA** implemente código similar em produção!

