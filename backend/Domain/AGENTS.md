# Domain — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Conter as entidades do dominio (modelos EF Core) e enums.
Esta camada NAO depende de nenhuma outra — e a base da aplicacao.

## ESTRUTURA

```
Domain/
├── Entities/    ← classes mapeadas para tabelas do banco
└── Enums/       ← enumeracoes do dominio
```

## REGRAS OBRIGATORIAS

- Namespace: `SMCV.Domain.Entities` ou `SMCV.Domain.Enums`
- Toda entidade tem `Id` (int, PK auto-increment)
- Campos obrigatorios usam `required`
- Campos opcionais usam `?` (nullable)
- Timestamps: `CreatedAt` (DateTime) e `UpdatedAt` (DateTime?)
- Default de `CreatedAt` = `DateTime.UtcNow`
- Default de `IsActive` = `true` (quando aplicavel)

## PROIBICOES

- **SEM** logica de negocio nas entidades
- **SEM** referencia a DTOs, Services ou Controllers
- **SEM** atributos de validacao (`[Required]`, `[MaxLength]`) — usar Fluent API no DbContext
