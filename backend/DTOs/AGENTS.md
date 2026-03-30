# DTOs — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Definir a forma dos dados que entram e saem da API.
Desacoplar a Entity da interface HTTP.

## REGRAS OBRIGATORIAS

- Nomenclatura: `XxxRequestDto` (entrada) e `XxxResponseDto` (saida)
- Namespace: `SMCV.DTOs`
- Um arquivo por DTO

### RequestDto (o que o cliente envia)
- **SEM** `Id` — o servidor gera
- **SEM** `CreatedAt`/`UpdatedAt` — o servidor controla
- Usar `required` para campos obrigatorios
- Usar `?` para campos opcionais
- Valor padrao quando aplicavel (ex: `bool IsActive { get; set; } = true;`)

### ResponseDto (o que a API retorna)
- **COM** `Id`, `CreatedAt`, `UpdatedAt`
- Todos os campos que o frontend precisa exibir
- Usar `string.Empty` como padrao para strings obrigatorias

## PROIBICOES

- **SEM** logica (metodos, calculos)
- **SEM** referencia a Entity (`using SMCV.Entities` nao deve aparecer)
- **SEM** heranca entre DTOs (nao criar `BaseDto`)
- **SEM** anotacoes de EF Core

## QUANDO CRIAR DTOs ESPECIFICOS

- Se Create e Update usam os mesmos campos: usar um unico `XxxRequestDto` (caso atual)
- Se diferem (ex: Update permite campos parciais): criar `XxxCreateDto` e `XxxUpdateDto`

## REFERENCIA RAPIDA

```csharp
// RequestDto
public class XxxRequestDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

// ResponseDto
public class XxxResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```
