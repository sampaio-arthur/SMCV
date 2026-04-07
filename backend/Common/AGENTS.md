# Common — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Utilitarios compartilhados: pattern de resultado, excecoes customizadas.
Nenhuma logica de negocio — apenas infraestrutura transversal.

## ESTRUTURA

```
Common/
├── Exceptions/      ← excecoes customizadas (NotFoundException, etc.)
└── ResultPattern/   ← Result<T>, ApiResponse<T>
```

## ARQUIVOS EXISTENTES

| Tipo | Arquivo | Descricao |
|------|---------|-----------|
| Result | `ResultPattern/ApiResponse.cs` | Wrapper generico para respostas da API |

## REGRAS OBRIGATORIAS

- Namespace: `SMCV.Common.Exceptions` ou `SMCV.Common.ResultPattern`
- Classes genericas e reutilizaveis — nao especificas de um dominio
- Excecoes herdam de `Exception` ou derivadas

## PROIBICOES

- **SEM** referencia a entidades, DTOs ou servicos especificos
- **SEM** logica de negocio
