using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
    internal class TcpClientHandlerFactory : ISmtpServerClientHandlerFactory
    {
        internal X509Certificate ServerCertificate { get; set; }

        public TcpClientHandlerFactory(X509Certificate certificate)
        {
            ServerCertificate = certificate;
        }

        public ISmtpServerClientProcessor Create(TcpClient client)
        {
            var tcpClientController = new TcpClientController(client, ServerCertificate);
            return new SmtpServerClientProcessor(tcpClientController);
        }
    }
}
