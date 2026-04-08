# Infrastructure — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Implementar os contratos definidos em `Application/Interfaces/`.
Contem acesso a banco, servicos concretos e integracoes externas.

## ESTRUTURA

```
Infrastructure/
├── Data/
│   ├── AppDbContext.cs
│   ├── AppDbContextFactory.cs
│   └── Migrations/
│       ├── 20260407185014_InitialCreate.cs
│       ├── 20260407185014_InitialCreate.Designer.cs
│       └── AppDbContextModelSnapshot.cs
├── Repositories/
│   ├── BaseRepository.cs
│   ├── CampaignRepository.cs
│   ├── ContactRepository.cs
│   └── EmailLogRepository.cs
└── ExternalServices/
    ├── CsvExportService.cs
    ├── EmailSenderService.cs
    ├── EmailSettings.cs
    └── HunterService.cs
```

## ARQUIVOS EXISTENTES

### Data (`SMCV.Infrastructure.Data`)

| Arquivo | Descricao |
|---------|-----------|
| `AppDbContext.cs` | DbContext do EF Core com DbSets para Campaign, Contact, EmailLog. Configura relacionamentos e constraints via Fluent API. |
| `AppDbContextFactory.cs` | Factory design-time para EF Core migrations. Le connection string de appsettings. |

### Data/Migrations (`SMCV.Infrastructure.Data.Migrations`)

| Arquivo | Descricao |
|---------|-----------|
| `20260407185014_InitialCreate.cs` | Migration inicial: cria tabelas Campaign, Contact, EmailLog com relacionamentos e constraints. |
| `20260407185014_InitialCreate.Designer.cs` | Snapshot designer da migration inicial (auto-gerado). |
| `AppDbContextModelSnapshot.cs` | Snapshot do modelo atual do banco (auto-gerado). |

### Repositories (`SMCV.Infrastructure.Repositories`)

| Arquivo | Interface | Descricao |
|---------|-----------|-----------|
| `BaseRepository.cs` | `IRepository<T>` | Repositorio generico base com CRUD e acesso ao DbContext. |
| `CampaignRepository.cs` | `ICampaignRepository` | Extensao do BaseRepository com eager loading de contacts e email logs. |
| `ContactRepository.cs` | `IContactRepository` | Extensao do BaseRepository com busca por campanha, email e include de email logs. |
| `EmailLogRepository.cs` | `IEmailLogRepository` | Extensao do BaseRepository com busca por contact ID e campaign ID. |

### ExternalServices (`SMCV.Infrastructure.ExternalServices`)

| Arquivo | Interface | Descricao |
|---------|-----------|-----------|
| `HunterService.cs` | `IHunterService` | Integracao com API Hunter.io para busca de contatos por dominio. Usa HttpClient. |
| `EmailSenderService.cs` | `IEmailSenderService` | Envio de email SMTP via MailKit com suporte a anexos. Configurado via `EmailSettings`. |
| `CsvExportService.cs` | `ICsvExportService` | Geracao de CSV a partir de contatos com escaping e UTF-8 BOM. |
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
- Registrar no DI em `Program.cs` com `AddScoped`

## REGRAS OBRIGATORIAS — DbContext

- Namespace: `SMCV.Infrastructure.Data`
- Um `DbSet<T>` por entidade do dominio
- Configuracoes Fluent API no `OnModelCreating`

## REGRAS OBRIGATORIAS — Migrations

- Namespace: `SMCV.Infrastructure.Data.Migrations`
- Gerar via `dotnet ef migrations add NomeDaMigration`
- NAO editar migrations geradas manualmente
- Auto-migrate habilitado no `Program.cs`

## PROIBICOES

- **SEM** logica de negocio nos Repositories — apenas CRUD e queries
- **SEM** retorno de DTOs nos Repositories — trabalhar com Entities
