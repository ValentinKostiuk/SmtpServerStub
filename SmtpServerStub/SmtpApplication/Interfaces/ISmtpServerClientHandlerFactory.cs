using System.Net.Sockets;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    internal interface ISmtpServerClientHandlerFactory
    {
        ISmtpServerClientProcessor Create(TcpClient client);
    }
}
