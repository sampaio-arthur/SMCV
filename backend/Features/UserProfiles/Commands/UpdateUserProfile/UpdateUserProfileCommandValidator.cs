using FluentValidation;

namespace SMCV.Features.UserProfiles.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.ResumeFilePath)
            .MaximumLength(1000)
            .When(x => x.ResumeFilePath is not null);
    }
}
