using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SMCV.Application.Interfaces;

namespace SMCV.Infrastructure.ExternalServices;

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailSettings _settings;

    public EmailSenderService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailWithAttachmentAsync(
        string toEmail, string toName,
        string subject, string body,
        byte[] attachmentBytes, string attachmentFileName,
        string replyToEmail, string replyToName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.ReplyTo.Add(new MailboxAddress(replyToName, replyToEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        builder.Attachments.Add(attachmentFileName, attachmentBytes);
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
