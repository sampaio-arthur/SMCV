namespace SMCV.Domain.Entities;

/// <summary>
/// Generic example entity. Replace with your domain entity.
/// </summary>
public class Example
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
