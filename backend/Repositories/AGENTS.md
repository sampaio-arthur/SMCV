# Repositories — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Acesso direto ao banco via `AppDbContext`. CRUD puro, sem logica de negocio.
Trabalha exclusivamente com Entities.

## REGRAS OBRIGATORIAS

- Criar interface `IXxxRepository` e implementacao `XxxRepository`
- Injetar `AppDbContext` via construtor (`private readonly`)
- Todas as operacoes sao `async` com sufixo `Async`
- Chamar `SaveChangesAsync()` apos cada operacao de escrita (Create, Update, Delete)
- Interface define contrato usando apenas Entity (nunca DTO)

## PROIBICOES

- **SEM** logica de negocio (validacoes, regras, calculos)
- **SEM** mapeamento para DTO — retornar sempre Entity
- **SEM** referencia a Services ou Controllers

## OPERACOES PADRAO

| Metodo | Implementacao | Retorno |
|--------|--------------|---------|
| `GetAllAsync` | `_context.Xxxs.ToListAsync()` | `IEnumerable<Xxx>` |
| `GetByIdAsync` | `_context.Xxxs.FindAsync(id)` | `Xxx?` |
| `CreateAsync` | `Add(entity)` + `SaveChangesAsync()` | `Xxx` |
| `UpdateAsync` | `Update(entity)` + `SaveChangesAsync()` | `Xxx` |
| `DeleteAsync` | `FindAsync` + `Remove` + `SaveChangesAsync()` | `bool` |

## QUANDO USAR INCLUDE

- Usar `.Include(x => x.Relacionamento)` quando a Entity tiver navegacao e o Service precisar dos dados relacionados
- NAO usar Include por padrao — apenas quando explicitamente necessario

## FindAsync vs FirstOrDefaultAsync

- `FindAsync(id)` — busca por chave primaria, usa cache do DbContext. Preferir para buscas por Id.
- `FirstOrDefaultAsync(x => ...)` — busca por qualquer campo. Usar para filtros complexos.

## REFERENCIA RAPIDA

```csharp
public class XxxRepository : IXxxRepository
{
    private readonly AppDbContext _context;

    public XxxRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Xxx>> GetAllAsync()
    {
        return await _context.Xxxs.ToListAsync();
    }

    public async Task<Xxx?> GetByIdAsync(int id)
    {
        return await _context.Xxxs.FindAsync(id);
    }
}
```
