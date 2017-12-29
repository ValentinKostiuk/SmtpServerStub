using SmtpServerStub.Dtos;
using SmtpServerStub.Utilities.Interfaces;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    internal interface ISmtpServerClientProcessor
    {
		IEmailParser EmailParser { get; set; }
		IRequestCommandsConverter RequestCommandsConverter { get; set; }
		IServerStatusCodesConverter ServerStatusCodesConverter { get; set; }
		ILogger Logger { get; set; }
		MailMessage Run();
    }
}
