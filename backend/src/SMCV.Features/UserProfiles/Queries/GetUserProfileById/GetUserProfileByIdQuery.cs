using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Queries.GetUserProfileById;

public record GetUserProfileByIdQuery(Guid Id) : IRequest<UserProfileResponse>;
