using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(Guid Id, string? ResumeFilePath) : IRequest<UserProfileResponse>;
