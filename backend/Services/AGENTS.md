# Services — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Conter toda a logica de negocio. Orquestrar chamadas ao Repository.
Converter entre Entity e DTO. Unica camada que conhece ambos.

## REGRAS OBRIGATORIAS

- Criar interface `IXxxService` e implementacao `XxxService`
- Interface define o contrato usando apenas DTOs (nunca Entity)
- Injetar `IXxxRepository` via construtor (`private readonly`)
- Mapeamento Entity -> DTO via metodo `private static XxxResponseDto MapToDto(Xxx e)`
- Mapeamento DTO -> Entity feito inline no metodo `CreateAsync`/`UpdateAsync`
- Retorno nulo com nullable: `Task<XxxResponseDto?>` para GetById e Update
- `CreatedAt = DateTime.UtcNow` no Create; `UpdatedAt = DateTime.UtcNow` no Update
- Todos os metodos sao `async Task<T>` com sufixo `Async`

## PROIBICOES

- **SEM** acesso direto ao `AppDbContext` — usar apenas o Repository
- **SEM** retorno de Entity — sempre converter para ResponseDto
- **SEM** dependencia de `HttpContext`, `Controller` ou qualquer conceito HTTP
- **SEM** referencia a outros Services (evitar acoplamento lateral)

## PADROES DE CODIGO

Assinaturas padrao da interface:
```csharp
Task<IEnumerable<XxxResponseDto>> GetAllAsync();
Task<XxxResponseDto?> GetByIdAsync(int id);
Task<XxxResponseDto> CreateAsync(XxxRequestDto dto);
Task<XxxResponseDto?> UpdateAsync(int id, XxxRequestDto dto);
Task<bool> DeleteAsync(int id);
```

Padrao do MapToDto:
```csharp
private static XxxResponseDto MapToDto(Xxx e) => new()
{
    Id = e.Id,
    // ... demais campos
    CreatedAt = e.CreatedAt,
    UpdatedAt = e.UpdatedAt
};
```

## REFERENCIA RAPIDA

- Create: instancia Entity com campos do DTO + `CreatedAt`, chama `_repository.CreateAsync`
- Update: busca entity via `_repository.GetByIdAsync`, retorna null se nao existe, atualiza campos + `UpdatedAt`
- Delete: delega direto para `_repository.DeleteAsync`, retorna `bool`
