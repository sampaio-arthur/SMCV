namespace SMCV.Application.DTOs.Campaigns;

public class CreateCampaignRequest
{
    public string Name { get; set; } = string.Empty;
    public string Niche { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
}
