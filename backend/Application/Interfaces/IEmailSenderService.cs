namespace SMCV.Application.Interfaces;

public interface IEmailSenderService
{
    /// <summary>
    /// Envia e-mail com anexo para o destinatário.
    /// Lança exceção em caso de falha — o caller deve tratar e registrar no EmailLog.
    /// </summary>
    Task SendEmailWithAttachmentAsync(
        string toEmail,
        string toName,
        string subject,
        string body,
        string attachmentPath,
        string attachmentFileName);
}
