namespace SMCV.Application.DTOs.Campaigns;

public record CampaignResponse(
    Guid Id,
    Guid UserId,
    string Name,
    string Niche,
    string Region,
    string EmailSubject,
    string EmailBody,
    string Status,
    DateTime CreatedAt,
    int TotalContacts
);
