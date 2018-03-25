using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication;
using SmtpServerStub.SmtpApplication.Interfaces;
using MailMessage = System.Net.Mail.MailMessage;

namespace SmtpServerStubIntegrationTests.Async
{
	public class AsyncBaseTest
	{
		public ISmtpServer Server { get; set; }
		private Thread Thread { get; set; }

		[OneTimeSetUp]
		public void RunBeforeAllTest()
		{
			var logger = Substitute.For<ILogger>();
			var sslCertificateFilePath = ConfigurationManager.AppSettings["SslCertificateFilePath"];
			var sslCertificatePassword = ConfigurationManager.AppSettings["SslCertificatePassword"];
			var settings = new SmtpServerSettings();
			settings.Certificate = new X509Certificate2(sslCertificateFilePath, sslCertificatePassword, X509KeyStorageFlags.MachineKeySet);
			Server = new SmtpServer(settings);
			Server.Start();
			Console.WriteLine("Server Started");
		}

		[SetUp]
		public void RunBeforeAnyTest()
		{
			Server.ResetState();
			Console.WriteLine("Server has been reset");
		}

		[OneTimeTearDown]
		public void RunAfterAllTests()
		{
			Server.Stop();
			Console.WriteLine("Server Stopped");
		}

		public void SendMessageAsync(MailMessage mailMessage, bool enableSsl = false)
		{
			var smtp = new SmtpClient
			{
				Host = "localhost",
				Port = 25,
				EnableSsl = enableSsl
			};

			smtp.SendCompleted += (sender, args) => { smtp.Dispose(); };
			smtp.SendMailAsync(mailMessage);
			//smtp.SendAsync(mailMessage, null);
		}

		public IList<IMailMessage> WaitMessagesReceived(Action startSending, int expectedMailsNumber, int timeout)
		{
			var receivedCount = 0;
			var receivedMessages = new List<IMailMessage>();

			var manualEvents = Enumerable.Range(0, expectedMailsNumber).Select(x => new ManualResetEvent(false)).ToArray();

			Server.OnEmailReceived += (sender, args) =>
			{
				receivedMessages.Add(args.MailMessage);
				manualEvents[receivedCount].Set();
			};

			startSending();

			WaitHandle.WaitAll(manualEvents, timeout);

			return receivedMessages;
		}

		public IMailMessage WaitOneMessageReceived(Action startSending, int timeout)
		{
			var receivedMessageEvent = new ManualResetEvent(false);
			IMailMessage receivedMessage = null;

			Server.OnEmailReceived += (sender, args) =>
			{
				receivedMessage = args.MailMessage;
				receivedMessageEvent.Set();
			};

			startSending();

			receivedMessageEvent.WaitOne(timeout);

			return receivedMessage;
		}
	}
}