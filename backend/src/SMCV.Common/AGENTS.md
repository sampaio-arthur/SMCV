# Common — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Utilitarios compartilhados: pattern de resultado, excecoes customizadas, middleware transversal.
Nenhuma logica de negocio — apenas infraestrutura transversal.

## ESTRUTURA

```
src/SMCV.Common/
├── Exceptions/
│   ├── BusinessException.cs
│   └── NotFoundException.cs
└── ResultPattern/
    └── ApiResponse.cs
```

> **Nota:** `ExceptionHandlingMiddleware` foi movido para `src/SMCV.Api/Middleware/` pois depende de ASP.NET Core (HttpContext, RequestDelegate). O namespace permanece `SMCV.Common.Middleware` por compatibilidade.

## ARQUIVOS EXISTENTES

### Exceptions (`SMCV.Common.Exceptions`)

| Arquivo | Descricao |
|---------|-----------|
| `BusinessException.cs` | Excecao para erros de logica de negocio. Recebe mensagem como parametro. |
| `NotFoundException.cs` | Excecao para entidade nao encontrada. Recebe tipo da entidade e ID, formata mensagem automaticamente. |

### ResultPattern (`SMCV.Common.ResultPattern`)

| Arquivo | Descricao |
|---------|-----------|
| `ApiResponse.cs` | Record generico `ApiResponse<T>` para respostas padronizadas da API com flag de sucesso, mensagem e dados. |

## REGRAS OBRIGATORIAS

- Namespace: `SMCV.Common.Exceptions` ou `SMCV.Common.ResultPattern`
- Classes genericas e reutilizaveis — nao especificas de um dominio
- Excecoes herdam de `Exception` ou derivadas

## PROIBICOES

- **SEM** referencia a entidades, DTOs ou servicos especificos
- **SEM** logica de negocio
