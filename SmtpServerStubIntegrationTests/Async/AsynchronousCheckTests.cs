using System.Net.Mail;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace SmtpServerStubIntegrationTests.Async
{
	public class AsynchronousCheckTests : AsyncBaseTest
	{
		private readonly MailAddress _fromAddress = new MailAddress("valentin.kostiukFrom@gmail.com", "From Name");
		private readonly MailAddress _toAddress = new MailAddress("valentin.kostiuk@gmail.com", "To Name");
		private readonly MailAddress _toAddress2 = new MailAddress("valentin.kostiuk2@gmail.com");

		[Ignore("To takes long time.")]
		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailsCount(bool enableSsl)
		{
			var expectedCount = 3;
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			var messages = WaitMessagesReceived(() =>
			{
				for (var i = 0; i < expectedCount; i++)
				{
					SendMessageAsync(message, enableSsl);
					Thread.Sleep(20);
				}
			}, expectedCount, 10000);

			messages.Count.Should().Be(expectedCount);
		}


		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailBody(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 50000);

			receivedMail.Body.Should().Be(message.Body);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailBodyWithNewLines(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of your email\r\nSomeSome\r\n\r\n\r\n\r\n\r\n\r\n"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.Body.Should().Be(message.Body);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailFrom(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.From.Address.Should().Be(_fromAddress.Address);
			receivedMail.From.DisplayName.Should().Be(_fromAddress.DisplayName);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailSubject(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.Subject.Should().Be(message.Subject);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailСс(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);
			message.CC.Add(_toAddress);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.CC.Count.Should().Be(2);
			receivedMail.CC[0].Address.Should().Be(_toAddress2.Address);
			receivedMail.CC[0].DisplayName.Should().Be(_toAddress2.DisplayName);
			receivedMail.CC[1].Address.Should().Be(_toAddress.Address);
			receivedMail.CC[1].DisplayName.Should().Be(_toAddress.DisplayName);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailTo(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.To.Count.Should().Be(1);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailToAndCcAreExcludedFromToList(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.CC.Add(_toAddress2);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.To.Count.Should().Be(1);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailToList(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.To.Add(_toAddress2);

			var receivedMail = WaitOneMessageReceived(() => SendMessageAsync(message, enableSsl), 2500);

			receivedMail.To.Count.Should().Be(2);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
			receivedMail.To[1].Address.Should().Be(_toAddress2.Address);
			receivedMail.To[1].DisplayName.Should().Be(_toAddress2.DisplayName);
		}
	}
}