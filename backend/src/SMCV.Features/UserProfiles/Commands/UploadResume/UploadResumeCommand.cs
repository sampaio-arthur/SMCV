using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Commands.UploadResume;

public record UploadResumeCommand(Guid Id, string ResumeFilePath) : IRequest<UserProfileResponse>;
