using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.UserProfiles.Queries.GetUserProfileByUserId;

public class GetUserProfileByUserIdQueryHandler : IRequestHandler<GetUserProfileByUserIdQuery, UserProfileResponse>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public GetUserProfileByUserIdQueryHandler(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileResponse> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId)
            ?? throw new NotFoundException("UserProfile", request.UserId);

        return _mapper.Map<UserProfileResponse>(profile);
    }
}
