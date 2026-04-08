# Features — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Nucleo CQRS da aplicacao. Cada dominio tem sua pasta com Commands (escrita) e Queries (leitura).
Handlers contem toda a logica de negocio e sao invocados via `MediatR.Send()`.
Validators (FluentValidation) ficam junto aos Commands que validam.

## ESTRUTURA

```
Features/
├── Campaigns/
│   ├── Commands/
│   │   ├── CreateCampaign/
│   │   │   ├── CreateCampaignCommand.cs
│   │   │   ├── CreateCampaignCommandHandler.cs
│   │   │   └── CreateCampaignCommandValidator.cs
│   │   ├── DeleteCampaign/
│   │   │   ├── DeleteCampaignCommand.cs
│   │   │   └── DeleteCampaignCommandHandler.cs
│   │   ├── ExportContactsCsv/
│   │   │   ├── ExportContactsCsvCommand.cs
│   │   │   └── ExportContactsCsvCommandHandler.cs
│   │   ├── SendCampaignEmails/
│   │   │   ├── SendCampaignEmailsCommand.cs
│   │   │   └── SendCampaignEmailsCommandHandler.cs
│   │   └── UpdateCampaign/
│   │       ├── UpdateCampaignCommand.cs
│   │       ├── UpdateCampaignCommandHandler.cs
│   │       └── UpdateCampaignCommandValidator.cs
│   └── Queries/
│       ├── GetAllCampaigns/
│       │   ├── GetAllCampaignsQuery.cs
│       │   └── GetAllCampaignsQueryHandler.cs
│       └── GetCampaignById/
│           ├── GetCampaignByIdQuery.cs
│           └── GetCampaignByIdQueryHandler.cs
├── Contacts/
│   ├── Commands/
│   │   ├── CreateContact/
│   │   │   ├── CreateContactCommand.cs
│   │   │   ├── CreateContactCommandHandler.cs
│   │   │   └── CreateContactCommandValidator.cs
│   │   ├── DeleteContact/
│   │   │   ├── DeleteContactCommand.cs
│   │   │   └── DeleteContactCommandHandler.cs
│   │   └── SearchContacts/
│   │       ├── SearchContactsCommand.cs
│   │       └── SearchContactsCommandHandler.cs
│   └── Queries/
│       ├── GetContactById/
│       │   ├── GetContactByIdQuery.cs
│       │   └── GetContactByIdQueryHandler.cs
│       └── GetContactsByCampaign/
│           ├── GetContactsByCampaignQuery.cs
│           └── GetContactsByCampaignQueryHandler.cs
└── EmailLogs/
    └── Queries/
        ├── GetEmailLogByContact/
        │   ├── GetEmailLogByContactQuery.cs
        │   └── GetEmailLogByContactQueryHandler.cs
        └── GetEmailLogsByCampaign/
            ├── GetEmailLogsByCampaignQuery.cs
            └── GetEmailLogsByCampaignQueryHandler.cs
```

## ESTADO ATUAL

Todos os handlers estao implementados e funcionais:

### Campaigns

| Handler | Tipo | Namespace | Descricao |
|---------|------|-----------|-----------|
| `CreateCampaignCommandHandler` | Command | `SMCV.Features.Campaigns.Commands.CreateCampaign` | Cria campanha em status Draft. Validator incluso. |
| `DeleteCampaignCommandHandler` | Command | `SMCV.Features.Campaigns.Commands.DeleteCampaign` | Deleta campanha. Valida existencia e impede exclusao se status Running. |
| `ExportContactsCsvCommandHandler` | Command | `SMCV.Features.Campaigns.Commands.ExportContactsCsv` | Busca contatos da campanha e gera CSV via ICsvExportService. |
| `SendCampaignEmailsCommandHandler` | Command | `SMCV.Features.Campaigns.Commands.SendCampaignEmails` | Envia emails para contatos com anexo de resume. Cria/atualiza EmailLogs. |
| `UpdateCampaignCommandHandler` | Command | `SMCV.Features.Campaigns.Commands.UpdateCampaign` | Atualiza subject/body. Valida status Draft antes de permitir alteracao. Validator incluso. |
| `GetAllCampaignsQueryHandler` | Query | `SMCV.Features.Campaigns.Queries.GetAllCampaigns` | Lista todas as campanhas com eager loading de contacts. |
| `GetCampaignByIdQueryHandler` | Query | `SMCV.Features.Campaigns.Queries.GetCampaignById` | Busca campanha por ID com contacts. Lanca NotFoundException se nao encontrada. |

### Contacts

| Handler | Tipo | Namespace | Descricao |
|---------|------|-----------|-----------|
| `CreateContactCommandHandler` | Command | `SMCV.Features.Contacts.Commands.CreateContact` | Cria contato manual. Valida existencia da campanha e unicidade de email. Validator incluso. |
| `DeleteContactCommandHandler` | Command | `SMCV.Features.Contacts.Commands.DeleteContact` | Deleta contato por ID. Valida existencia. |
| `SearchContactsCommandHandler` | Command | `SMCV.Features.Contacts.Commands.SearchContacts` | Busca contatos via Hunter.io, filtra duplicatas, cria novos contatos no banco. |
| `GetContactByIdQueryHandler` | Query | `SMCV.Features.Contacts.Queries.GetContactById` | Busca contato por ID com email log. Lanca NotFoundException se nao encontrado. |
| `GetContactsByCampaignQueryHandler` | Query | `SMCV.Features.Contacts.Queries.GetContactsByCampaign` | Lista contatos de uma campanha com email logs. Valida existencia da campanha. |

### EmailLogs

| Handler | Tipo | Namespace | Descricao |
|---------|------|-----------|-----------|
| `GetEmailLogByContactQueryHandler` | Query | `SMCV.Features.EmailLogs.Queries.GetEmailLogByContact` | Busca log de email por contact ID. Lanca NotFoundException se nao encontrado. |
| `GetEmailLogsByCampaignQueryHandler` | Query | `SMCV.Features.EmailLogs.Queries.GetEmailLogsByCampaign` | Lista todos os logs de email de uma campanha com detalhes do contato. |

> **Nota:** EmailLogs nao possui Commands — os logs sao criados automaticamente pelo `SendCampaignEmailsCommandHandler`.

## REGRAS OBRIGATORIAS

- Namespace: `SMCV.Features.{Dominio}.Commands.{NomeDoCommand}` ou `SMCV.Features.{Dominio}.Queries.{NomeDaQuery}`
- Cada feature em subpasta propria: `CreateCampaign/`, `GetCampaignById/`, etc.
- Command e Handler em arquivos separados (Command.cs + CommandHandler.cs)
- Validator (quando existir) no mesmo diretorio do Command: `XxxCommandValidator.cs`
- Handler injeta Repository/Services via construtor
- Retorno de Commands: DTO ou tipo primitivo via `IRequest<T>`
- Retorno de Queries: DTO ou lista de DTOs via `IRequest<T>`

## PROIBICOES

- **SEM** acesso direto ao `DbContext` — usar Repository
- **SEM** referencia a `HttpContext` ou conceitos HTTP
- **SEM** dependencia lateral entre Features de dominios diferentes (exceto quando um handler precisa validar existencia de entidade de outro dominio via Repository)
