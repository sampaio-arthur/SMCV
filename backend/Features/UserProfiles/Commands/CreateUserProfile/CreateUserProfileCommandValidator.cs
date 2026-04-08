using FluentValidation;

namespace SMCV.Features.UserProfiles.Commands.CreateUserProfile;

public class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
