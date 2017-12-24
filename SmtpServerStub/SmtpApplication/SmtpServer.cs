using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
    /// <inheritdoc />
    public class SmtpServer : ISmtpServer
    {
        private readonly TcpListener _tcpListener;
        private readonly ISmtpServerClientHandlerFactory _clientHandlerFactory;
        private readonly List<IMailMessage> _receivedMessages;
        private readonly ILogger _logger;

        /// <inheritdoc />
        public SmtpServer(ISmtpServerSettings settings, ILogger logger = null)
        {
            var endPoint = new IPEndPoint(settings.IpAddress, settings.Port);
            _tcpListener = new TcpListener(endPoint);
            _receivedMessages = new List<IMailMessage>();
            _logger = logger != null && settings.EnableLogging ? logger : new NoopLogger();

            _clientHandlerFactory = new TcpClientHandlerFactory(settings.Certificate, _logger);
        }

        /// <inheritdoc />
        public void Start()
        {
            _tcpListener.Start();
            while (true)
            {
                var client = _tcpListener.AcceptTcpClient();
                var handler = _clientHandlerFactory.Create(client);
                var task = new Task<IMailMessage>(handler.Run);
                task.Start();
                _receivedMessages.Add(task.Result);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            _tcpListener.Stop();
        }

        /// <inheritdoc />
        public List<IMailMessage> GetReceivedMails()
        {
            return _receivedMessages;
        }
    }
}