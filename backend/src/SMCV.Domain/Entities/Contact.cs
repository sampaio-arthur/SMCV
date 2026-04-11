namespace SMCV.Domain.Entities;

using SMCV.Domain.Enums;

public class Contact
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmailStatus EmailStatus { get; set; }
    public DateTime? EmailSentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public EmailLog? EmailLog { get; set; }

    public Contact()
    {
        Id = Guid.NewGuid();
        EmailStatus = EmailStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    protected Contact(bool _) { }
}
