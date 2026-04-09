namespace SMCV.Application.DTOs.Contacts;

public class ContactResponse
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmailStatus { get; set; } = string.Empty;
    public DateTime? EmailSentAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
