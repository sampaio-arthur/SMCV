using MediatR;
using SMCV.Application.DTOs.EmailLogs;

namespace SMCV.Features.EmailLogs.Queries.GetEmailLogsByCampaign;

public record GetEmailLogsByCampaignQuery(Guid CampaignId) : IRequest<IEnumerable<EmailLogResponse>>;
