# Infrastructure — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Implementar os contratos definidos em `Application/Interfaces/`.
Contem acesso a banco, servicos concretos e integracoes externas.

## ESTRUTURA

```
Infrastructure/
├── Data/
│   ├── AppDbContext.cs       ← DbContext do EF Core
│   └── Migrations/           ← migrations geradas pelo EF Core
├── Repositories/             ← implementacao de repositorios
├── Services/                 ← implementacao de servicos (legado)
└── ExternalServices/         ← integracoes com APIs externas
```

## ARQUIVOS EXISTENTES

| Tipo | Arquivo | Descricao |
|------|---------|-----------|
| DbContext | `Data/AppDbContext.cs` | Contexto do banco com DbSets |
| Repository | `Repositories/ExampleRepository.cs` | CRUD de Example no banco |
| Service | `Services/ExampleService.cs` | Logica de negocio do Example (legado) |
| Migration | `Data/Migrations/InitialCreate` | Criacao da tabela Examples |

## REGRAS OBRIGATORIAS — Repositories

- Namespace: `SMCV.Infrastructure.Repositories`
- Implementar interface de `Application/Interfaces/`
- Injetar `AppDbContext` via construtor
- Usar metodos async do EF Core (`ToListAsync`, `FindAsync`, etc.)
- `SaveChangesAsync()` apos cada operacao de escrita

## REGRAS OBRIGATORIAS — DbContext

- Namespace: `SMCV.Infrastructure.Data`
- Um `DbSet<T>` por entidade do dominio
- Configuracoes Fluent API no `OnModelCreating` (quando necessario)

## REGRAS OBRIGATORIAS — Migrations

- Namespace: `SMCV.Infrastructure.Data.Migrations`
- Gerar via `dotnet ef migrations add NomeDaMigration`
- NAO editar migrations geradas manualmente
- Auto-migrate habilitado no `Program.cs`

## PROIBICOES

- **SEM** logica de negocio nos Repositories — apenas CRUD
- **SEM** retorno de DTOs nos Repositories — trabalhar com Entities
