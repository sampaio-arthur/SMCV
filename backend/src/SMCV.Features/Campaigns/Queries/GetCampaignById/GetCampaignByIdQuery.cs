using MediatR;
using SMCV.Application.DTOs.Campaigns;

namespace SMCV.Features.Campaigns.Queries.GetCampaignById;

public record GetCampaignByIdQuery(Guid Id) : IRequest<CampaignDetailResponse>;
