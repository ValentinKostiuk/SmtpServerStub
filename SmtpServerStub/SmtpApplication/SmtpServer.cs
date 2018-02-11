using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
	/// <summary>
	///     <param name="sender">Is current <see cref="ISmtpServer" /></param>
	///     <param name="e">Event argument of type <see cref="EmailReceivedEventArgs" /></param>
	/// </summary>
	public delegate void EmailReceivedEventHandler(ISmtpServer sender, EmailReceivedEventArgs e);

	/// <summary>
	///     Main class which allows stub SMTP server to check sent emails
	/// </summary>
	public class SmtpServer : ISmtpServer
	{
		private readonly TcpListener _tcpListener;
		private readonly ISmtpServerClientHandlerFactory _clientHandlerFactory;
		private readonly ILogger _logger;
		private readonly IList<Task> _receiveEmailTasks;

		private ConcurrentBag<IMailMessage> _receivedMessages;
		private bool _stopped;
		private Thread _serverThread;

		/// <inheritdoc />
		public event EmailReceivedEventHandler OnEmailReceived;

		/// <inheritdoc />
		public SmtpServer(ISmtpServerSettings settings, ILogger logger = null)
		{
			var endPoint = new IPEndPoint(settings.IpAddress, settings.Port);
			_tcpListener = new TcpListener(endPoint);
			_receivedMessages = new ConcurrentBag<IMailMessage>();
			_logger = logger != null && settings.EnableLogging ? logger : new NoopLogger();
			_clientHandlerFactory = new TcpClientHandlerFactory(settings.Certificate, _logger);
			_receiveEmailTasks = new List<Task>();
		}

		/// <inheritdoc />
		public Thread Start()
		{
			_serverThread = new Thread(StartServer);
			_serverThread.Start();
			return _serverThread;
		}

		/// <inheritdoc />
		public void Join()
		{
			_serverThread.Join();
		}

		/// <inheritdoc />
		public void Stop()
		{
			_stopped = true;
			Task.WaitAll(_receiveEmailTasks.ToArray());
			_tcpListener.Stop();
			_serverThread.Join();
		}

		/// <inheritdoc />
		public List<IMailMessage> GetReceivedMails()
		{
			Task.WaitAll(_receiveEmailTasks.ToArray());
			return _receivedMessages.ToList();
		}

		/// <inheritdoc />
		public void ResetState()
		{
			lock (_receiveEmailTasks)
			{
				Task.WaitAll(_receiveEmailTasks.ToArray());
				foreach (var receiveEmailTask in _receiveEmailTasks)
				{
					receiveEmailTask.Dispose();
				}
				_receiveEmailTasks.Clear();
				_receivedMessages = new ConcurrentBag<IMailMessage>();
				OnEmailReceived = null;
			}
		}

		private void StartServer()
		{
			_tcpListener.Start();

			while (!_stopped)
			{
				try
				{
					var client = _tcpListener.AcceptTcpClient();
					var task = CreateReceiveMailTask(client);
					lock (_receiveEmailTasks)
					{
						_receiveEmailTasks.Add(task);
					}
				}
				catch (SocketException e) when (e.SocketErrorCode == SocketError.Interrupted && _stopped)
				{
					_logger.LogInfo("Server socket stopped by request");
				}
			}
		}

		private Task CreateReceiveMailTask(TcpClient client)
		{
			var handler = _clientHandlerFactory.Create(client);
			var task = new Task<MailMessage>(handler.Run);

			var handleMailTask = task.ContinueWith(t =>
			{
				_receivedMessages.Add(t.Result);
				OnEmailReceived?.Invoke(this, new EmailReceivedEventArgs
				{
					MailMessage = t.Result
				});
			});
			task.Start();
			return handleMailTask;
		}
	}
}