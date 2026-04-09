using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.EmailLogs;
using SMCV.Application.Interfaces;

namespace SMCV.Features.EmailLogs.Queries.GetEmailLogByContact;

public class GetEmailLogByContactQueryHandler : IRequestHandler<GetEmailLogByContactQuery, IEnumerable<EmailLogResponse>>
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IMapper _mapper;

    public GetEmailLogByContactQueryHandler(IEmailLogRepository emailLogRepository, IMapper mapper)
    {
        _emailLogRepository = emailLogRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmailLogResponse>> Handle(GetEmailLogByContactQuery request, CancellationToken cancellationToken)
    {
        var emailLogs = await _emailLogRepository.GetByContactIdAsync(request.ContactId);

        return _mapper.Map<IEnumerable<EmailLogResponse>>(emailLogs);
    }
}
