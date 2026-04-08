using SMCV.Application.DTOs.Contacts;

namespace SMCV.Application.DTOs.Campaigns;

public record CampaignDetailResponse(
    Guid Id,
    Guid UserId,
    string Name,
    string Niche,
    string Region,
    string EmailSubject,
    string EmailBody,
    string Status,
    DateTime CreatedAt,
    IEnumerable<ContactResponse> Contacts
);
