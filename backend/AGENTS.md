# Backend — AGENTS.md

> **PONTO DE ENTRADA — Leia este arquivo primeiro.**
> Navegue para o MD especifico conforme sua tarefa.

## VISAO GERAL

Arquitetura em 3 camadas: **Controller -> Service -> Repository -> PostgreSQL**.
Framework: ASP.NET Core 8 com EF Core. Namespace raiz: `SMCV`.
Controllers recebem/retornam DTOs. Services orquestram logica. Repositories acessam o banco.
Entidades nunca sao expostas na API. Injecao de dependencia via interfaces com `AddScoped`.

## NAVIGATION MAP

| Tarefa | Arquivo a ler |
|--------|--------------|
| Criar/alterar endpoint HTTP | Controllers/AGENTS.md |
| Criar/alterar regra de negocio | Services/AGENTS.md |
| Criar/alterar acesso a dados | Repositories/AGENTS.md |
| Criar/alterar entidade de banco | Entities/AGENTS.md |
| Criar/alterar DTO | DTOs/AGENTS.md |
| Alterar DbContext ou migrations | Data/AGENTS.md |
| Criar/alterar utilitario compartilhado | Utils/AGENTS.md |
| Alterar pipeline ou DI | PROGRAM_AGENTS.md |

## REGRAS GLOBAIS

1. **Sempre usar DTOs** — nunca expor `Entity` em controllers ou retornos de service
2. **Interfaces obrigatorias** — todo Service e Repository tem sua interface (`IXxx`)
3. **Async em tudo** — todas as operacoes de I/O usam `async/await` com sufixo `Async`
4. **Namespace** — seguir `SMCV.{Camada}` (ex: `SMCV.Services`)
5. **Injecao via construtor** — nunca usar `[FromServices]` ou service locator
6. **Uma classe por arquivo** — nome do arquivo = nome da classe
7. **PascalCase** para classes, metodos e propriedades
8. **Timestamps UTC** — sempre `DateTime.UtcNow` para `CreatedAt`/`UpdatedAt`
9. **Registrar no DI** — todo novo Repository e Service deve ser registrado em `Program.cs`

## FLUXO DE DADOS

```
HTTP Request
  -> Controller (recebe RequestDto, valida rota)
    -> Service (logica de negocio, mapeia Entity <-> ResponseDto)
      -> Repository (CRUD no banco via DbContext)
        -> Entity (modelo EF Core)
```
