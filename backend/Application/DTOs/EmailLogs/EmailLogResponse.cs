namespace SMCV.Application.DTOs.EmailLogs;

public record EmailLogResponse(
    Guid Id,
    string Status,
    DateTime? SentAt,
    string? ErrorMessage,
    DateTime CreatedAt
);
