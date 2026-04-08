# Common — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Utilitarios compartilhados: pattern de resultado, excecoes customizadas, middleware transversal.
Nenhuma logica de negocio — apenas infraestrutura transversal.

## ESTRUTURA

```
Common/
├── Exceptions/
│   ├── BusinessException.cs
│   └── NotFoundException.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
└── ResultPattern/
    └── ApiResponse.cs
```

## ARQUIVOS EXISTENTES

### Exceptions (`SMCV.Common.Exceptions`)

| Arquivo | Descricao |
|---------|-----------|
| `BusinessException.cs` | Excecao para erros de logica de negocio. Recebe mensagem como parametro. |
| `NotFoundException.cs` | Excecao para entidade nao encontrada. Recebe tipo da entidade e ID, formata mensagem automaticamente. |

### Middleware (`SMCV.Common.Middleware`)

| Arquivo | Descricao |
|---------|-----------|
| `ExceptionHandlingMiddleware.cs` | Captura todas as excecoes e mapeia para HTTP status codes e JSON. Inclui detalhes de debug em Development. |

### ResultPattern (`SMCV.Common.ResultPattern`)

| Arquivo | Descricao |
|---------|-----------|
| `ApiResponse.cs` | Record generico `ApiResponse<T>` para respostas padronizadas da API com flag de sucesso, mensagem e dados. |

## REGRAS OBRIGATORIAS

- Namespace: `SMCV.Common.Exceptions`, `SMCV.Common.Middleware` ou `SMCV.Common.ResultPattern`
- Classes genericas e reutilizaveis — nao especificas de um dominio
- Excecoes herdam de `Exception` ou derivadas

## PROIBICOES

- **SEM** referencia a entidades, DTOs ou servicos especificos
- **SEM** logica de negocio
