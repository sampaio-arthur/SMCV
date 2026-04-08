using FluentValidation;

namespace SMCV.Features.Contacts.Commands.CreateContact;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x => x.CampaignId).NotEmpty();
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
    }
}
