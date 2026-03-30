# Utils — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Helpers genericos, wrappers e extensoes reutilizaveis.
Codigo que nao pertence a nenhuma camada especifica.

## REGRAS OBRIGATORIAS

- Classes devem ser **estaticas** ou **sem estado** (stateless)
- Namespace: `SMCV.Utils`
- Funcionalidade deve ser generica — util para qualquer parte da aplicacao

## PROIBICOES

- **SEM** dependencia de Services, Repositories, Controllers ou Entities
- **SEM** acesso ao DbContext
- **SEM** injecao de dependencia (utils sao autocontidos)
- **SEM** estado mutavel (campos de instancia)

## ApiResponse<T>

Wrapper generico para respostas padronizadas da API.

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null);
    public static ApiResponse<T> Fail(string message);
}
```

Uso no Controller:
```csharp
return Ok(ApiResponse<XxxResponseDto>.Ok(result));
return BadRequest(ApiResponse<object>.Fail("Mensagem de erro"));
```

> **ATENCAO:** `ApiResponse<T>` existe mas NAO esta sendo usado nos controllers atuais.
> Os controllers retornam DTOs diretamente. Decidir com o time antes de adotar.

## REFERENCIA RAPIDA

Ao criar novo utilitario:
1. Criar classe estatica em `Utils/`
2. Metodos devem ser `static` e puros (sem side effects)
3. Nao precisa registrar no DI
