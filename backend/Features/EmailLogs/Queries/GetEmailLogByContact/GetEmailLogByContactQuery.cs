using MediatR;
using SMCV.Application.DTOs.EmailLogs;

namespace SMCV.Features.EmailLogs.Queries.GetEmailLogByContact;

public record GetEmailLogByContactQuery(Guid ContactId) : IRequest<IEnumerable<EmailLogResponse>>;
