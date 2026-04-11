using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.Interfaces;

namespace SMCV.Features.UserProfiles.Queries.GetAllUserProfiles;

public class GetAllUserProfilesQueryHandler : IRequestHandler<GetAllUserProfilesQuery, IEnumerable<UserProfileResponse>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public GetAllUserProfilesQueryHandler(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserProfileResponse>> Handle(GetAllUserProfilesQuery request, CancellationToken cancellationToken)
    {
        var profiles = await _userProfileRepository.GetAllPagedAsync(request.PageNumber, request.PageSize);
        return _mapper.Map<IEnumerable<UserProfileResponse>>(profiles);
    }
}
