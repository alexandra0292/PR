using Email.Model.mail;

namespace Email.Interfaces
{
    public interface IMailsService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
