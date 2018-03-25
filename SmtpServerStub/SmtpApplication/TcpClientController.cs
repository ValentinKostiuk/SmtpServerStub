using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SmtpServerStub.Dtos;
using SmtpServerStub.Exceptions;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.SmtpApplication
{
	internal class TcpClientController : ITcpClientController
	{
		private readonly TcpClient _client;
		private readonly SslStream _sslStream;
		private readonly X509Certificate2 _certificate;
		private readonly NetworkStream _networkStream;
		private bool _switchedToSsl;
		public ILogger Logger { get; set; }

		public TcpClientController(TcpClient client, X509Certificate2 certificate, ILogger logger)
		{
			Logger = logger;
			_client = client;
			_certificate = certificate;
			_networkStream = _client.GetStream();
			_sslStream = new SslStream(_networkStream, false);
		}

		public void SwitchToTlsProtocol()
		{
			try
			{
				_sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, true);
				_switchedToSsl = true;
			}
			catch (AuthenticationException)
			{
				Close();
				throw;
			}
		}

		public bool IsTlsAvailable => _certificate != null;

		public void Write(string message)
		{
			var stream = _switchedToSsl ? _sslStream : (Stream) _networkStream;
			var encoder = new UTF8Encoding();
			var buffer = encoder.GetBytes(message + "\r\n");

			stream.Write(buffer, 0, buffer.Length);
			stream.Flush();
		}

		public string Read()
		{
			if (HasClientDisconnected())
			{
				throw new ClientControllerException("Client has disconnected unexpectedly.");
			}
			var stream = _switchedToSsl ? _sslStream : (Stream) _networkStream;
			var buffer = new byte[2048];
			var messageData = new StringBuilder();
			var bytes = -1;
			do
			{
				bytes = stream.Read(buffer, 0, buffer.Length);
				var decoder = Encoding.UTF8.GetDecoder();
				var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
				decoder.GetChars(buffer, 0, bytes, chars, 0);
				messageData.Append(chars);
			} while (_networkStream.DataAvailable);

			var result = messageData.ToString();
			return result;
		}

		public void Close()
		{
			try
			{
				_networkStream.Flush();
				_sslStream.Flush();
				_client.Close();
			}
			finally
			{
				_client.Dispose();
			}
		}

		public string HostName
		{
			get
			{
				var endPoint = (IPEndPoint) _client.Client.LocalEndPoint;
				var ipAddress = endPoint.Address;
				var hostEntry = Dns.GetHostEntry(ipAddress);
				return hostEntry.HostName;
			}
		}

		private bool HasClientDisconnected()
		{
			if (_client.Client.Poll(0, SelectMode.SelectRead))
			{
				var buff = new byte[1];
				if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}