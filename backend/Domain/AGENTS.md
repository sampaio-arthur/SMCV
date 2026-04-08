# Domain — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Conter as entidades do dominio (modelos EF Core) e enums.
Esta camada NAO depende de nenhuma outra — e a base da aplicacao.

## ESTRUTURA

```
Domain/
├── Entities/
│   ├── Campaign.cs
│   ├── Contact.cs
│   └── EmailLog.cs
└── Enums/
    ├── CampaignStatus.cs
    └── EmailStatus.cs
```

## ARQUIVOS EXISTENTES

### Entities (`SMCV.Domain.Entities`)

| Arquivo | Descricao |
|---------|-----------|
| `Campaign.cs` | Agregado raiz de campanha: niche, region, resume, email content, status, colecao de contacts. |
| `Contact.cs` | Contato prospectado: company, email, domain, position, referencia a Campaign, EmailLog opcional. |
| `EmailLog.cs` | Log de envio de email: status, timestamp de envio, mensagem de erro, referencia a Contact. |

### Enums (`SMCV.Domain.Enums`)

| Arquivo | Valores |
|---------|---------|
| `CampaignStatus.cs` | `Draft`, `Running`, `Completed`, `Cancelled` |
| `EmailStatus.cs` | `Pending`, `Sent`, `Failed` |

## REGRAS OBRIGATORIAS

- Namespace: `SMCV.Domain.Entities` ou `SMCV.Domain.Enums`
- Toda entidade tem `Id` (int, PK auto-increment)
- Campos obrigatorios usam `required`
- Campos opcionais usam `?` (nullable)
- Timestamps: `CreatedAt` (DateTime) e `UpdatedAt` (DateTime?)
- Default de `CreatedAt` = `DateTime.UtcNow`
- Default de `IsActive` = `true` (quando aplicavel)

## PROIBICOES

- **SEM** logica de negocio nas entidades
- **SEM** referencia a DTOs, Services ou Controllers
- **SEM** atributos de validacao (`[Required]`, `[MaxLength]`) — usar Fluent API no DbContext
