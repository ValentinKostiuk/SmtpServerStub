using System.Net.Sockets;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    public interface ISmtpServerClientHandlerFactory
    {
        ISmtpServerClientProcessor Create(TcpClient client);
    }
}
