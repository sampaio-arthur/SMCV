# Backend — AGENTS.md

> **PONTO DE ENTRADA — Leia este arquivo primeiro.**
> Navegue para o MD especifico conforme sua tarefa.

## VISAO GERAL

Arquitetura CQRS: **Controller -> MediatR Handler -> Repository -> PostgreSQL**.
Framework: ASP.NET Core 8 com EF Core. Namespace raiz: `SMCV`.
Controllers delegam para `MediatR.Send()`. Nenhuma logica de negocio vive no Controller.
Handlers CQRS sao a unica camada com logica de negocio.
Entidades nunca sao expostas na API. Injecao de dependencia via interfaces com `AddScoped`.
Autenticacao simples via sessao (email + senha com BCrypt). Sem JWT, sem OAuth.

## NAVIGATION MAP

| Tarefa | Arquivo a ler |
|--------|--------------|
| Criar/alterar endpoint HTTP | src/SMCV.Api/Controllers/AGENTS.md |
| Criar/alterar entidade de banco | src/SMCV.Domain/AGENTS.md |
| Criar/alterar DTO ou contrato | src/SMCV.Application/AGENTS.md |
| Criar/alterar handler CQRS | src/SMCV.Features/AGENTS.md |
| Alterar DbContext, migrations ou repositorio | src/SMCV.Infrastructure/AGENTS.md |
| Criar/alterar Result ou excecao | src/SMCV.Common/AGENTS.md |
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

## ESTRUTURA DE PROJETOS (Multi-projeto)

```
backend/
├── SMCV.slnx                          ← solution file
├── src/
│   ├── SMCV.Domain/                   ← entidades e enums (sem dependencias)
│   │   ├── Entities/
│   │   └── Enums/
│   ├── SMCV.Common/                   ← excecoes e result pattern (sem dependencias)
│   │   ├── Exceptions/
│   │   └── ResultPattern/
│   ├── SMCV.Application/             ← DTOs, interfaces, mappings (depende de Domain)
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Mappings/
│   ├── SMCV.Infrastructure/          ← DbContext, repos, services (depende de Domain, Application)
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── ExternalServices/
│   ├── SMCV.Features/                ← nucleo CQRS (depende de Domain, Application, Common)
│   │   ├── Auth/
│   │   ├── Campaigns/
│   │   ├── Contacts/
│   │   ├── EmailLogs/
│   │   ├── Users/
│   │   └── UserProfiles/
│   └── SMCV.Api/                     ← controllers, middleware, Program.cs (depende de todos)
│       ├── Controllers/
│       ├── Middleware/
│       ├── Program.cs
│       └── appsettings*.json
```

## FLUXO DE DADOS (CQRS)

```
HTTP Request
  -> Controller (recebe RequestDto, chama MediatR.Send())
    -> Handler (logica de negocio, usa Repository/Services)
      -> Repository (CRUD no banco via DbContext)
        -> Entity (modelo EF Core)
```
