# 🎓 Guia Prático: Simulando Erros em Testes de Integração

## 📌 Resumo da Sessão

Você perguntou: **"Como simular um erro para validar se o teste de integração está correto?"**

### Resposta Completa ✅

Criamos **4 técnicas práticas** para simular erros e validar testes:

---

## 🛠️ Técnica 1: Quebrar a Persistência

**Objetivo**: Validar que o teste realmente verifica persistência em banco de dados

```csharp
[Fact]
public async Task ValidateTestChecksPersistence()
{
    var dbContext = _fixture.DbContext;
    var customer = new Customer(...);
    
    dbContext.Customers.Add(customer);
    // ❌ NÃO SALVAR:
    // await dbContext.SaveChangesAsync();
    
    // Este teste DEVE falhar aqui
    var saved = await dbContext.Customers.FindAsync(customer.Id);
    Assert.NotNull(saved); // ❌ Falha porque não salvou
}
```

**Como Rodar:**
```bash
dotnet test --filter "ValidatePersistence"
```

**Resultado Esperado:**
```
❌ FAILED - Object reference not set to an instance of an object
```

**O que aprende:** Seu teste NÃO está validando persistência real!

---

## 🛠️ Técnica 2: Assertion Errada

**Objetivo**: Validar que assertions estão testando os valores corretos

```csharp
[Fact]
public async Task ValidateAssertionIsCorrected()
{
    var result = await useCase.ExecuteAsync(input);
    
    // ❌ Assertion ERRADA:
    Assert.Equal("Maria", result.Name); // Esperado "João Silva"
    
    // ✅ Assertion CORRETA:
    // Assert.Equal("João Silva", result.Name);
}
```

**Como Rodar:**
```bash
dotnet test --filter "AssertionIsCorrected"
```

**Resultado Esperado:**
```
❌ FAILED - Expected: "Maria", Actual: "João Silva"
```

**O que aprende:** Seus assertions estão realmente validando valores!

---

## 🛠️ Técnica 3: Dados Inválidos

**Objetivo**: Validar que o teste falha com dados inválidos

```csharp
[Fact]
public async Task ValidateCpfValidation()
{
    var useCase = new CreateCustomerUseCase(repository);
    
    var input = new CreateCustomerDTO
    {
        CPF = "12345678901", // ❌ Checksum inválido
        Name = "Test",
        Email = "test@example.com",
        PhoneNumber = "11999999999"
    };
    
    // Este teste DEVE falhar
    var result = await useCase.ExecuteAsync(input);
    Assert.NotNull(result); // ❌ Falha com ArgumentException
}
```

**Como Rodar:**
```bash
dotnet test --filter "CpfValidation"
```

**Resultado Esperado:**
```
❌ FAILED - System.ArgumentException: CPF inválido
```

**O que aprende:** CPF validation está funcionando!

---

## 🛠️ Técnica 4: Simular Exceção de Banco

**Objetivo**: Validar que o teste captura exceções do banco de dados

```csharp
[Fact]
public async Task ValidateDatabaseErrorHandling()
{
    // Mock que lança exceção
    var mockRepository = new Mock<ICustomerRepository>();
    mockRepository
        .Setup(r => r.AddAsync(It.IsAny<Customer>()))
        .ThrowsAsync(new InvalidOperationException("DB Connection Failed"));
    
    var useCase = new CreateCustomerUseCase(mockRepository.Object);
    var input = new CreateCustomerDTO { ... };
    
    // ✅ Deve lançar exceção
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => useCase.ExecuteAsync(input)
    );
}
```

**Como Rodar:**
```bash
dotnet test --filter "DatabaseErrorHandling"
```

**Resultado Esperado:**
```
✅ PASSED - Exceção foi capturada corretamente
```

**O que aprende:** Seu teste está capturando exceções!

---

## 🛠️ Técnica 5: Validar Isolamento

**Objetivo**: Validar que dados não vazam entre testes

```csharp
[Fact]
public async Task Test1_CreateCustomer()
{
    var count1 = _fixture.DbContext.Customers.Count(); // 0
    // Criar cliente
    var count2 = _fixture.DbContext.Customers.Count(); // 1
}

[Fact]
public async Task Test2_ShouldHaveCleanDatabase()
{
    // ✅ Deve estar vazio se isolamento funciona
    var count = _fixture.DbContext.Customers.Count();
    Assert.Equal(0, count); // ✅ PASSED se isolamento OK
}
```

**Como Rodar:**
```bash
dotnet test --filter "Isolation"
```

**Resultado Esperado:**
```
✅ PASSED - Isolamento funcionando (cada teste começa limpo)
OU
❌ FAILED - Dados vazaram de teste anterior (isolamento quebrado)
```

---

## 📊 Resultados Reais deste Projeto

### Execução Completa:
```
Passed:   19 ✅
Failed:   4  ❌ (Isolamento quebrado - esperado!)
Skipped:  5  ⏭️  (Para estudo)
Total:    28 testes

Duration: 2 segundos
```

### Análise:
- ✅ **19 testes passaram** - Testes principais funcionam
- ❌ **4 testes falharam** - Isolamento precisa correção
- ⏭️ **5 testes pulados** - Exemplos educacionais

### Descoberta Importante:
```
O SQLite Fixture NÃO limpa dados entre testes!

Cada teste vê dados do teste anterior.
Isso é um problema de isolamento.

Solução: Adicionar transações que são revertidas.
```

---

## ✅ Checklist: Como Validar Seus Testes

```bash
# 1. Teste passa com dados válidos?
✅ dotnet test --filter "WithValidData"

# 2. Teste falha com dados inválidos?
✅ dotnet test --filter "WithInvalidData"

# 3. Teste valida persistência?
✅ Comente SaveChangesAsync() - deve falhar

# 4. Assertions estão corretas?
✅ Mude assertion - deve falhar

# 5. Isolamento funciona?
✅ Rodar dois testes - cada um começa limpo?

# 6. Exceções são capturadas?
✅ Teste deve falhar com ThrowsAsync

# 7. Fixture está injetado?
✅ Teste deve passar com Assert.NotNull(_fixture)

# 8. Suite completa passa?
✅ dotnet test tests/Core.Tests/Core.Tests.csproj
```

---

## 📚 Arquivos Criados

1. **TESTING_ERROR_VALIDATION.md** (8KB)
   - Guia completo de 10 técnicas de simulação
   - Exemplos práticos com código
   - Checklist de validação

2. **ErrorSimulationTests.cs** (15KB)
   - 12 testes implementados
   - 5 testes educacionais (Skip)
   - 3 testes de validação
   - 4 testes de isolamento

3. **TESTING_VALIDATION_RESULTS.md** (10KB)
   - Análise dos resultados
   - Descobertas importantes
   - Ações recomendadas

4. **TESTING_FIX_ISOLATION.md** (12KB)
   - Solução com transações
   - Comparação antes/depois
   - Como implementar

---

## 🚀 Próximas Ações

### Imediato (Hoje):
- [x] Aprender técnicas de simulação
- [x] Executar testes de erro
- [x] Ver testes falharem/passarem
- [ ] **Estudar os 5 exemplos práticos**

### Curto Prazo (Esta Semana):
- [ ] Implementar solução com transações
- [ ] Atualizar SQLiteIntegrationTestFixture
- [ ] Rodar testes novamente
- [ ] Ver todos os 28 testes passarem

### Médio Prazo (Este Mês):
- [ ] Adicionar mais testes de negócio
- [ ] Validar isolamento
- [ ] Documentar padrões de teste
- [ ] Integrar com CI/CD

---

## 🎯 Comandos Úteis

```bash
# Ver um teste específico falhar
dotnet test --filter "SimulateError_MissingPersistence"

# Ver teste falhar porque dados vazaram
dotnet test --filter "ValidateIsolation"

# Executar todos com output detalhado
dotnet test --logger "console;verbosity=detailed"

# Rodar apenas testes de simulação
dotnet test --filter "ErrorSimulationTests"

# Rodar testes com cores
dotnet test --logger "console;verbosity=minimal"
```

---

## 💡 Dicas Importantes

### ✅ Teste Bem Escrito:
```csharp
// ✅ Tem setup claro
// ✅ Testa uma coisa
// ✅ Tem assertion clara
// ✅ Passa com dados válidos
// ✅ Falha com dados inválidos
// ✅ Não afeta outros testes
```

### ❌ Teste Fraco:
```csharp
// ❌ Setup confuso
// ❌ Testa múltiplas coisas
// ❌ Assertions obscuras
// ❌ Passa com dados inválidos
// ❌ Falha aleatoriamente
// ❌ Afeta outros testes
```

---

## 📞 Suporte

Se tiver dúvidas sobre simulação de erros:

1. **Leia** `TESTING_ERROR_VALIDATION.md` (guia completo)
2. **Estude** `ErrorSimulationTests.cs` (exemplos práticos)
3. **Analise** `TESTING_VALIDATION_RESULTS.md` (resultados)
4. **Implemente** `TESTING_FIX_ISOLATION.md` (solução)

---

## 🎉 Conclusão

Você agora sabe como:

✅ Simular erros intencionais  
✅ Validar que testes realmente testam  
✅ Detectar isolamento quebrado  
✅ Capturar exceções  
✅ Verificar persistência  
✅ Validar assertions  

**Parabéns!** 🚀 Você tem conhecimento de teste de integração de nível avançado!

