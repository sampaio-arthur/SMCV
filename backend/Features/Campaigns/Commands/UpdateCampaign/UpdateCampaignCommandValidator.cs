using FluentValidation;

namespace SMCV.Features.Campaigns.Commands.UpdateCampaign;

public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.EmailSubject).NotEmpty().MaximumLength(500);
        RuleFor(x => x.EmailBody).NotEmpty();
    }
}
