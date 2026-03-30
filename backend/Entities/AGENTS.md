# Entities — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Mapeamento direto para tabela do banco de dados via EF Core.
Cada Entity = uma tabela. Propriedades = colunas.

## REGRAS OBRIGATORIAS

- Nomenclatura: **PascalCase**, **singular**, **ingles** (ex: `Product`, nao `Produtos`)
- Namespace: `DesenvWebApi.Entities`
- Campos obrigatorios em toda Entity:
  - `public int Id { get; set; }` — chave primaria
  - `public DateTime CreatedAt { get; set; } = DateTime.UtcNow;`
  - `public DateTime? UpdatedAt { get; set; }`
- Usar `required` para campos obrigatorios que nao tem valor padrao (ex: `required string Name`)
- Usar `?` (nullable) para campos opcionais (ex: `string? Description`)
- Valor padrao com `= true` ou `= string.Empty` quando fizer sentido

## PROIBICOES

- **SEM** logica de negocio (metodos, calculos)
- **SEM** anotacoes de validacao de API (`[Required]`, `[MaxLength]`) — validacao fica no DTO/Service
- **SEM** referencia a DTOs ou Services

## RELACIONAMENTOS

Relacionamento 1:N (exemplo comentado):
```csharp
// Na Entity "pai" (ex: Category)
public ICollection<Product> Products { get; set; } = [];

// Na Entity "filha" (ex: Product)
public int CategoryId { get; set; }          // FK explicita
public Category Category { get; set; } = null!; // navegacao
```

Relacionamento 1:1:
```csharp
public int ProfileId { get; set; }
public Profile Profile { get; set; } = null!;
```

> Sempre declarar FK explicita + propriedade de navegacao.

## REFERENCIA RAPIDA

```csharp
namespace DesenvWebApi.Entities;

public class Xxx
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```
