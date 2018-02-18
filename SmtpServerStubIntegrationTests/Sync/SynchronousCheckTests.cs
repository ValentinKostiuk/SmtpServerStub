using System.Net.Mail;
using FluentAssertions;
using NUnit.Framework;

namespace SmtpServerStubIntegrationTests.Sync
{
	[TestFixture]
	public class SynchronousCheckTests: SyncBaseTest
	{
		readonly MailAddress _fromAddress = new MailAddress("valentin.kostiukFrom@gmail.com", "From Name");
		readonly MailAddress _toAddress = new MailAddress("valentin.kostiuk@gmail.com", "To Name");
		readonly MailAddress _toAddress2 = new MailAddress("valentin.kostiuk2@gmail.com");

		[TestCase(true)]
		[TestCase(false)]
		public void ContainsCorrectEmailsCount(bool enableSsl)
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);
			
			SendMessage(message, enableSsl);
			SendMessage(message, enableSsl);
			SendMessage(message, enableSsl);
			SendMessage(message, enableSsl);
			SendMessage(message, enableSsl);
			SendMessage(message, enableSsl);
			SendMessage(message, enableSsl);

			Server.GetReceivedMails().Count.Should().Be(7);
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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

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

			SendMessage(message, enableSsl);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.To.Count.Should().Be(1);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
		}
	}
}
