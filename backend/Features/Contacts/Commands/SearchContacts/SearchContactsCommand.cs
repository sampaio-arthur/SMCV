using MediatR;
using SMCV.Application.DTOs.Contacts;

namespace SMCV.Features.Contacts.Commands.SearchContacts;

public record SearchContactsCommand(
    Guid CampaignId,
    string Niche,
    string Region,
    int Limit
) : IRequest<SearchContactsResponse>;
