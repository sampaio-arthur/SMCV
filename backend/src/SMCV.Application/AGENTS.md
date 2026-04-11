# Application — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Definir contratos (interfaces) e objetos de transferencia (DTOs).
Camada de abstracao entre Domain e Infrastructure.

## ESTRUTURA

```
src/SMCV.Application/
├── ApplicationServiceExtensions.cs   ← Startup module (AddApplication)
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequest.cs
│   │   └── RegisterRequest.cs
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
│   │   └── UpdateContactRequest.cs
│   ├── EmailLogs/
│   │   └── EmailLogResponse.cs
│   ├── Users/
│   │   ├── UserResponse.cs
│   │   ├── CreateUserRequest.cs
│   │   └── UpdateUserRequest.cs
│   ├── UserProfiles/
│   │   ├── UserProfileResponse.cs
│   │   ├── CreateUserProfileRequest.cs
│   │   └── UpdateUserProfileRequest.cs
│   └── ErrorResponse.cs
├── Interfaces/
│   ├── ICampaignRepository.cs
│   ├── IContactRepository.cs
│   ├── ICsvExportService.cs
│   ├── IEmailLogRepository.cs
│   ├── IEmailSenderService.cs
│   ├── IRepository.cs
│   ├── IUserRepository.cs
│   └── IUserProfileRepository.cs
└── Mappings/
    └── MappingProfile.cs
```

## ARQUIVOS EXISTENTES

### DTOs — Auth (`SMCV.Application.DTOs.Auth`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `RegisterRequest.cs` | Request | Registro de usuario: Name, Email, Password. |
| `LoginRequest.cs` | Request | Login: Email, Password. |

### DTOs — Campaigns (`SMCV.Application.DTOs.Campaigns`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `CampaignDetailResponse.cs` | Response | Detalhes completos da campanha com UserId, Name e contatos aninhados. |
| `CampaignResponse.cs` | Response | Resumo da campanha com UserId, Name e contagem de contatos (sem lista detalhada). |
| `CreateCampaignRequest.cs` | Request | Criacao de campanha: Name, Niche, Region, EmailSubject, EmailBody (UserId extraido da sessao no controller). |
| `ExportContactsCsvRequest.cs` | Request | Exportacao CSV de contatos por campaign ID. |
| `SendCampaignEmailsRequest.cs` | Request | Disparo de emails da campanha por campaign ID. |
| `UpdateCampaignRequest.cs` | Request | Atualizacao de Name, EmailSubject e EmailBody da campanha. |

### DTOs — Contacts (`SMCV.Application.DTOs.Contacts`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `ContactResponse.cs` | Response | Contato com CompanyName, Email, EmailStatus, EmailSentAt e referencia a campanha. |
| `CreateContactRequest.cs` | Request | Criacao de contato: CampaignId, CompanyName, Email. |
| `UpdateContactRequest.cs` | Request | Atualizacao de contato: CompanyName, Email. |

### DTOs — EmailLogs (`SMCV.Application.DTOs.EmailLogs`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `EmailLogResponse.cs` | Response | Log de email: Id, ContactId, ErrorMessage, CreatedAt. |

### DTOs — Users (`SMCV.Application.DTOs.Users`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `UserResponse.cs` | Response | Dados do usuario: Id, Name, Email, CreatedAt. |
| `CreateUserRequest.cs` | Request | Criacao de usuario: Name, Email. |
| `UpdateUserRequest.cs` | Request | Atualizacao de usuario: Name, Email. |

### DTOs — UserProfiles (`SMCV.Application.DTOs.UserProfiles`)

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `UserProfileResponse.cs` | Response | Perfil do usuario: Id, UserId, ResumeFilePath, CreatedAt. |
| `CreateUserProfileRequest.cs` | Request | Criacao de perfil (vazio — UserId extraido da sessao no controller). |
| `UpdateUserProfileRequest.cs` | Request | Atualizacao de perfil (vazio — ResumeFilePath so via endpoint de upload dedicado). |

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
| `IUserRepository.cs` | Extensao de IRepository para User com GetByEmailAsync e GetAllPagedAsync(pageNumber, pageSize). | `UserRepository.cs` |
| `IUserProfileRepository.cs` | Extensao de IRepository para UserProfile com GetByUserIdAsync e GetAllPagedAsync(pageNumber, pageSize). | `UserProfileRepository.cs` |
| `IEmailSenderService.cs` | Envio de email com anexos via SMTP. Aceita replyToEmail e replyToName (From usa SMTP SenderEmail, Reply-To usa email do usuario). | `EmailSenderService.cs` |
| `ICsvExportService.cs` | Geracao de CSV a partir de lista de contatos. | `CsvExportService.cs` |

### Mappings (`SMCV.Application.Mappings`)

| Arquivo | Descricao |
|---------|-----------|
| `MappingProfile.cs` | Perfil AutoMapper: Campaign, Contact, EmailLog, User, UserProfile -> DTOs com conversao de enum para string. |

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
