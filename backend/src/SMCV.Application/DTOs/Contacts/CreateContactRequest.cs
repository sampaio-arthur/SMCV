namespace SMCV.Application.DTOs.Contacts;

public record CreateContactRequest(
    Guid CampaignId,
    string CompanyName,
    string Email
);
