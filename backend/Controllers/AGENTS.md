# Controllers — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Receber requisicoes HTTP, delegar ao Service e retornar respostas com status code correto.
Controllers NAO contem logica de negocio.

## REGRAS OBRIGATORIAS

- Decorar com `[ApiController]` e `[Route("api/[controller]")]`
- Herdar de `ControllerBase`
- Injetar o `IXxxService` via construtor (`private readonly`)
- Todos os metodos sao `async Task<ActionResult<T>>` (exceto DELETE que retorna `Task<IActionResult>`)
- Verificar retorno nulo do Service e retornar `NotFound()` quando aplicavel

## PROIBICOES

- **SEM** logica de negocio (if/else de regras, calculos, validacoes complexas)
- **SEM** acesso direto ao `AppDbContext` ou Repository
- **SEM** instanciacao de Entity — o controller trabalha apenas com DTOs
- **SEM** try/catch generico (deixar o middleware tratar excecoes)

## PADROES DE CODIGO

| Operacao | Verbo | Status Sucesso | Status Erro |
|----------|-------|---------------|-------------|
| Listar | `[HttpGet]` | `Ok(result)` 200 | — |
| Buscar | `[HttpGet("{id}")]` | `Ok(result)` 200 | `NotFound()` 404 |
| Criar | `[HttpPost]` | `CreatedAtAction()` 201 | 400 (automatico) |
| Atualizar | `[HttpPut("{id}")]` | `Ok(result)` 200 | `NotFound()` 404 |
| Deletar | `[HttpDelete("{id}")]` | `NoContent()` 204 | `NotFound()` 404 |

## REFERENCIA RAPIDA

```csharp
[ApiController]
[Route("api/[controller]")]
public class XxxController : ControllerBase
{
    private readonly IXxxService _service;

    public XxxController(IXxxService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<XxxResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<XxxResponseDto>> Create([FromBody] XxxRequestDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
```

> **ATENCAO:** `ApiResponse<T>` existe em `Utils/` mas NAO esta sendo usado nos controllers atuais. Os controllers retornam DTOs diretamente.
