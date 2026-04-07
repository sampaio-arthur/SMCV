namespace SMCV.Domain.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Position { get; set; }
    public string Source { get; set; } = string.Empty;
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public EmailLog? EmailLog { get; set; }
    public DateTime CreatedAt { get; set; }

    public Contact()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected Contact(bool _) { }
}
