# Backend — AGENTS.md

> **PONTO DE ENTRADA — Leia este arquivo primeiro.**
> Navegue para o MD especifico conforme sua tarefa.

## VISAO GERAL

Arquitetura CQRS: **Controller -> MediatR Handler -> Repository -> PostgreSQL**.
Framework: ASP.NET Core 8 com EF Core. Namespace raiz: `SMCV`.
Controllers delegam para `MediatR.Send()`. Nenhuma logica de negocio vive no Controller.
Handlers CQRS sao a unica camada com logica de negocio.
Entidades nunca sao expostas na API. Injecao de dependencia via interfaces com `AddScoped`.

## NAVIGATION MAP

| Tarefa | Arquivo a ler |
|--------|--------------|
| Criar/alterar endpoint HTTP | Controllers/AGENTS.md |
| Criar/alterar entidade de banco | Domain/AGENTS.md |
| Criar/alterar DTO ou contrato | Application/AGENTS.md |
| Criar/alterar handler CQRS | Features/AGENTS.md |
| Alterar DbContext, migrations ou repositorio | Infrastructure/AGENTS.md |
| Criar/alterar Result ou excecao | Common/AGENTS.md |
| Alterar pipeline ou DI | PROGRAM_AGENTS.md |

## REGRAS GLOBAIS

1. **Sempre usar DTOs** — nunca expor `Entity` em controllers ou retornos de handler
2. **Interfaces obrigatorias** — todo Repository e Service externo tem sua interface em `Application/Interfaces/`
3. **Async em tudo** — todas as operacoes de I/O usam `async/await` com sufixo `Async`
4. **Namespace** — seguir `SMCV.{Camada}.{Subcamada}` (ex: `SMCV.Domain.Entities`)
5. **Injecao via construtor** — nunca usar `[FromServices]` ou service locator
6. **Uma classe por arquivo** — nome do arquivo = nome da classe
7. **PascalCase** para classes, metodos e propriedades
8. **Timestamps UTC** — sempre `DateTime.UtcNow` para `CreatedAt`/`UpdatedAt`
9. **Registrar no DI** — todo novo Repository/Service deve ser registrado em `Program.cs`
10. **Validacao via FluentValidation** — validators em `Features/{Dominio}/Commands/`

## ESTRUTURA DE PASTAS

```
backend/
├── Controllers/          ← roteamento HTTP, delega para MediatR
├── Features/             ← nucleo CQRS por dominio (Commands + Queries)
│   ├── Campaigns/
│   ├── Contacts/
│   ├── EmailLogs/
│   ├── Users/
│   └── UserProfiles/
├── Domain/
│   ├── Entities/         ← entidades EF Core (Campaign, Contact, EmailLog, User, UserProfile)
│   └── Enums/            ← enums do dominio (CampaignStatus, EmailStatus)
├── Infrastructure/
│   ├── Data/             ← DbContext, DbContextFactory, Migrations
│   ├── Repositories/     ← implementacao de acesso a dados (Base, Campaign, Contact, EmailLog, User, UserProfile)
│   └── ExternalServices/ ← integracao com APIs externas (Hunter.io, SMTP, CSV)
├── Application/
│   ├── DTOs/             ← objetos de transferencia (Campaigns/, Contacts/, EmailLogs/, Users/, UserProfiles/)
│   ├── Interfaces/       ← contratos de repositorios e servicos
│   └── Mappings/         ← AutoMapper profiles
├── Common/
│   ├── Exceptions/       ← excecoes customizadas (Business, NotFound)
│   ├── Middleware/        ← ExceptionHandlingMiddleware
│   └── ResultPattern/    ← ApiResponse<T> para respostas padronizadas
└── Program.cs
```

## FLUXO DE DADOS (CQRS)

```
HTTP Request
  -> Controller (recebe RequestDto, chama MediatR.Send())
    -> Handler (logica de negocio, usa Repository/Services)
      -> Repository (CRUD no banco via DbContext)
        -> Entity (modelo EF Core)
```
