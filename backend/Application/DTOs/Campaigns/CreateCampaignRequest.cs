namespace SMCV.Application.DTOs.Campaigns;

public class CreateCampaignRequest
{
    public string Niche { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string EmailSubject { get; set; } = string.Empty;
    public string EmailBody { get; set; } = string.Empty;
    // O arquivo de currículo será recebido via IFormFile no Controller
    // O path e nome do arquivo serão resolvidos pelo Command Handler
}
