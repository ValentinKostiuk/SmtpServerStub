using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
    public class SmtpServer : ISmtpServer
    {
        private readonly TcpListener _tcpListener;
        private readonly IList<TcpClient> _clients;
        private readonly ISmtpServerClientHandlerFactory _clientHandlerFactory;
        private readonly List<IMailMessage> _receivedMessages;

        public SmtpServer(ISmtpServerSettings settings, ILogger logger)
        {
            var endPoint = new IPEndPoint(settings.IpAddress, settings.Port);
            _tcpListener = new TcpListener(endPoint);
            _clients = new List<TcpClient>();
            _receivedMessages = new List<IMailMessage>();
            var serverCertificate = new X509Certificate(settings.SslCertificateFilePath, settings.SslCertificatePassword, X509KeyStorageFlags.MachineKeySet);
            _clientHandlerFactory = new TcpClientHandlerFactory(serverCertificate);
        }

        public void Start()
        {
            _tcpListener.Start();

            while (true)
            {
                var client = _tcpListener.AcceptTcpClient();
                _clients.Add(client);
                var handler = _clientHandlerFactory.Create(client);
                var task = new Task<IMailMessage>(handler.Run);
                task.Start();
                _receivedMessages.Add(task.Result);
            }
        }

        public List<IMailMessage> GetReceivedMails()
        {
            return _receivedMessages;
        }
    }
}