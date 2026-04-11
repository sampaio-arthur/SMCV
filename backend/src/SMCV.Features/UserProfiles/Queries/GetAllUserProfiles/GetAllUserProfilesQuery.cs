using MediatR;
using SMCV.Application.DTOs.UserProfiles;

namespace SMCV.Features.UserProfiles.Queries.GetAllUserProfiles;

public record GetAllUserProfilesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<IEnumerable<UserProfileResponse>>;
