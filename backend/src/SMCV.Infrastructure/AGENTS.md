# Infrastructure — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Implementar os contratos definidos em `Application/Interfaces/`.
Contem acesso a banco, servicos concretos e integracoes externas.

## ESTRUTURA

```
src/SMCV.Infrastructure/
├── InfrastructureServiceExtensions.cs   ← Startup module (AddInfrastructure)
├── Data/
│   ├── AppDbContext.cs
│   ├── AppDbContextFactory.cs
│   └── Migrations/
│       ├── 20260408190858_InitialCreate.cs
│       ├── 20260408224725_RemovePasswordHashAndSchemaSync.cs
│       ├── 20260409114950_ConvertStatusEnumsToString.cs
│       ├── 20260409134101_AddPasswordHashToUser.cs
│       └── AppDbContextModelSnapshot.cs
├── Repositories/
│   ├── BaseRepository.cs
│   ├── CampaignRepository.cs
│   ├── ContactRepository.cs
│   ├── EmailLogRepository.cs
│   ├── UserRepository.cs
│   └── UserProfileRepository.cs
└── ExternalServices/
    ├── CsvExportService.cs
    ├── EmailSenderService.cs
    └── EmailSettings.cs
```

## ARQUIVOS EXISTENTES

### Data (`SMCV.Infrastructure.Data`)

| Arquivo | Descricao |
|---------|-----------|
| `AppDbContext.cs` | DbContext do EF Core com DbSets para Campaign, Contact, EmailLog, User, UserProfile. Configura relacionamentos e constraints de todas as 5 entidades via Fluent API. |
| `AppDbContextFactory.cs` | Factory design-time para EF Core migrations. Le connection string de appsettings. |

### Repositories (`SMCV.Infrastructure.Repositories`)

| Arquivo | Interface | Descricao |
|---------|-----------|-----------|
| `BaseRepository.cs` | `IRepository<T>` | Repositorio generico base com CRUD e acesso ao DbContext. |
| `CampaignRepository.cs` | `ICampaignRepository` | Extensao do BaseRepository com eager loading de contacts e email logs. |
| `ContactRepository.cs` | `IContactRepository` | Extensao do BaseRepository com busca por campanha, email e include de email logs. |
| `EmailLogRepository.cs` | `IEmailLogRepository` | Extensao do BaseRepository com busca por contact ID e campaign ID. |
| `UserRepository.cs` | `IUserRepository` | Extensao do BaseRepository com GetByEmailAsync e GetAllPagedAsync(pageNumber, pageSize) para listagem paginada. |
| `UserProfileRepository.cs` | `IUserProfileRepository` | Extensao do BaseRepository com GetByUserIdAsync e GetAllPagedAsync(pageNumber, pageSize) para listagem paginada. |

### ExternalServices (`SMCV.Infrastructure.ExternalServices`)

| Arquivo | Interface | Descricao |
|---------|-----------|-----------|
| `EmailSenderService.cs` | `IEmailSenderService` | Envio de email SMTP via MailKit com suporte a anexos. From usa SMTP SenderEmail, Reply-To usa replyToEmail/replyToName do usuario. Configurado via `EmailSettings`. |
| `CsvExportService.cs` | `ICsvExportService` | Geracao de CSV com colunas CompanyName, Email, EmailStatus, CampaignId. Escaping e UTF-8 BOM. |
| `EmailSettings.cs` | — | Classe de configuracao SMTP: host, port, sender email/password/name. Bind via `IOptions<EmailSettings>`. |

## REGRAS OBRIGATORIAS — Repositories

- Namespace: `SMCV.Infrastructure.Repositories`
- Herdar de `BaseRepository<T>` e implementar interface de `Application/Interfaces/`
- Injetar `AppDbContext` via construtor (passando para base)
- Usar metodos async do EF Core (`ToListAsync`, `FindAsync`, etc.)
- `SaveChangesAsync()` apos cada operacao de escrita

## PADRAO DE CONFIGURACAO — Servicos Externos

Credenciais e configuracoes sensiveis NUNCA sao lidas via IConfiguration ou appsettings.json.

Padroes corretos:
- API keys e strings simples → `Environment.GetEnvironmentVariable("NOME_VAR")`
- Grupos de configuracao com tipagem → `IOptions<T>` alimentado via `Program.cs`
  usando `RequiredSetting()`, nunca via `.GetSection()` apontando para appsettings.json

Variaveis disponiveis estao documentadas em `.env.example` na raiz do projeto.
O `docker-compose.yml` e responsavel por injetar essas variaveis no container do backend.

## REGRAS OBRIGATORIAS — ExternalServices

- Namespace: `SMCV.Infrastructure.ExternalServices`
- Implementar interface de `Application/Interfaces/`
- Configuracoes via `IOptions<T>` ou `Environment.GetEnvironmentVariable` (nunca hardcoded, nunca via appsettings.json)
- Registrar no DI em `InfrastructureServiceExtensions.cs` com `AddScoped`

## REGRAS OBRIGATORIAS — DbContext

- Namespace: `SMCV.Infrastructure.Data`
- Um `DbSet<T>` por entidade do dominio
- Configuracoes Fluent API no `OnModelCreating`

## REGRAS OBRIGATORIAS — Migrations

- Namespace: `SMCV.Infrastructure.Data.Migrations`
- Gerar via `dotnet ef migrations add NomeDaMigration --project src/SMCV.Infrastructure --startup-project src/SMCV.Api`
- NAO editar migrations geradas manualmente
- Auto-migrate habilitado no `Program.cs`

## PROIBICOES

- **SEM** logica de negocio nos Repositories — apenas CRUD e queries
- **SEM** retorno de DTOs nos Repositories — trabalhar com Entities
