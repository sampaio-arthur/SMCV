namespace SMCV.Application.DTOs.Campaigns;

public class UpdateCampaignRequest
{
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
}
