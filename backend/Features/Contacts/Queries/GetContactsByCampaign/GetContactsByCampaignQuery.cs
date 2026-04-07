using MediatR;
using SMCV.Application.DTOs.Contacts;

namespace SMCV.Features.Contacts.Queries.GetContactsByCampaign;

public record GetContactsByCampaignQuery(Guid CampaignId) : IRequest<IEnumerable<ContactResponse>>;
