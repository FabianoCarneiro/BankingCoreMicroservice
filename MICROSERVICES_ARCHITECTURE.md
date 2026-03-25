# 🏗️ Proposta de Arquitetura em Microsserviços
**Banking Core - Evolução para Microsserviços**

---

## 📊 Visão Geral da Migração

### Monolito Atual (1 Serviço)
```
┌────────────────────────────────────────────┐
│         Banking Core Monolito              │
├────────────────────────────────────────────┤
│  • Customers (KYC)                         │
│  • Accounts (Contas)                       │
│  • Transfers (Transferências)              │
│  • Notifications (Notificações)            │
│  • Database (SQL Server)                   │
└────────────────────────────────────────────┘
```

### Arquitetura em Microsserviços (5 Serviços)
```
                    ┌─────────────────┐
                    │   API Gateway   │ (Nginx / Kong)
                    └────────┬────────┘
         ┌──────────┬────────┼────────┬──────────┐
         │          │        │        │          │
    ┌────▼───┐ ┌───▼─────┐ ┌▼──────┐ ┌▼───────┐ ┌▼──────────┐
    │Customer│ │ Account │ │Transfer│ │Notif.  │ │ Audit Log│
    │Service │ │ Service │ │Service │ │Service │ │ Service  │
    └────┬───┘ └───┬─────┘ └┬──────┘ └┬───────┘ └┬──────────┘
         │          │        │        │          │
    ┌────▼───┐ ┌───▼─────┐ ┌▼──────┐ ┌▼───────┐ ┌▼──────────┐
    │ Customer│ │ Account │ │Transfer│ │Notif.  │ │  Audit   │
    │   DB    │ │   DB    │ │  DB   │ │  DB    │ │   DB     │
    └─────────┘ └─────────┘ └───────┘ └────────┘ └──────────┘
         │          │        │        │          │
    PostgreSQL  PostgreSQL MySQL  MongoDB  PostgreSQL
    (ou outro)  (ou outro) (ou)  (ou outro)
```

---

## 🎯 Divisão em 5 Microsserviços

### 1️⃣ **Customer Service** (Gerenciamento de Clientes)
**Responsabilidades:**
- ✅ Cadastro de clientes (KYC)
- ✅ Validação de CPF
- ✅ Dados demográficos
- ✅ Perfil de risco
- ✅ Limites de operação

**Endpoints:**
```
POST   /api/customers              - Criar cliente
GET    /api/customers/{id}         - Obter cliente
PUT    /api/customers/{id}         - Atualizar cliente
GET    /api/customers/{id}/profile - Perfil completo
DELETE /api/customers/{id}         - Desativar cliente
```

**Dependências:**
- Nenhuma (independente)
- Pode ser chamado por Account Service

**Tecnologias:**
- .NET 10 / C#
- Entity Framework Core
- PostgreSQL
- RabbitMQ/Kafka (publicar eventos: CustomerCreated, CustomerUpdated)

**Database Schema:**
```sql
- Customers (id, cpf, name, email, phone, risk_profile, status)
- CustomerAddresses (customer_id, address_type, street, city)
- CustomerDocuments (customer_id, doc_type, doc_number)
```

---

### 2️⃣ **Account Service** (Gerenciamento de Contas)
**Responsabilidades:**
- ✅ Criar contas bancárias
- ✅ Consultar saldo
- ✅ Listar movimentações
- ✅ Depósitos
- ✅ Saques
- ✅ Limites de operação

**Endpoints:**
```
POST   /api/accounts                - Criar conta
GET    /api/accounts/{number}       - Obter conta
GET    /api/accounts/{number}/balance - Consultar saldo
GET    /api/accounts/{number}/statement - Extrato
POST   /api/accounts/{number}/deposit  - Depositar
POST   /api/accounts/{number}/withdraw - Sacar
```

**Dependências:**
- **Customer Service** (validar cliente antes de criar conta)
- **Notification Service** (notificar operações importantes)

**Eventos Publicados:**
- `AccountCreated`
- `DepositMade`
- `WithdrawalMade`
- `BalanceUpdated`

**Eventos Consumidos:**
- `CustomerCreated` (do Customer Service)
- `TransferCompleted` (do Transfer Service)

**Tecnologias:**
- .NET 10 / C#
- Entity Framework Core
- PostgreSQL
- RabbitMQ/Kafka

**Database Schema:**
```sql
- BankAccounts (id, account_number, branch, customer_id, balance, status)
- Transactions (id, account_id, type, amount, date, description)
- AccountLimits (account_id, daily_limit, monthly_limit)
```

---

### 3️⃣ **Transfer Service** (Gerenciamento de Transferências)
**Responsabilidades:**
- ✅ Transferências entre contas
- ✅ TED (Transferência Eletrônica Disponível)
- ✅ PIX
- ✅ DOC (Documento de Crédito)
- ✅ Validação de regras
- ✅ Concorrência (Saga Pattern)

**Endpoints:**
```
POST   /api/transfers               - Iniciar transferência
GET    /api/transfers/{id}         - Status da transferência
GET    /api/transfers/history      - Histórico de transferências
POST   /api/transfers/{id}/confirm - Confirmar (2FA)
POST   /api/transfers/{id}/cancel  - Cancelar
```

**Dependências:**
- **Account Service** (validar contas, debitar/creditar)
- **Notification Service** (notificar ambos os clientes)
- **Audit Service** (registrar operação crítica)

**Padrão: Saga Distribuída (Choreography)**
```
Transfer Request
    ↓
Validate Accounts (Account Service)
    ↓
Debit Source Account (Account Service)
    ↓
Credit Destination Account (Account Service)
    ↓
Send Notification (Notification Service)
    ↓
Log Audit (Audit Service)
    ↓
Return Confirmation
```

**Eventos Publicados:**
- `TransferInitiated`
- `TransferCompleted`
- `TransferFailed`
- `TransferCancelled`

**Eventos Consumidos:**
- `AccountCreated` (do Account Service)

**Tecnologias:**
- .NET 10 / C#
- Entity Framework Core
- MySQL (ou PostgreSQL)
- RabbitMQ/Kafka (para orquestração de saga)
- Redis (cache para validações rápidas)

**Database Schema:**
```sql
- Transfers (id, source_account, dest_account, amount, type, status, date)
- TransferHistory (transfer_id, status_old, status_new, timestamp, reason)
```

---

### 4️⃣ **Notification Service** (Sistema de Notificações)
**Responsabilidades:**
- ✅ Enviar emails
- ✅ Enviar SMS
- ✅ Enviar notificações push
- ✅ Logs de notificações
- ✅ Retentar falhas

**Endpoints:**
```
POST   /api/notifications/email     - Enviar email
POST   /api/notifications/sms       - Enviar SMS
POST   /api/notifications/push      - Enviar push
GET    /api/notifications/{id}      - Status de notificação
```

**Dependências:**
- Nenhuma dependência de negócio
- Consome eventos de todos os serviços

**Eventos Consumidos:**
- `CustomerCreated` (Customer Service)
- `AccountCreated` (Account Service)
- `TransferCompleted` (Transfer Service)
- `DepositMade` (Account Service)
- `WithdrawalMade` (Account Service)

**Tecnologias:**
- .NET 10 / C#
- MongoDB (armazenar histórico de notificações)
- SendGrid / AWS SES (envio de emails)
- Twilio (envio de SMS)
- Firebase (notificações push)
- RabbitMQ/Kafka (consumidor de eventos)

**Database Schema:**
```sql
- NotificationLog (id, customer_id, type, destination, content, status, timestamp)
- NotificationTemplate (id, type, subject, body)
- NotificationRetry (notification_id, attempt, next_retry_date)
```

---

### 5️⃣ **Audit Service** (Auditoria e Conformidade)
**Responsabilidades:**
- ✅ Registrar todas as operações críticas
- ✅ Rastreabilidade completa
- ✅ Conformidade regulatória
- ✅ Detecção de fraudes
- ✅ Relatórios de compliance

**Endpoints:**
```
GET    /api/audit/logs              - Listar logs
GET    /api/audit/logs/{customer}   - Auditoria por cliente
GET    /api/audit/reports           - Relatórios
POST   /api/audit/export            - Exportar dados
```

**Dependências:**
- Nenhuma (lê apenas)

**Eventos Consumidos:**
- Todos os eventos de todos os serviços

**Tecnologias:**
- .NET 10 / C#
- PostgreSQL + Archive Storage
- Elasticsearch (buscas rápidas em logs)
- Kibana (visualização)
- RabbitMQ/Kafka

**Database Schema:**
```sql
- AuditLog (id, event_type, user_id, customer_id, action, entity, 
            old_value, new_value, timestamp, ip_address)
- ComplianceReport (id, report_type, generated_date, data)
```

---

## 📡 Padrões de Comunicação

### 1. **Síncrona (REST/gRPC)**
```
Customer Service → [HTTP] → Account Service
    ↓
Request: POST /api/accounts/validate/{customer_id}
Response: {valid: true, limits: {...}}
```

**Quando usar:**
- Validações imediatas
- Consultas de dados
- Operações que precisam de resposta rápida

### 2. **Assíncrona (Event-Driven)**
```
Account Service → [RabbitMQ/Kafka] → Notification Service
                                  → Audit Service
                                  → Transfer Service
```

**Quando usar:**
- Notificações
- Operações que podem ser lentas
- Desacoplamento de serviços
- Escalabilidade horizontal

### 3. **Saga Pattern (Orquestração Distribuída)**
```
Transfer Service
    ↓ (inicia saga)
Account Service (debita)
    ↓
Account Service (credita)
    ↓
Notification Service (notifica)
    ↓
Audit Service (registra)
    ↓
Sucesso ou Rollback
```

---

## 🔐 API Gateway

Ponto de entrada único para todos os clientes:

```
Client
  ↓
┌─────────────────────────────────┐
│      API Gateway (Kong/Nginx)   │
├─────────────────────────────────┤
│  • Rate Limiting                │
│  • Authentication (JWT)         │
│  • Load Balancing               │
│  • Request/Response Logging     │
│  • Routing inteligente          │
└────────┬──────────┬────┬────┬──┘
         ↓          ↓    ↓    ↓
    Customer   Account  Transfer Notif.
    Service    Service  Service  Service
```

**Roteamento:**
```yaml
/api/customers/*       → Customer Service
/api/accounts/*        → Account Service
/api/transfers/*       → Transfer Service
/api/notifications/*   → Notification Service
/api/audit/*           → Audit Service (admin only)
```

---

## 📊 Escalabilidade e Deployment

### Infraestrutura Recomendada:

```
┌─────────────────────────────────────────────┐
│          Kubernetes Cluster                 │
├─────────────────────────────────────────────┤
│                                             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐ │
│  │ Customer │  │ Account  │  │ Transfer │ │
│  │ Service  │  │ Service  │  │ Service  │ │
│  │ (3 pods) │  │ (5 pods) │  │ (4 pods) │ │
│  └──────────┘  └──────────┘  └──────────┘ │
│                                             │
│  ┌──────────┐  ┌──────────┐               │
│  │  Notif.  │  │  Audit   │               │
│  │ Service  │  │ Service  │               │
│  │ (2 pods) │  │ (2 pods) │               │
│  └──────────┘  └──────────┘               │
│                                             │
│  ┌──────────────────────────────────────┐ │
│  │   Message Broker (RabbitMQ/Kafka)    │ │
│  └──────────────────────────────────────┘ │
│                                             │
│  ┌──────────────────────────────────────┐ │
│  │  Service Mesh (Istio)                │ │
│  │  - Circuit Breaker                   │ │
│  │  - Retry Logic                       │ │
│  │  - Distributed Tracing               │ │
│  └──────────────────────────────────────┘ │
└─────────────────────────────────────────────┘
         ↓           ↓            ↓
     PostgreSQL   MySQL/Mongo  PostgreSQL
```

---

## 🔄 Exemplo de Fluxo: Transferência Completa

```
1. Cliente acessa API Gateway
   POST /api/transfers
   {
     "sourceAccount": "1001",
     "destAccount": "2002",
     "amount": 500.00
   }

2. API Gateway valida token JWT e roteia para Transfer Service

3. Transfer Service inicia uma Saga:
   ┌─────────────────────────────────────┐
   │ Saga: TransferMoney                 │
   └─────────────────────────────────────┘
   
   Step 1: Account Service.ValidateAndDebit()
   - Valida saldo
   - Debita conta origem
   - Publica: SourceAccountDebited
   
   Step 2: Account Service.Credit()
   - Credita conta destino
   - Publica: DestAccountCredited
   
   Step 3: Notification Service (consome eventos)
   - Envia email para cliente origem
   - Envia SMS para cliente destino
   - Publica: NotificationsSent
   
   Step 4: Audit Service (consome eventos)
   - Registra toda a operação
   - Verifica compliance
   - Publica: AuditLogged
   
   Step 5: Transfer Service finaliza saga
   - Salva status: COMPLETED
   - Retorna confirmação ao cliente

4. Em caso de falha em qualquer etapa:
   - Rollback automático via Saga Compensating Transactions
   - Notificação de erro ao cliente
   - Log de auditoria da falha
```

---

## 📈 Benefícios da Migração

| Aspecto | Monolito | Microsserviços |
|---------|----------|----------------|
| **Escalabilidade** | Vertical (mais lento) | Horizontal (cada serviço independente) |
| **Deployment** | Tudo junto (mais arriscado) | Independente (mais seguro) |
| **Manutenção** | Código monolítico | Código menor, mais focado |
| **Times** | Um time grande | Múltiplos times pequenos |
| **Falhas** | Falha geral do sistema | Falha isolada em um serviço |
| **Tech Stack** | Unificado | Múltiplas tecnologias possíveis |
| **Performance** | ⚠️ Possíveis gargalos | ✅ Otimizado por serviço |
| **Complexidade** | ✅ Simples | ⚠️ Complexa (network, eventual consistency) |

---

## ⚠️ Desafios da Migração

1. **Consistência de Dados**
   - Eventual Consistency vs Strong Consistency
   - Solução: Event Sourcing + CQRS

2. **Network Latency**
   - Mais chamadas de rede
   - Solução: Caching, Circuit Breaker

3. **Monitoramento**
   - Rastreamento distribuído complexo
   - Solução: Jaeger, DataDog, Prometheus

4. **Testes**
   - Testes de integração mais complexos
   - Solução: Test Containers, Postman Collections

5. **Transações Distribuídas**
   - ACID não funciona naturalmente
   - Solução: Saga Pattern, Compensating Transactions

---

## 🚀 Roadmap de Migração

### Fase 1: Preparação (Semanas 1-2)
- [ ] Escolher padrão de comunicação (síncrono/assíncrono)
- [ ] Configurar Message Broker (RabbitMQ/Kafka)
- [ ] Escolher ferramentas de monitoramento
- [ ] Documentar bounded contexts

### Fase 2: Customer Service (Semanas 3-4)
- [ ] Extrair Customer Service do monolito
- [ ] Criar database independente
- [ ] Testes end-to-end
- [ ] Deploy em staging

### Fase 3: Account Service (Semanas 5-6)
- [ ] Extrair Account Service
- [ ] Implementar chamadas síncronas para Customer Service
- [ ] Testes e deploy

### Fase 4: Transfer Service (Semanas 7-8)
- [ ] Extrair Transfer Service
- [ ] Implementar Saga Pattern
- [ ] Testes complexos

### Fase 5: Services de Suporte (Semanas 9-10)
- [ ] Notification Service
- [ ] Audit Service
- [ ] Event Streaming setup

### Fase 6: API Gateway e Deploy (Semanas 11-12)
- [ ] Configurar Kong/Nginx
- [ ] Kubernetes manifests
- [ ] Load testing
- [ ] Documentação de produção

---

## 📚 Tecnologias Recomendadas

```
┌─────────────────────────────────────┐
│  Linguagem/Framework                │
├─────────────────────────────────────┤
│  .NET 10 / C# (todos os serviços)   │
│  ou Node.js/Go para alguns         │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  Banco de Dados                     │
├─────────────────────────────────────┤
│  PostgreSQL (Customer, Audit)       │
│  MySQL (Transfer)                   │
│  MongoDB (Notification History)     │
│  Redis (Cache e Session)            │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  Message Broker                     │
├─────────────────────────────────────┤
│  RabbitMQ (mais simples)            │
│  Kafka (mais poderoso)              │
│  AWS SQS/SNS (cloud-native)         │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  Orquestração                       │
├─────────────────────────────────────┤
│  Kubernetes (produção)              │
│  Docker Compose (desenvolvimento)   │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│  Observabilidade                    │
├─────────────────────────────────────┤
│  Prometheus (métricas)              │
│  Grafana (visualização)             │
│  Jaeger (distributed tracing)       │
│  ELK Stack (logs)                   │
│  DataDog (APM)                      │
└─────────────────────────────────────┘
```

---

## 📝 Conclusão

A migração para microsserviços é viável e traria **grande escalabilidade**, mas introduziria **complexidade operacional**. 

Recomendação: Começar com **Customer Service** como prova de conceito e validar antes de migrar outros serviços.

