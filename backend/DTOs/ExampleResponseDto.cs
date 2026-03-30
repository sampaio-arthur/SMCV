namespace SMCV.DTOs;

/// <summary>
/// DTO returned by the API for Example resources.
/// </summary>
public class ExampleResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
