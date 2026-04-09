using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.UserProfiles.Commands.UploadResume;

public class UploadResumeCommandHandler : IRequestHandler<UploadResumeCommand, UserProfileResponse>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public UploadResumeCommandHandler(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileResponse> Handle(UploadResumeCommand request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("UserProfile", request.Id);

        profile.ResumeFilePath = request.ResumeFilePath;

        await _userProfileRepository.UpdateAsync(profile);
        return _mapper.Map<UserProfileResponse>(profile);
    }
}
