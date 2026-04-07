namespace SMCV.Application.DTOs.Campaigns;

public record CampaignResponse(
    Guid Id,
    string Niche,
    string Region,
    string ResumeFileName,
    string EmailSubject,
    string EmailBody,
    string Status,
    DateTime CreatedAt,
    int TotalContacts
);
