# Banking Core Microservice - Arquitetura Hexagonal

## 📋 Visão Geral

Microserviço de Core Bancário desenvolvido em .NET 8 seguindo os princípios de **Arquitetura Hexagonal (Ports & Adapters)**.

### Contexto de Negócio

Sistema responsável por operações críticas de um banco digital:
- ✅ Cadastro de clientes (KYC)
- ✅ Contas correntes
- ✅ Saldos e extratos
- ✅ Transferências (TED/PIX)
- ✅ Pagamentos de boletos
- ✅ Notificações transacionais

## 🏗️ Estrutura da Arquitetura Hexagonal

```
┌──────────────────────────────────────┐
│     ADAPTADORES (Externos)           │
│ API │ DB │ Mensageria │ Notificações │
└──────────────────────────────────────┘
           ▲
           │ [PORTAS]
           ▼
┌──────────────────────────────────────┐
│   CAMADA DE APLICAÇÃO                │
│  Casos de Uso / Serviços             │
└──────────────────────────────────────┘
           ▲
           │ [PORTAS]
           ▼
┌──────────────────────────────────────┐
│   CAMADA DE DOMÍNIO                  │
│ Entidades │ Agregados │ Value Objects│
└──────────────────────────────────────┘
```

## 🔧 Tecnologias

- .NET 8
- Entity Framework Core
- MediatR
- Fluent Validation
- AutoMapper
- Serilog

---

**Versão**: 1.0.0 | **Data**: 2026-03-23
