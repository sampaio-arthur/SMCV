namespace SMCV.Application.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailWithAttachmentAsync(
        string toEmail,
        string toName,
        string subject,
        string body,
        string attachmentPath,
        string attachmentFileName,
        string fromEmail,
        string fromName);
}
