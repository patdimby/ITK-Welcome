using System.Net.Mail;

namespace Intitek.Welcome.Service.Back
{
    public interface IMailService
    {
        void Send(MailMessage mail);
        void Historize();
    }
}
