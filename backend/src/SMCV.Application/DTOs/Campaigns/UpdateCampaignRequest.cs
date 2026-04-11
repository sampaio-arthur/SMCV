namespace SMCV.Application.DTOs.Campaigns;

public class UpdateCampaignRequest
{
    public string Name { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
}
