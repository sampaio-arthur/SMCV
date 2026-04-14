using MediatR;
using SMCV.Application.Interfaces;
using SMCV.Common.Exceptions;
using SMCV.Domain.Entities;
using SMCV.Domain.Enums;

namespace SMCV.Features.Campaigns.Commands.SendCampaignEmails;

public class SendCampaignEmailsCommandHandler : IRequestHandler<SendCampaignEmailsCommand, int>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IContactRepository _contactRepository;
    private readonly IEmailLogRepository _emailLogRepository;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public SendCampaignEmailsCommandHandler(
        ICampaignRepository campaignRepository,
        IContactRepository contactRepository,
        IEmailLogRepository emailLogRepository,
        IEmailSenderService emailSenderService,
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository)
    {
        _campaignRepository = campaignRepository;
        _contactRepository = contactRepository;
        _emailLogRepository = emailLogRepository;
        _emailSenderService = emailSenderService;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<int> Handle(SendCampaignEmailsCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdWithContactsAsync(request.CampaignId)
            ?? throw new NotFoundException("Campaign", request.CampaignId);

        var userProfile = await _userProfileRepository.GetByUserIdAsync(campaign.UserId)
            ?? throw new BusinessException("Perfil do usuário não encontrado. Configure seu perfil antes de disparar emails.");

        if (string.IsNullOrEmpty(userProfile.ResumeFilePath))
            throw new BusinessException("Nenhum currículo encontrado no perfil. Faça upload do currículo antes de disparar emails.");

        var user = await _userRepository.GetByIdAsync(campaign.UserId)
            ?? throw new NotFoundException("User", campaign.UserId);

        campaign.Status = CampaignStatus.Running;
        await _campaignRepository.UpdateAsync(campaign);

        var successCount = 0;
        var failureCount = 0;

        try
        {
            foreach (var contact in campaign.Contacts)
            {
                if (contact.EmailStatus == EmailStatus.Sent)
                    continue;

                try
                {
                    await _emailSenderService.SendEmailWithAttachmentAsync(
                        toEmail: contact.Email,
                        toName: contact.CompanyName,
                        subject: campaign.EmailSubject,
                        body: campaign.EmailBody,
                        attachmentPath: userProfile.ResumeFilePath,
                        attachmentFileName: Path.GetFileName(userProfile.ResumeFilePath),
                        replyToEmail: user.Email,
                        replyToName: user.Name);

                    contact.EmailStatus = EmailStatus.Sent;
                    contact.EmailSentAt = DateTime.UtcNow;
                    await _contactRepository.UpdateAsync(contact);

                    if (contact.EmailLog is null)
                    {
                        var emailLog = new EmailLog
                        {
                            ContactId = contact.Id,
                            ErrorMessage = null
                        };
                        await _emailLogRepository.AddAsync(emailLog);
                    }
                    else
                    {
                        contact.EmailLog.ErrorMessage = null;
                        await _emailLogRepository.UpdateAsync(contact.EmailLog);
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    contact.EmailStatus = EmailStatus.Failed;
                    await _contactRepository.UpdateAsync(contact);

                    if (contact.EmailLog is null)
                    {
                        var emailLog = new EmailLog
                        {
                            ContactId = contact.Id,
                            ErrorMessage = ex.Message
                        };
                        await _emailLogRepository.AddAsync(emailLog);
                    }
                    else
                    {
                        contact.EmailLog.ErrorMessage = ex.Message;
                        await _emailLogRepository.UpdateAsync(contact.EmailLog);
                    }

                    failureCount++;
                }
            }
        }
        finally
        {
            if (successCount == 0 && failureCount > 0)
                campaign.Status = CampaignStatus.Failed;
            else if (failureCount == 0 && successCount > 0)
                campaign.Status = CampaignStatus.Completed;
            else if (successCount > 0 && failureCount > 0)
                campaign.Status = CampaignStatus.PartialSuccess;
            else
                campaign.Status = CampaignStatus.Completed;

            await _campaignRepository.UpdateAsync(campaign);
        }

        return successCount;
    }
}
