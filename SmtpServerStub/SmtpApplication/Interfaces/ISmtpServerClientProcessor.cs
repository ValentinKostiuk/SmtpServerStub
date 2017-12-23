using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    public interface ISmtpServerClientProcessor
    {
        IMailMessage Run();
    }
}
