using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.UserProfiles.Queries.GetUserProfileById;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileResponse>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public GetUserProfileByIdQueryHandler(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileResponse> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("UserProfile", request.Id);

        return _mapper.Map<UserProfileResponse>(profile);
    }
}
