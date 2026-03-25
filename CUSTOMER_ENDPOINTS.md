# 📖 Exemplos de Uso - Novo Endpoint de Consulta de Clientes

## 🆕 Novos Endpoints Implementados

### 1️⃣ Criar Cliente
```bash
POST /api/customers
Content-Type: application/json

{
  "cpf": "12345678909",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11987654321"
}
```

**Resposta (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "cpf": "123.456.789-09",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11987654321",
  "createdAt": "2026-03-24T10:30:00Z",
  "isActive": true
}
```

---

### 2️⃣ Obter Cliente por ID (NOVO)
```bash
GET /api/customers/550e8400-e29b-41d4-a716-446655440000
```

**Resposta (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "cpf": "123.456.789-09",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11987654321",
  "createdAt": "2026-03-24T10:30:00Z",
  "isActive": true
}
```

**Resposta (404 Not Found) - Se cliente não existe:**
```json
{
  "error": "Cliente não encontrado"
}
```

**Resposta (400 Bad Request) - Se ID é inválido:**
```json
{
  "error": "ID do cliente inválido"
}
```

---

### 3️⃣ Listar Todos os Clientes (NOVO)
```bash
GET /api/customers
```

**Resposta (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "cpf": "123.456.789-09",
    "name": "João Silva",
    "email": "joao@example.com",
    "phoneNumber": "11987654321",
    "createdAt": "2026-03-24T10:30:00Z",
    "isActive": true
  },
  {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "cpf": "987.654.321-00",
    "name": "Maria Santos",
    "email": "maria@example.com",
    "phoneNumber": "11987654322",
    "createdAt": "2026-03-24T10:35:00Z",
    "isActive": true
  }
]
```

---

## 🔍 Exemplos com cURL

### Criar cliente:
```bash
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "12345678909",
    "name": "João Silva",
    "email": "joao@example.com",
    "phoneNumber": "11987654321"
  }'
```

### Obter cliente por ID:
```bash
curl -X GET http://localhost:5000/api/customers/550e8400-e29b-41d4-a716-446655440000
```

### Listar todos os clientes:
```bash
curl -X GET http://localhost:5000/api/customers
```

---

## 🧪 Exemplos com Postman

### 1. Criar Cliente (POST)
**URL:** `http://localhost:5000/api/customers`

**Headers:**
```
Content-Type: application/json
```

**Body (raw JSON):**
```json
{
  "cpf": "12345678909",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11987654321"
}
```

**Expected Response:** 201 Created

---

### 2. Obter Cliente (GET)
**URL:** `http://localhost:5000/api/customers/550e8400-e29b-41d4-a716-446655440000`

**Method:** GET

**Expected Response:** 200 OK

---

### 3. Listar Todos (GET)
**URL:** `http://localhost:5000/api/customers`

**Method:** GET

**Expected Response:** 200 OK

---

## 📊 Fluxo Completo de Operações

```
1. Criar novo cliente
   POST /api/customers
   ↓
   Retorna: CustomerDTO com ID gerado
   
2. Consultar cliente criado
   GET /api/customers/{id}
   ↓
   Retorna: Dados completos do cliente
   
3. Listar todos os clientes
   GET /api/customers
   ↓
   Retorna: Array com todos os clientes
```

---

## ✅ Testes Unitários Implementados

### GetCustomerByIdUseCase Tests:
- ✅ ExecuteAsync_WithValidId_ShouldReturnCustomer
- ✅ ExecuteAsync_WithInvalidId_ShouldThrowArgumentException
- ✅ ExecuteAsync_WithNonExistentId_ShouldReturnNull

### ListAllCustomersUseCase Tests:
- ✅ ExecuteAsync_ShouldReturnAllCustomers
- ✅ ExecuteAsync_WithNoCustomers_ShouldReturnEmptyList

**Resultado:** 13 testes passando (5 novos + 8 existentes)

---

## 🏗️ Arquitetura Implementada

```
Request
  ↓
CustomersController
  ├─ POST    CreateCustomer()       → CreateCustomerUseCase
  ├─ GET     GetCustomerById()      → GetCustomerByIdUseCase (NOVO)
  └─ GET     ListAllCustomers()     → ListAllCustomersUseCase (NOVO)
  ↓
Use Cases
  ├─ CreateCustomerUseCase
  ├─ GetCustomerByIdUseCase (NOVO)
  └─ ListAllCustomersUseCase (NOVO)
  ↓
ICustomerRepository (Porta)
  ↓
CustomerRepository (Adaptador)
  ↓
Database (SQL Server)
```

---

## 📝 Notas Implementadas

1. **Validação de Input:**
   - ID vazio retorna BadRequest
   - CPF é validado no domínio

2. **Tratamento de Erros:**
   - 404 Not Found quando cliente não existe
   - 400 Bad Request para dados inválidos
   - 500 Internal Server Error para erros inesperados

3. **Padrão REST:**
   - POST para criar (201 Created)
   - GET para consultar (200 OK)
   - Respostas padronizadas com DTOs

4. **Injeção de Dependência:**
   - Todos os Use Cases registrados no DI Container
   - Controllers recebem dependências via construtor

5. **Documentação XML:**
   - Métodos documentados com `///` comments
   - Visível no Swagger UI

---

## 🚀 Próximas Melhorias (Sugestões)

1. **Paginação:** Adicionar limit/offset em ListAllCustomers
2. **Filtros:** Buscar por CPF, nome ou email
3. **Update:** Criar endpoint PUT para atualizar cliente
4. **Delete:** Criar endpoint DELETE para desativar cliente
5. **Soft Delete:** Marcar como inativo ao invés de deletar

