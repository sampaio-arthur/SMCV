using MediatR;

namespace SMCV.Features.UserProfiles.Commands.DeleteUserProfile;

public record DeleteUserProfileCommand(Guid Id) : IRequest<bool>;
