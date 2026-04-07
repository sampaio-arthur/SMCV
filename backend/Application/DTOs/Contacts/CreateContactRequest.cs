namespace SMCV.Application.DTOs.Contacts;

public class CreateContactRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Position { get; set; }
    public Guid CampaignId { get; set; }
}
