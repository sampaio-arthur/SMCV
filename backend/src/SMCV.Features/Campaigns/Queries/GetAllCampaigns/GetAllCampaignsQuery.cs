using MediatR;
using SMCV.Application.DTOs.Campaigns;

namespace SMCV.Features.Campaigns.Queries.GetAllCampaigns;

public record GetAllCampaignsQuery() : IRequest<IEnumerable<CampaignResponse>>;
