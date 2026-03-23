# Exemplos de Uso da API

## 🚀 Base URL
```
https://localhost:5001/api
```

## 📋 Endpoints Disponíveis

### 1️⃣ Criar Cliente (KYC)

**Endpoint**: `POST /customers`

**Request**:
```bash
curl -X POST https://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "123.456.789-09",
    "name": "João Silva",
    "email": "joao@example.com",
    "phoneNumber": "11987654321"
  }'
```

**Response (201 Created)**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "cpf": "123.456.789-09",
  "name": "João Silva",
  "email": "joao@example.com",
  "phoneNumber": "11987654321",
  "createdAt": "2026-03-23T10:30:00Z",
  "isActive": true
}
```

---

### 2️⃣ Criar Conta Corrente

**Endpoint**: `POST /accounts`

**Request**:
```bash
curl -X POST https://localhost:5001/api/accounts \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "550e8400-e29b-41d4-a716-446655440000"
  }'
```

**Response (201 Created)**:
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "accountNumber": "1234567890",
  "branch": "0001",
  "balance": 0.00,
  "currency": "BRL",
  "createdAt": "2026-03-23T10:35:00Z",
  "isActive": true
}
```

---

### 3️⃣ Obter Saldo

**Endpoint**: `GET /accounts/{accountNumber}/balance`

**Request**:
```bash
curl https://localhost:5001/api/accounts/1234567890/balance
```

**Response (200 OK)**:
```json
{
  "accountNumber": "1234567890",
  "balance": 1500.00,
  "currency": "BRL"
}
```

---

### 4️⃣ Obter Extrato

**Endpoint**: `GET /accounts/{accountNumber}/statement`

**Request**:
```bash
curl https://localhost:5001/api/accounts/1234567890/statement
```

**Response (200 OK)**:
```json
{
  "accountNumber": "1234567890",
  "balance": 1500.00,
  "transactions": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440002",
      "type": "Deposit",
      "amount": 1000.00,
      "description": "Depósito",
      "createdAt": "2026-03-23T10:40:00Z"
    },
    {
      "id": "770e8400-e29b-41d4-a716-446655440003",
      "type": "Deposit",
      "amount": 500.00,
      "description": "Depósito",
      "createdAt": "2026-03-23T10:45:00Z"
    }
  ]
}
```

---

### 5️⃣ Realizar Transferência (TED/PIX)

**Endpoint**: `POST /accounts/transfer`

**Request**:
```bash
curl -X POST https://localhost:5001/api/accounts/transfer \
  -H "Content-Type: application/json" \
  -d '{
    "fromAccountNumber": "1234567890",
    "toAccountNumber": "9876543210",
    "amount": 250.00
  }'
```

**Response (204 No Content)**: Sem conteúdo (sucesso)

---

## 📊 Fluxo de Teste Completo

```bash
#!/bin/bash

# 1. Criar primeiro cliente
CUSTOMER1=$(curl -s -X POST https://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "111.111.111-11",
    "name": "Cliente Um",
    "email": "cliente1@example.com",
    "phoneNumber": "11999999991"
  }' | jq -r '.id')

# 2. Criar segundo cliente
CUSTOMER2=$(curl -s -X POST https://localhost:5001/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "cpf": "222.222.222-22",
    "name": "Cliente Dois",
    "email": "cliente2@example.com",
    "phoneNumber": "11999999992"
  }' | jq -r '.id')

# 3. Criar conta para cliente 1
ACCOUNT1=$(curl -s -X POST https://localhost:5001/api/accounts \
  -H "Content-Type: application/json" \
  -d "{\"customerId\": \"$CUSTOMER1\"}" | jq -r '.accountNumber')

# 4. Criar conta para cliente 2
ACCOUNT2=$(curl -s -X POST https://localhost:5001/api/accounts \
  -H "Content-Type: application/json" \
  -d "{\"customerId\": \"$CUSTOMER2\"}" | jq -r '.accountNumber')

# 5. Depositar R$ 500 na conta 1
curl -X POST https://localhost:5001/api/accounts/deposit \
  -H "Content-Type: application/json" \
  -d "{\"accountNumber\": \"$ACCOUNT1\", \"amount\": 500}"

# 6. Consultar saldo conta 1
echo "Saldo Conta 1:"
curl -s https://localhost:5001/api/accounts/$ACCOUNT1/balance | jq '.'

# 7. Consultar saldo conta 2
echo "Saldo Conta 2:"
curl -s https://localhost:5001/api/accounts/$ACCOUNT2/balance | jq '.'

# 8. Transferir R$ 150 de conta 1 para conta 2
curl -X POST https://localhost:5001/api/accounts/transfer \
  -H "Content-Type: application/json" \
  -d "{\"fromAccountNumber\": \"$ACCOUNT1\", \"toAccountNumber\": \"$ACCOUNT2\", \"amount\": 150}"

# 9. Verificar novo saldo conta 1
echo "Novo Saldo Conta 1:"
curl -s https://localhost:5001/api/accounts/$ACCOUNT1/balance | jq '.'

# 10. Verificar novo saldo conta 2
echo "Novo Saldo Conta 2:"
curl -s https://localhost:5001/api/accounts/$ACCOUNT2/balance | jq '.'

# 11. Ver extrato completo conta 1
echo "Extrato Conta 1:"
curl -s https://localhost:5001/api/accounts/$ACCOUNT1/statement | jq '.'
```

---

## ⚠️ Códigos de Erro

| Código | Descrição |
|--------|-----------|
| 200    | OK - Requisição bem-sucedida |
| 201    | Created - Recurso criado com sucesso |
| 204    | No Content - Operação realizada com sucesso (sem conteúdo) |
| 400    | Bad Request - Dados inválidos ou operação não permitida |
| 404    | Not Found - Recurso não encontrado |
| 500    | Internal Server Error - Erro no servidor |

---

## 🔍 Validações

### CPF Válido
- Deve ter 11 dígitos
- Digitos verificadores devem estar corretos
- Não pode ser todos iguais

### Transferência
- Conta de origem e destino devem existir e estar ativas
- Saldo deve ser suficiente
- Valor deve ser positivo

### Cliente
- CPF obrigatório e válido
- Name, Email, PhoneNumber obrigatórios

---

**Versão da API**: 1.0.0 | **Data**: 2026-03-23
