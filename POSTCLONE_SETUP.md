# 🚀 Setup Pós-Clone do Repositório

Após clonar este repositório, siga estes passos para começar.

---

## 1️⃣ Restaurar Dependências NuGet

```bash
dotnet restore
```

**O que faz**: Baixa todos os pacotes NuGet necessários baseado em `*.csproj`

---

## 2️⃣ Compilar o Projeto

```bash
# Build completo
dotnet build MicroserviceArchitecture.sln

# Build específico (ex: Customer Service)
dotnet build src/Customer.Service/Customer.API/Customer.API.csproj
```

**Resultado**: Cria pasta `bin/` e `obj/` (não commitadas)

---

## 3️⃣ Executar Microserviços

### Opção A: Local (2 terminais)

**Terminal 1 - Customer Service (Porta 5001)**
```bash
cd src/Customer.Service/Customer.API
dotnet run
```

**Terminal 2 - Banking Core Service (Porta 5000)**
```bash
cd src/Core.API
dotnet run
```

---

### Opção B: Docker Compose (1 comando)

```bash
docker-compose up -d
```

---

## 4️⃣ Testar API

### Criar Cliente
```bash
curl -X POST http://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "11144477735",
    "name": "João Silva",
    "email": "joao@example.com",
    "phoneNumber": "11999999999"
  }'
```

### Acessar Swagger
- **Customer Service**: http://localhost:5001/swagger
- **Banking Core**: http://localhost:5000/swagger

---

## 5️⃣ Rodar Testes

```bash
dotnet test
```

---

## 📁 Estrutura Pós-Clone

```
.
├── src/
│   ├── Core/                    # Banking Core Service
│   ├── Customer.Service/        # Customer Service
│   └── Shared/                  # Código compartilhado
├── tests/
├── bin/                         # ⚠️ Gerado (não commit)
├── obj/                         # ⚠️ Gerado (não commit)
├── logs/                        # ⚠️ Gerado (não commit)
├── *.db                         # ⚠️ Gerado (não commit)
├── .gitignore                   # ✅ Commit
├── .gitattributes               # ✅ Commit
└── MicroserviceArchitecture.sln # ✅ Commit
```

---

## ⚠️ Arquivos NÃO Commitados

Os seguintes arquivos/pastas **não estão no repositório** mas **serão gerados**:

```
bin/              # Binários compilados
obj/              # Objetos intermediários
logs/             # Arquivos de log
*.db              # Bancos de dados SQLite
*.sqlite          # Bancos de dados SQLite
.vs/              # Visual Studio cache
.vscode/          # VS Code settings
.idea/            # Rider settings
```

Para **limpar** esses arquivos:

```bash
dotnet clean
rm -rf logs/ bin/ obj/ *.db
```

---

## 🔄 Ciclo de Trabalho

```
1. Clone: git clone <repo>
2. Restore: dotnet restore
3. Build: dotnet build
4. Run: dotnet run (em cada serviço)
5. Test: dotnet test
6. Code: Edite os arquivos .cs
7. Build: dotnet build (verifica)
8. Commit: git add . && git commit -m "..."
9. Push: git push origin develop
```

---

## 🐳 Com Docker

```bash
# Iniciar
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar
docker-compose down

# Limpar tudo
docker-compose down -v
```

---

## 📊 Problemas Comuns

### "Arquivo não encontrado após clonar"

**Causa**: Arquivo `.dll` ou log não foi clonado (por design via `.gitignore`)

**Solução**: 
```bash
dotnet build
```

### "Porta 5000 já em uso"

**Solução**:
```bash
# Macintosh/Linux
lsof -i :5000
kill -9 <PID>

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### "Erro de compilação com NuGet"

**Solução**:
```bash
dotnet nuget locals all --clear
dotnet restore
```

---

## ✅ Verificação Rápida

```bash
# Verificar versão .NET
dotnet --version

# Verificar projetos
dotnet sln list

# Verificar build
dotnet build --no-restore

# Verificar testes
dotnet test --no-build
```

---

## 📚 Documentação Disponível

- [MICROSERVICES_IMPLEMENTATION.md](./MICROSERVICES_IMPLEMENTATION.md) - Arquitetura detalhada
- [MICROSERVICES_QUICKSTART.md](./MICROSERVICES_QUICKSTART.md) - Guia rápido
- [GITIGNORE_GUIDE.md](./GITIGNORE_GUIDE.md) - Explicação do .gitignore
- [DATABASE_SETUP.md](./DATABASE_SETUP.md) - Configuração de banco de dados
- [README.md](./README.md) - Visão geral do projeto

---

## 🎯 Próximas Etapas

1. ✅ Clone e build bem-sucedido
2. ⏭️ Executar microserviços
3. ⏭️ Testar endpoints via Swagger
4. ⏭️ Estudar arquitetura de microserviços
5. ⏭️ Adicionar novas funcionalidades

---

**Pronto para começar!** 🚀
