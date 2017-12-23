using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    internal interface ISmtpServerClientProcessor
    {
        IMailMessage Run();
    }
}
