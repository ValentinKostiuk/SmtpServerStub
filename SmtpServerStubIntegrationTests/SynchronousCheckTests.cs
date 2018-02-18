using System.Net.Mail;
using FluentAssertions;
using NUnit.Framework;

namespace SmtpServerStubIntegrationTests
{
	[TestFixture]
	public class SynchronousCheckTests: SyncBaseTest
	{
		[Test]
		public void ContainsCorrectEmailsCount()
		{
			var fromAddress = new MailAddress("valentin.kostiuk@gmail.com", "From Name");
			var toAddress = new MailAddress("valentin.kostiuk@gmail.com", "To Name");
			var toAddress2 = new MailAddress("valentin.kostiuk@gmail.com");
			var message = new MailMessage(fromAddress, toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of your email"
			};

			message.To.Add(toAddress);
			message.CC.Add(toAddress2);
			
			SendMessage(message);

			Server.GetReceivedMails().Count.Should().Be(1);
		}
	}
}
