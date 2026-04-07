using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.EmailLogs;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.EmailLogs.Queries.GetEmailLogByContact;

public class GetEmailLogByContactQueryHandler : IRequestHandler<GetEmailLogByContactQuery, EmailLogResponse>
{
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IMapper _mapper;

    public GetEmailLogByContactQueryHandler(IEmailLogRepository emailLogRepository, IMapper mapper)
    {
        _emailLogRepository = emailLogRepository;
        _mapper = mapper;
    }

    public async Task<EmailLogResponse> Handle(GetEmailLogByContactQuery request, CancellationToken cancellationToken)
    {
        var emailLog = await _emailLogRepository.GetByContactIdAsync(request.ContactId)
            ?? throw new NotFoundException("EmailLog", request.ContactId);

        return _mapper.Map<EmailLogResponse>(emailLog);
    }
}
