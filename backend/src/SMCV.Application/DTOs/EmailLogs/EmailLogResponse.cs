namespace SMCV.Application.DTOs.EmailLogs;

public record EmailLogResponse(
    Guid Id,
    Guid ContactId,
    string? ErrorMessage,
    DateTime CreatedAt
);
