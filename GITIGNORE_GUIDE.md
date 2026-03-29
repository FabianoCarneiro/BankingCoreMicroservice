# 📋 Git Ignore Configuration

## Visão Geral

Este projeto possui uma configuração completa de `.gitignore` para evitar que arquivos desnecessários sejam commitados no repositório.

---

## 📁 Arquivos Configurados

### `.gitignore` - Arquivo Principal

Exclui os seguintes tipos de arquivos:

#### 1. **Artefatos de Build** 🔨
```
bin/              # Pasta de compilação binária
obj/              # Pasta de objetos intermediários
*.dll             # Bibliotecas dinâmicas
*.exe             # Executáveis
*.pdb             # Arquivos de debug
```

#### 2. **IDEs e Editores** 💻
```
.vs/              # Visual Studio
.vscode/          # Visual Studio Code
.idea/            # JetBrains Rider
*.sln.iml
*.user
*.userosscache
.vscode-test
```

#### 3. **Logs** 📝
```
logs/             # Diretório de logs
*.log             # Arquivos de log
```

#### 4. **NuGet e Pacotes** 📦
```
packages/         # Pasta de pacotes NuGet
*.nupkg          # Pacotes NuGet
*.snupkg         # Pacotes de símbolos
nuget.config     # Configuração local do NuGet
```

#### 5. **Banco de Dados** 🗄️
```
*.db              # SQLite
*.sqlite          # SQLite alternativo
*.sqlite3
*.mdf             # SQL Server
*.ldf             # SQL Server log
customer.db       # Arquivos específicos do projeto
banking.db
```

#### 6. **Configurações Locais** ⚙️
```
.env              # Variáveis de ambiente
.env.local
.env.*.local
appsettings.local.json
appsettings.Development.local.json
local.settings.json
```

#### 7. **Arquivos do Sistema** 🖥️
```
.DS_Store         # macOS
Thumbs.db         # Windows
.Trash-*          # Linux
Desktop.ini
```

#### 8. **Testes e Cobertura** ✅
```
TestResults/      # Resultados de testes
*.trx             # Testes em formato XML
*.coverage        # Cobertura de código
*.coveragexml
```

#### 9. **Docker** 🐳
```
docker-compose.override.yml
docker-compose.local.yml
.env.docker
```

#### 10. **Temporários** 🗑️
```
tmp/
temp/
*.tmp
*.bak
*.backup
*.swp
*.swo
*~
```

---

## 📄 `.gitattributes`

Controla como o Git trata diferentes tipos de arquivo:

### Line Endings (Quebras de Linha)
```
*.cs              text eol=lf      # C# sempre com LF (Unix)
*.bat             text eol=crlf    # Batch sempre com CRLF (Windows)
*.sh              text eol=lf      # Shell script sempre com LF
*.json            text eol=lf      # JSON sempre com LF
*.md              text eol=lf      # Markdown sempre com LF
```

### Arquivos Binários
```
*.dll             binary            # DLLs sempre como binário
*.exe             binary            # Executáveis sempre como binário
*.png             binary            # Imagens sempre como binário
*.db              binary            # Bancos de dados como binário
```

**Benefício**: Evita problemas de line endings entre Windows/Mac/Linux

---

## 🐳 `.dockerignore`

Exclui arquivos do build do Docker:

```
.docker/
.env.docker
.env.docker.local
volumes/
data/
db/
```

---

## ✅ Verificar Configuração

### Ver arquivos rastreados
```bash
git ls-files | head -20
```

### Verificar qual arquivo seria ignorado
```bash
git check-ignore -v bin/Debug/Core.API.dll
```

### Ver padrão de ignore
```bash
cat .gitignore | grep -v "^#"
```

---

## 🔄 Sincronizar Repositório Existente

Se você já havia commitado arquivos que agora devem ser ignorados:

```bash
# Remover do índice (não deleta arquivos locais)
git rm -r --cached bin/ obj/ logs/ --ignore-unmatch

# Fazer commit
git commit -m "Remove build artifacts from git tracking"

# Fazer push
git push origin main
```

---

## 📊 Tamanho do Repositório

### Antes (com binários)
```
Repository size: ~500 MB
Build artifacts: ~450 MB
Source code: ~50 MB
```

### Depois (com .gitignore)
```
Repository size: ~50 MB
Build artifacts: 0 MB (local only)
Source code: ~50 MB
```

**Economia: 90% de redução!** 🚀

---

## 📋 Checklist - O que NÃO commitar

- ❌ Arquivos `bin/` e `obj/`
- ❌ Arquivos `.dll`, `.exe`, `.pdb`
- ❌ Arquivos `*.db`, `*.sqlite`
- ❌ Arquivos `.env` com credenciais
- ❌ Arquivos `appsettings.local.json`
- ❌ Diretório `.vs/`, `.vscode/`, `.idea/`
- ❌ Arquivos de log
- ❌ Arquivos temporários

---

## ✅ Checklist - O que COMMITAR

- ✅ Código fonte (`.cs`, `.json`, `.md`)
- ✅ Arquivos de projeto (`.csproj`, `.sln`)
- ✅ Configurações padrão (`appsettings.json`)
- ✅ Dockerfiles e docker-compose.yml
- ✅ Scripts (`.sh`, `.bat`)
- ✅ Documentação

---

## 🚀 Exemplo: Fluxo Correto

```bash
# 1. Compilar o projeto (cria bin/ e obj/)
dotnet build

# 2. Verificar status (bin/ e obj/ não aparecem)
git status

# 3. Adicionar mudanças
git add src/MyClass.cs

# 4. Commit
git commit -m "Add new feature"

# 5. Push
git push origin main
```

---

## ⚠️ Troubleshooting

### Acidentalmente commitei um arquivo grande

```bash
# Remover do histórico
git rm --cached large_file.dll
echo "large_file.dll" >> .gitignore
git commit -m "Remove large file from git tracking"
```

### Precisar forçar adicionar arquivo ignorado

```bash
git add -f arquivo_ignorado.txt
```

### Ver histórico de um arquivo ignorado

```bash
# Isso não funcionará se estiver em .gitignore
# Use git log com path
git log -p -- arquivo_ignorado.txt
```

---

## 📚 Referências

- [Git Documentation - gitignore](https://git-scm.com/docs/gitignore)
- [Git Documentation - gitattributes](https://git-scm.com/docs/gitattributes)
- [GitHub .gitignore Templates](https://github.com/github/gitignore)

---

## 📞 Dúvidas?

Se um arquivo ou pasta precisa ser ignorado:

1. Adicione o padrão em `.gitignore`
2. Execute: `git rm -r --cached caminho/`
3. Faça commit e push

---

**Resultado**: Repositório limpo, rápido e profissional! ✨
