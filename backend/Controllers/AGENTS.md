# Controllers — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Receber requisicoes HTTP e delegar para MediatR. Retornar respostas com status code correto.
Controllers NAO contem logica de negocio.
Na arquitetura CQRS, controllers chamam `_mediator.Send()` em vez de chamar Services diretamente.

## ARQUIVOS EXISTENTES

| Arquivo | Namespace | Descricao |
|---------|-----------|-----------|
| `CampaignsController.cs` | `SMCV.Controllers` | CRUD de campanhas, envio de emails, exportacao CSV. Recebe [FromBody] (sem multipart/IFormFile). Requer [Authorize]. |
| `ContactsController.cs` | `SMCV.Controllers` | CRUD de contatos, busca via Hunter.io, filtro por campanha. Requer [Authorize]. |
| `EmailLogsController.cs` | `SMCV.Controllers` | Consulta de logs de email por contato ou por campanha. Requer [Authorize]. |
| `UsersController.cs` | `SMCV.Controllers` | CRUD de usuarios. POST e [AllowAnonymous], demais endpoints requerem [Authorize]. |
| `UserProfilesController.cs` | `SMCV.Controllers` | CRUD de perfis de usuario e endpoint upload-resume. Requer [Authorize]. |

## REGRAS OBRIGATORIAS

- Decorar com `[ApiController]` e `[Route("api/[controller]")]`
- Herdar de `ControllerBase`
- Todos os metodos sao `async Task<ActionResult<T>>` (exceto DELETE que retorna `Task<IActionResult>`)
- Verificar retorno nulo e retornar `NotFound()` quando aplicavel
- Injetar `IMediator` via construtor

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
