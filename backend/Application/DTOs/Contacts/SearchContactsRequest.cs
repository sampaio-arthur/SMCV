namespace SMCV.Application.DTOs.Contacts;

public class SearchContactsRequest
{
    public Guid CampaignId { get; set; }
    public string Niche { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}
