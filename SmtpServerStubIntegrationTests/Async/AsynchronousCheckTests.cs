using System.Net.Mail;
using FluentAssertions;
using NUnit.Framework;

namespace SmtpServerStubIntegrationTests.Async
{
	public class AsynchronousCheckTests : AsyncBaseTest
	{
		private readonly MailAddress _fromAddress = new MailAddress("valentin.kostiukFrom@gmail.com", "From Name");
		private readonly MailAddress _toAddress = new MailAddress("valentin.kostiuk@gmail.com", "To Name");
		private readonly MailAddress _toAddress2 = new MailAddress("valentin.kostiuk2@gmail.com");

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailsCount(bool enableSsl)
		{
			var expectedCount = 7;
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			var messages = WaitMailMessagesReceived(() =>
			{
				for (var i = 0; i < expectedCount; i++)
				{
					SendMessageAsync(message, enableSsl);
				}
			}, expectedCount, 2500);

			messages.Count.Should().Be(expectedCount);
		}
	}
}