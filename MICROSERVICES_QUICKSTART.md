# 🚀 Guia Rápido - Arquitetura de Microserviços

## Opção 1: Execução Local (Recomendado para Desenvolvimento)

### Terminal 1: Customer Service

```bash
cd src/Customer.Service/Customer.API
dotnet run
```

**Output esperado**:
```
[HH:MM:SS INF] Now listening on: http://localhost:5001
```

**Swagger**: http://localhost:5001/swagger

---

### Terminal 2: Banking Core Service

```bash
cd src/Core.API
dotnet run
```

**Output esperado**:
```
[HH:MM:SS INF] Now listening on: http://localhost:5000
```

**Swagger**: http://localhost:5000/swagger

---

## Opção 2: Docker Compose (Produção)

### Build e execute os dois serviços

```bash
docker-compose up -d
```

### Verificar status

```bash
docker-compose ps
```

### Acessar Swagger

- **Customer Service**: http://localhost:5001/swagger
- **Banking Core Service**: http://localhost:5000/swagger

### Parar serviços

```bash
docker-compose down
```

---

## 🧪 Testes Rápidos

### 1. Criar um Cliente

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

**Resposta**:
```json
{
  "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "cpf": "11144477735",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11999999999",
  "createdAt": "2025-03-25T10:30:00",
  "updatedAt": null,
  "isActive": true
}
```

### 2. Listar Clientes

```bash
curl http://localhost:5001/api/customers
```

### 3. Obter Cliente por ID

```bash
curl http://localhost:5001/api/customers/a1b2c3d4-e5f6-7890-1234-567890abcdef
```

### 4. Atualizar Cliente

```bash
curl -X PUT http://localhost:5001/api/customers/a1b2c3d4-e5f6-7890-1234-567890abcdef \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao.silva@example.com",
    "phoneNumber": "11988888888"
  }'
```

### 5. Deletar Cliente

```bash
curl -X DELETE http://localhost:5001/api/customers/a1b2c3d4-e5f6-7890-1234-567890abcdef
```

---

## 📊 Arquivos de Banco de Dados

### SQLite (Padrão)

**Customer Service**:
```
customer.db
```

**Banking Core Service**:
```
banking.db
```

**Limpar bancos de dados**:
```bash
rm customer.db banking.db
```

---

## 📝 Estrutura de Diretórios

```
src/
├── Core/                              # Banking Core Service
│   ├── Core.Domain/
│   ├── Core.Application/
│   ├── Core.Infrastructure/
│   └── Core.API/
│
├── Customer.Service/                  # Customer Service
│   ├── Customer.Domain/
│   ├── Customer.Application/
│   ├── Customer.Infrastructure/
│   └── Customer.API/
│
└── Shared/                            # Código Compartilhado
    ├── Shared.DTOs/                   # DTOs para comunicação
    └── Shared.HttpClients/            # Clientes HTTP
```

---

## 🔍 Verificar Comunicação Entre Serviços

### Banking Core chama Customer Service

Se você criar um endpoint no Banking Core que precisa de dados do cliente:

```csharp
// No Banking Core
var customerClient = new CustomerServiceClient(httpClient);
customerClient.BaseUrl = "http://localhost:5001";  // Customer Service URL

var customer = await customerClient.GetCustomerByIdAsync(customerId);
```

---

## 📈 Monitorar Logs

### Docker Compose

```bash
# Customer Service
docker-compose logs -f customer-service

# Banking Core
docker-compose logs -f banking-core-service

# Ambos
docker-compose logs -f
```

### Local

Os logs são salvos em:
- `logs/customer-service-*.txt`
- `logs/banking-core-.txt`

---

## 🛠️ Troubleshooting

### Porta em uso

```bash
# Liberar porta 5001
lsof -i :5001
kill -9 <PID>

# Liberar porta 5000
lsof -i :5000
kill -9 <PID>
```

### Banco de dados corrompido

```bash
rm *.db
# Reexecutar o serviço
```

### Build falhou

```bash
dotnet clean
dotnet build MicroserviceArchitecture.sln
```

---

## 📚 Documentação Completa

Para mais detalhes, consulte: [MICROSERVICES_IMPLEMENTATION.md](MICROSERVICES_IMPLEMENTATION.md)

---

## ✨ Próximos Passos

- [ ] Implementar Circuit Breaker (Polly)
- [ ] Adicionar Message Queue (RabbitMQ)
- [ ] Implementar API Gateway (Ocelot)
- [ ] Adicionar autenticação OAuth2
- [ ] Observabilidade com ELK Stack
- [ ] Testes de integração com TestContainers

---

**Bora começar!** 🚀
