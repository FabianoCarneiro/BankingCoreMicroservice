# 🔍 SonarScanner CLI para .NET

## Linha Completa do SonarScanner CLI

Para análise de código .NET com SonarCloud/SonarQube, use:

```bash
dotnet sonarscanner begin \
  /k:"caixaeconomica_bankingcore" \
  /o:"caixaeconomica" \
  /d:sonar.token="${SONAR_TOKEN}" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
  /d:sonar.coverage.exclusions="**/bin/**,**/obj/**" \
  /d:sonar.exclusions="**/*.md,**/bin/**,**/obj/**"
```

---

## 📋 Argumentos Principais

| Argumento | Valor | Descrição |
|-----------|-------|-----------|
| `/k` | `caixaeconomica_bankingcore` | **Chave do projeto** (obrigatório) |
| `/o` | `caixaeconomica` | **Organização** no SonarCloud (obrigatório) |
| `/d:sonar.token` | `${SONAR_TOKEN}` | **Token de autenticação** (obrigatório) |
| `/d:sonar.host.url` | `https://sonarcloud.io` | URL do SonarCloud (ou seu servidor SonarQube) |
| `/d:sonar.cs.opencover.reportsPaths` | `**/coverage.opencover.xml` | Relatório de cobertura de testes |
| `/d:sonar.coverage.exclusions` | `**/bin/**,**/obj/**` | Excluir pastas de build |
| `/d:sonar.exclusions` | `**/*.md,**/bin/**` | Excluir arquivos adicionais |

---

## 🔄 Fluxo Completo no CI/CD

### Step 1: Begin Analysis
```bash
dotnet sonarscanner begin \
  /k:"caixaeconomica_bankingcore" \
  /o:"caixaeconomica" \
  /d:sonar.token="${SONAR_TOKEN}" \
  /d:sonar.host.url="https://sonarcloud.io"
```

### Step 2: Build do Projeto
```bash
dotnet build
```

### Step 3: Executar Testes com Cobertura
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Step 4: End Analysis
```bash
dotnet sonarscanner end /d:sonar.token="${SONAR_TOKEN}"
```

---

## 🛠️ Para GitHub Actions

```yaml
- name: Begin SonarScanner
  run: |
    dotnet tool install --global dotnet-sonarscanner
    dotnet sonarscanner begin \
      /k:"caixaeconomica_bankingcore" \
      /o:"caixaeconomica" \
      /d:sonar.token="${{ secrets.SONAR_TOKEN }}" \
      /d:sonar.host.url="https://sonarcloud.io"

- name: Build
  run: dotnet build

- name: Test with Coverage
  run: dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

- name: End SonarScanner
  run: dotnet sonarscanner end /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
```

---

## 📊 Argumentos Adicionais Úteis

```bash
# Adicionar branches
/d:sonar.branch.name="main"

# Adicionar pull request
/d:sonar.pullrequest.key="123"
/d:sonar.pullrequest.branch="feature/my-feature"
/d:sonar.pullrequest.base="main"

# Exclusões específicas
/d:sonar.exclusions="**/Tests/**,**/*.Tests/**"

# Problemas a ignorar
/d:sonar.issue.ignore.multicriteria=e1,e2
/d:sonar.issue.ignore.multicriteria.e1.ruleKey=cs:S1234
/d:sonar.issue.ignore.multicriteria.e1.resourceKey=**
```

---

## 🚀 Instalação do SonarScanner

```bash
# Instalar como ferramenta global
dotnet tool install --global dotnet-sonarscanner

# Ou atualizar se já instalada
dotnet tool update --global dotnet-sonarscanner

# Verificar versão
dotnet sonarscanner --version
```

---

## ✅ Exemplo Completo para seu Projeto

Para seu projeto `BankingCoreMicroservice`:

```bash
# 1. Instalar
dotnet tool install --global dotnet-sonarscanner

# 2. Begin
dotnet sonarscanner begin \
  /k:"caixaeconomica_bankingcore" \
  /o:"caixaeconomica" \
  /d:sonar.token="seu_token_aqui" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"

# 3. Build
dotnet build

# 4. Test
dotnet test \
  /p:CollectCoverage=true \
  /p:CoverageFormat=opencover \
  /p:CoverageFileName="coverage.opencover.xml"

# 5. End
dotnet sonarscanner end /d:sonar.token="seu_token_aqui"
```

---

## 🔐 Variáveis de Ambiente (GitHub Actions)

```yaml
env:
  SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
```

Depois use no comando:
```bash
/d:sonar.token="${SONAR_TOKEN}"
```

---

## 📚 Referência Oficial

- **SonarScanner para .NET**: https://docs.sonarcloud.io/advanced-setup/ci-based-analysis/
- **Documentação SonarQube**: https://docs.sonarqube.org/latest/analyzing-source-code/scanners/sonarscanner-for-dotnet/
- **Tokens SonarCloud**: https://sonarcloud.io/account/security

---

## 💡 Dicas

✅ **Use aspas simples** em scripts bash para evitar expansões  
✅ **Sempre comece com `begin`** e termine com `end`  
✅ **Instale globalmente** para reutilizar em múltiplos projetos  
✅ **Configure exclusões** para evitar analisar testes e dependências  
✅ **Collect coverage** para análise mais completa  

