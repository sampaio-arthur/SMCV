# Application — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Definir contratos (interfaces) e objetos de transferencia (DTOs).
Camada de abstracao entre Domain e Infrastructure.

## ESTRUTURA

```
Application/
├── DTOs/
│   ├── Campaigns/
│   │   ├── CampaignDetailResponse.cs
│   │   ├── CampaignResponse.cs
│   │   ├── CreateCampaignRequest.cs
│   │   ├── ExportContactsCsvRequest.cs
│   │   ├── SendCampaignEmailsRequest.cs
│   │   └── UpdateCampaignRequest.cs
│   ├── Contacts/
│   │   ├── ContactResponse.cs
│   │   ├── CreateContactRequest.cs
│   │   ├── SearchContactsRequest.cs
│   │   └── SearchContactsResponse.cs
│   ├── EmailLogs/
│   │   └── EmailLogResponse.cs
│   └── ErrorResponse.cs
├── Interfaces/
│   ├── HunterContactResult.cs
│   ├── ICampaignRepository.cs
│   ├── IContactRepository.cs
│   ├── ICsvExportService.cs
│   ├── IEmailLogRepository.cs
│   ├── IEmailSenderService.cs
│   ├── IHunterService.cs
│   └── IRepository.cs
└── Mappings/
    └── MappingProfile.cs
```

## ARQUIVOS EXISTENTES

### DTOs — Campaigns (`SMCV.Application.DTOs.Campaigns`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `CampaignDetailResponse.cs` | Response | Detalhes completos da campanha com contatos e logs de email aninhados. |
| `CampaignResponse.cs` | Response | Resumo da campanha com contagem de contatos (sem lista detalhada). |
| `CreateCampaignRequest.cs` | Request | Criacao de campanha: niche, region, email subject/body. Resume tratado separadamente. |
| `ExportContactsCsvRequest.cs` | Request | Exportacao CSV de contatos por campaign ID. |
| `SendCampaignEmailsRequest.cs` | Request | Disparo de emails da campanha por campaign ID. |
| `UpdateCampaignRequest.cs` | Request | Atualizacao de email subject e body da campanha. |

### DTOs — Contacts (`SMCV.Application.DTOs.Contacts`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `ContactResponse.cs` | Response | Contato com company/email, referencia a campanha e log de email opcional. |
| `CreateContactRequest.cs` | Request | Criacao de contato: company, email, domain, position, campaign ID. |
| `SearchContactsRequest.cs` | Request | Busca de contatos via Hunter.io: niche, region, limit. |
| `SearchContactsResponse.cs` | Response | Resultado da busca: total encontrado e lista de contatos adicionados. |

### DTOs — EmailLogs (`SMCV.Application.DTOs.EmailLogs`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `EmailLogResponse.cs` | Response | Status de envio, timestamp e detalhes de erro. |

### DTOs — Root (`SMCV.Application.DTOs`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `ErrorResponse.cs` | Response | Resposta generica de erro com mensagem e detalhes opcionais. |

### Interfaces (`SMCV.Application.Interfaces`)

| Arquivo | Descricao | Implementacao |
|---------|-----------|---------------|
| `IRepository.cs` | Interface base generica com CRUD e ExistsAsync. | `BaseRepository.cs` |
| `ICampaignRepository.cs` | Extensao de IRepository para Campaign com eager loading de contacts. | `CampaignRepository.cs` |
| `IContactRepository.cs` | Extensao de IRepository para Contact com busca por campanha e email. | `ContactRepository.cs` |
| `IEmailLogRepository.cs` | Extensao de IRepository para EmailLog com busca por contact/campaign. | `EmailLogRepository.cs` |
| `IHunterService.cs` | Busca de contatos por niche/region via Hunter.io. | `HunterService.cs` |
| `IEmailSenderService.cs` | Envio de email com anexos via SMTP. | `EmailSenderService.cs` |
| `ICsvExportService.cs` | Geracao de CSV a partir de lista de contatos. | `CsvExportService.cs` |
| `HunterContactResult.cs` | Record representando resultado individual da API Hunter.io (nao e interface). | — |

### Mappings (`SMCV.Application.Mappings`)

| Arquivo | Descricao |
|---------|-----------|
| `MappingProfile.cs` | Perfil AutoMapper: Campaign, Contact, EmailLog -> DTOs com conversao de enum para string. |

## REGRAS OBRIGATORIAS — DTOs

- Namespace: `SMCV.Application.DTOs.{Dominio}` (subpasta por dominio)
- Sufixo `Request` para entrada, `Response` para saida
- Usar `record` para DTOs (imutabilidade)
- RequestDto NAO inclui `Id`, `CreatedAt`, `UpdatedAt`
- ResponseDto inclui todos os campos publicos relevantes da entidade

## REGRAS OBRIGATORIAS — Interfaces

- Namespace: `SMCV.Application.Interfaces`
- Prefixo `I` no nome (ex: `ICampaignRepository`)
- Metodos async com sufixo `Async`
- Repository trabalha com Entity; Service trabalha com DTOs ou tipos proprios

## PROIBICOES

- **SEM** implementacao concreta nesta camada — apenas contratos e DTOs
- **SEM** referencia a `DbContext` ou pacotes de infraestrutura
