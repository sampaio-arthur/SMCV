using MediatR;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;

namespace SMCV.Features.UserProfiles.Commands.DeleteUserProfile;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, bool>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteUserProfileCommandHandler(
        IUserProfileRepository userProfileRepository,
        IFileStorageService fileStorageService)
    {
        _userProfileRepository = userProfileRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<bool> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("UserProfile", request.Id);

        if (!string.IsNullOrEmpty(profile.ResumeFilePath))
            await _fileStorageService.DeleteAsync(profile.ResumeFilePath, cancellationToken);

        await _userProfileRepository.DeleteAsync(profile);
        return true;
    }
}
