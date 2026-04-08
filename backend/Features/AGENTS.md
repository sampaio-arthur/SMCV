# Features — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Nucleo CQRS da aplicacao. Cada dominio tem sua pasta com Commands (escrita) e Queries (leitura).
Handlers contem toda a logica de negocio e sao invocados via `MediatR.Send()`.

## ESTRUTURA

```
Features/
├── Contacts/
│   ├── Commands/    ← criar, atualizar, deletar contatos
│   └── Queries/     ← listar, buscar contatos
├── Campaigns/
│   ├── Commands/    ← criar, atualizar, deletar campanhas
│   └── Queries/     ← listar, buscar campanhas
└── EmailLogs/
    ├── Commands/    ← registrar envio de e-mail
    └── Queries/     ← listar logs de envio
```

## ESTADO ATUAL

Pastas criadas mas ainda sem handlers implementados.
Novos dominios (Contacts, Campaigns, EmailLogs) serao implementados diretamente como handlers CQRS.

## REGRAS OBRIGATORIAS (para implementacoes futuras)

- Namespace: `SMCV.Features.{Dominio}.Commands` ou `SMCV.Features.{Dominio}.Queries`
- Cada handler em arquivo proprio: `CreateXxxCommand.cs`, `GetXxxByIdQuery.cs`, etc.
- Command/Query + Handler no mesmo arquivo
- Handler injeta Repository via construtor
- Retorno via `Result<T>` ou DTO

## PROIBICOES

- **SEM** acesso direto ao `DbContext` — usar Repository
- **SEM** referencia a `HttpContext` ou conceitos HTTP
- **SEM** dependencia lateral entre Features de dominios diferentes
