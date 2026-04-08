namespace SMCV.Domain.Entities;

public class EmailLog
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public EmailLog()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected EmailLog(bool _) { }
}
