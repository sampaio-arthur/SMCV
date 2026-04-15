using MediatR;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IFileStorageService fileStorageService)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("User", request.Id);

        var profile = await _userProfileRepository.GetByUserIdAsync(user.Id);

        if (profile is not null && !string.IsNullOrEmpty(profile.ResumeFilePath))
            await _fileStorageService.DeleteAsync(profile.ResumeFilePath, cancellationToken);

        await _userRepository.DeleteAsync(user);
        return true;
    }
}
