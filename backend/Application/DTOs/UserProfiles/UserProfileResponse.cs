namespace SMCV.Application.DTOs.UserProfiles;

public record UserProfileResponse(
    Guid Id,
    Guid UserId,
    string? ResumeFilePath,
    DateTime CreatedAt
);
