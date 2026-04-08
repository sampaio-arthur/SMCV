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
        string attachmentPath, string attachmentFileName,
        string fromEmail, string fromName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        builder.Attachments.Add(attachmentFileName, await File.ReadAllBytesAsync(attachmentPath));
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
