using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
    internal class TcpClientHandlerFactory : ISmtpServerClientHandlerFactory
    {
        internal X509Certificate ServerCertificate { get; set; }
        internal ILogger _logger { get; set; }

        public TcpClientHandlerFactory(X509Certificate certificate, ILogger logger)
        {
            ServerCertificate = certificate;
            _logger = logger;
        }

        public ISmtpServerClientProcessor Create(TcpClient client)
        {
            var tcpClientController = new TcpClientController(client, ServerCertificate, _logger);
            return new SmtpServerClientProcessor(tcpClientController, _logger);
        }
    }
}
