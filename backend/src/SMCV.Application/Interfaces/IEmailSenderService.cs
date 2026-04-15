namespace SMCV.Application.Interfaces;

public interface IEmailSenderService
{
    Task SendEmailWithAttachmentAsync(
        string toEmail,
        string toName,
        string subject,
        string body,
        byte[] attachmentBytes,
        string attachmentFileName,
        string replyToEmail,
        string replyToName);
}
