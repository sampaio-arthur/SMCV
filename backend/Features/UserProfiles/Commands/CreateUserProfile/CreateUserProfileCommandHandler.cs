using AutoMapper;
using MediatR;
using SMCV.Application.DTOs.UserProfiles;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Entities;

namespace SMCV.Features.UserProfiles.Commands.CreateUserProfile;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfileResponse>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateUserProfileCommandHandler(
        IUserProfileRepository userProfileRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileResponse> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        if (!await _userRepository.ExistsAsync(request.UserId))
            throw new NotFoundException("User", request.UserId);

        var existing = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (existing is not null)
            throw new BusinessException("Usuário já possui perfil.");

        var profile = new UserProfile
        {
            UserId = request.UserId,
            ResumeFilePath = request.ResumeFilePath
        };

        await _userProfileRepository.AddAsync(profile);
        return _mapper.Map<UserProfileResponse>(profile);
    }
}
