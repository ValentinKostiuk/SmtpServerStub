using System.Collections.Generic;
using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    public interface ISmtpServer
    {
        void Start();
        List<IMailMessage> GetReceivedMails();
    }
}
