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
│   ├── EmailLog.cs
│   ├── User.cs
│   └── UserProfile.cs
└── Enums/
    ├── CampaignStatus.cs
    └── EmailStatus.cs
```

## ARQUIVOS EXISTENTES

### Entities (`SMCV.Domain.Entities`)

| Arquivo | Descricao |
|---------|-----------|
| `Campaign.cs` | Agregado raiz de campanha: UserId (FK), Name, Niche, Region, EmailSubject, EmailBody, Status, User nav, colecao de Contacts. |
| `Contact.cs` | Contato prospectado: CompanyName, Email, EmailStatus (enum), EmailSentAt (DateTime?), referencia a Campaign. |
| `EmailLog.cs` | Log de envio de email: Id, ContactId, Contact nav, ErrorMessage, CreatedAt. |
| `User.cs` | Usuario da plataforma: Id, Name, Email, CreatedAt, UserProfile nav, Campaigns collection. Autenticacao via Keycloak (sem PasswordHash). |
| `UserProfile.cs` | Perfil do usuario: Id, UserId (FK), User nav, ResumeFilePath, CreatedAt. |

### Enums (`SMCV.Domain.Enums`)

| Arquivo | Valores |
|---------|---------|
| `CampaignStatus.cs` | `Draft`, `Running`, `Completed`, `Cancelled`, `Failed`, `PartialSuccess` |
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
