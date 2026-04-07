using SMCV.Application.DTOs.EmailLogs;

namespace SMCV.Application.DTOs.Contacts;

public record ContactResponse(
    Guid Id,
    string CompanyName,
    string Email,
    string Domain,
    string? ContactName,
    string? Position,
    string Source,
    Guid CampaignId,
    DateTime CreatedAt,
    EmailLogResponse? EmailLog
);
