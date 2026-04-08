namespace SMCV.Application.DTOs.Contacts;

public record ContactResponse(
    Guid Id,
    Guid CampaignId,
    string CompanyName,
    string Email,
    string EmailStatus,
    DateTime? EmailSentAt,
    DateTime CreatedAt
);
