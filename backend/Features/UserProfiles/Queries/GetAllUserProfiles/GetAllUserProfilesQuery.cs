using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Queries.GetAllUserProfiles;

public record GetAllUserProfilesQuery() : IRequest<IEnumerable<UserProfileResponse>>;
