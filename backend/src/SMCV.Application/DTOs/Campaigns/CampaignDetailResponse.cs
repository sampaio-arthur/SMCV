using SMCV.Application.DTOs.Contacts;

namespace SMCV.Application.DTOs.Campaigns;

public class CampaignDetailResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Niche { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ContactResponse> Contacts { get; set; } = [];
}
