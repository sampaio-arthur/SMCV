using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Commands.CreateUserProfile;

public record CreateUserProfileCommand(Guid UserId, string? ResumeFilePath) : IRequest<UserProfileResponse>;
