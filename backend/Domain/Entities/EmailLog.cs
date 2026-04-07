namespace SMCV.Domain.Entities;

using SMCV.Domain.Enums;

public class EmailLog
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public EmailStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public EmailLog()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected EmailLog(bool _) { }
}
