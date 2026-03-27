using Microsoft.AspNetCore.Mvc;
using DesenvWebApi.DTOs;
using DesenvWebApi.Services;

namespace DesenvWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IExampleService _service;

    public ExampleController(IExampleService service)
    {
        _service = service;
    }

    // GET /api/example
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExampleResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // GET /api/example/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ExampleResponseDto>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(new { message = $"Example with ID {id} not found." });

        return Ok(result);
    }

    // POST /api/example
    [HttpPost]
    public async Task<ActionResult<ExampleResponseDto>> Create([FromBody] ExampleRequestDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT /api/example/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ExampleResponseDto>> Update(int id, [FromBody] ExampleRequestDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(new { message = $"Example with ID {id} not found." });

        return Ok(result);
    }

    // DELETE /api/example/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Example with ID {id} not found." });

        return NoContent();
    }
}
