using FluentValidation;

namespace SMCV.Features.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Niche).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Region).NotEmpty().MaximumLength(255);
        RuleFor(x => x.EmailSubject).NotEmpty().MaximumLength(500);
        RuleFor(x => x.EmailBody).NotEmpty();
    }
}
