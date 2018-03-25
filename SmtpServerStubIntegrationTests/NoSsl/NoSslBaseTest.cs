using System;
using System.Net.Mail;
using NSubstitute;
using NUnit.Framework;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication;
using SmtpServerStub.SmtpApplication.Interfaces;
using MailMessage = System.Net.Mail.MailMessage;

namespace SmtpServerStubIntegrationTests.NoSsl
{
	public class NoSslBaseTest
	{
		public ISmtpServer Server { get; set; }
		public ILogger LoggerSubstitute { get; set; }

		[OneTimeSetUp]
		public void RunBeforeAllTest()
		{
			LoggerSubstitute = Substitute.For<ILogger>();
			var settings = new SmtpServerSettings();
			Server = new SmtpServer(settings, LoggerSubstitute);
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

		public void SendMessage(MailMessage mailMessage, bool enableSsl = false)
		{
			using (var smtp = new SmtpClient
			{
				Host = "localhost",
				Port = 25,
				EnableSsl = enableSsl
			})
			{
				smtp.Send(mailMessage);
			}
		}
	}
}