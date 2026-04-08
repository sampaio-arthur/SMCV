using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Queries.GetUserProfileByUserId;

public record GetUserProfileByUserIdQuery(Guid UserId) : IRequest<UserProfileResponse>;
