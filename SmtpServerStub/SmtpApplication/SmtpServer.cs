using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
    /// <summary> 
    /// <param name="sender">Is current <see cref="ISmtpServer"/></param>
    /// <param name="e">Event argument of type <see cref="EmailReceivedEventArgs"/></param>
    /// </summary>
    public delegate void EmailReceivedEventHandler(ISmtpServer sender, EmailReceivedEventArgs e);

    /// <inheritdoc />
    public class SmtpServer : ISmtpServer
    {
        private readonly TcpListener _tcpListener;
        private readonly ISmtpServerClientHandlerFactory _clientHandlerFactory;
        private readonly List<IMailMessage> _receivedMessages;
        private readonly ILogger _logger;

        /// <inheritdoc />
        public event EmailReceivedEventHandler OnEmailReceived;

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
                var task = new Task<MailMessage>(handler.Run);

	            task.ContinueWith(t =>
	            {
		            _receivedMessages.Add(t.Result);
		            OnEmailReceived?.Invoke(this, new EmailReceivedEventArgs
		            {
			            MailMessage = t.Result
		            });
	            });

                task.Start();
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