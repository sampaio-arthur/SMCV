namespace SMCV.Application.DTOs.Contacts;

public record SearchContactsResponse(
    int TotalFound,
    IEnumerable<ContactResponse> Contacts
);
