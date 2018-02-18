using System;
using System.Configuration;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using SmtpServerStub.Dtos;
using SmtpServerStub.SmtpApplication;
using SmtpServerStub.SmtpApplication.Interfaces;
using MailMessage = System.Net.Mail.MailMessage;

namespace SmtpServerStubIntegrationTests
{
	public class SyncBaseTest
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

		public void SendMessage(MailMessage mailMessage, bool enableSsl = false)
		{
			using (var smtp = new SmtpClient
			{
				Host = "localhost",
				Port = 25,
				EnableSsl = enableSsl
			})
			{
				try
				{
					smtp.Send(mailMessage);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
	}
}
