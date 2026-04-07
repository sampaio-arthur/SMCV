using SMCV.Application.DTOs.Contacts;

namespace SMCV.Application.DTOs.Campaigns;

public record CampaignDetailResponse(
    Guid Id,
    string Niche,
    string Region,
    string ResumeFileName,
    string EmailSubject,
    string EmailBody,
    string Status,
    DateTime CreatedAt,
    IEnumerable<ContactResponse> Contacts
);
