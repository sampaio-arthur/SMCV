namespace SMCV.Domain.Entities;

using SMCV.Domain.Enums;

public class Campaign
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Niche { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
    public CampaignStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Contact> Contacts { get; set; }

    public Campaign()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Contacts = new List<Contact>();
    }

    protected Campaign(bool _)
    {
        Contacts = new List<Contact>();
    }
}
