namespace SMCV.Domain.Entities;

using SMCV.Domain.Enums;

public class Campaign
{
    public Guid Id { get; set; }
    public string Niche { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string ResumeFileName { get; set; } = string.Empty;
    public string ResumeFilePath { get; set; } = string.Empty;
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
