using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.EmailLogs;
using SMCV.Application.Interfaces;

namespace SMCV.Features.EmailLogs.Queries.GetEmailLogsByCampaign;

public class GetEmailLogsByCampaignQueryHandler : IRequestHandler<GetEmailLogsByCampaignQuery, IEnumerable<EmailLogResponse>>
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IMapper _mapper;

    public GetEmailLogsByCampaignQueryHandler(IEmailLogRepository emailLogRepository, IMapper mapper)
    {
        _emailLogRepository = emailLogRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmailLogResponse>> Handle(GetEmailLogsByCampaignQuery request, CancellationToken cancellationToken)
    {
        var emailLogs = await _emailLogRepository.GetByCampaignIdAsync(request.CampaignId);

        return _mapper.Map<IEnumerable<EmailLogResponse>>(emailLogs);
    }
}
