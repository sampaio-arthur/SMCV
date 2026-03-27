namespace DesenvWebApi.DTOs;

/// <summary>
/// DTO for creating or updating an Example resource.
/// </summary>
public class ExampleRequestDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
