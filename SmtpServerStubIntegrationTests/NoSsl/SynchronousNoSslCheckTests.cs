using System;
using System.Net.Mail;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace SmtpServerStubIntegrationTests.NoSsl
{
	[TestFixture]
	public class SynchronousNoSslCheckTests : NoSslBaseTest
	{
		private readonly MailAddress _fromAddress = new MailAddress("valentin.kostiukFrom@gmail.com", "From Name");
		private readonly MailAddress _toAddress = new MailAddress("valentin.kostiuk@gmail.com", "To Name");
		private readonly MailAddress _toAddress2 = new MailAddress("valentin.kostiuk2@gmail.com");

		public void ContainsCorrectEmailsCount()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			for (var i = 0; i < 7; i++)
			{
				SendMessage(message);
			}

			Server.GetReceivedMails().Count.Should().Be(7);
		}

		[Test]
		public void ContainsCorrectEmailBodyNonAscii()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever. Некоторые, не ASCII символы. Ну, просто что бы что-то проверить. Ё!!!"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.Body.Should().Be(message.Body);
		}

		[Test]
		public void ContainsCorrectEmailBodyWithNewLines()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of your email\r\nSomeSome\r\n\r\n\r\n\r\n\r\n\r\n"
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.Body.Should().Be(message.Body);
		}

		[Test]
		public void ContainsCorrectEmailFrom()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.From.Address.Should().Be(_fromAddress.Address);
			receivedMail.From.DisplayName.Should().Be(_fromAddress.DisplayName);
		}

		[Test]
		public void ContainsCorrectEmailSubject()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.Subject.Should().Be(message.Subject);
		}

		[Test]
		public void ContainsCorrectEmailСс()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);
			message.CC.Add(_toAddress);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.CC.Count.Should().Be(2);
			receivedMail.CC[0].Address.Should().Be(_toAddress2.Address);
			receivedMail.CC[0].DisplayName.Should().Be(_toAddress2.DisplayName);
			receivedMail.CC[1].Address.Should().Be(_toAddress.Address);
			receivedMail.CC[1].DisplayName.Should().Be(_toAddress.DisplayName);
		}

		[Test]
		public void ContainsCorrectEmailTo()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.To.Count.Should().Be(1);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
		}

		[Test]
		public void ContainsCorrectEmailToAndCcAreExcludedFromToList()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.CC.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.To.Count.Should().Be(1);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
		}

		[Test]
		public void ContainsCorrectEmailToList()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.To.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.To.Count.Should().Be(2);
			receivedMail.To[0].Address.Should().Be(_toAddress.Address);
			receivedMail.To[0].DisplayName.Should().Be(_toAddress.DisplayName);
			receivedMail.To[1].Address.Should().Be(_toAddress2.Address);
			receivedMail.To[1].DisplayName.Should().Be(_toAddress2.DisplayName);
		}

		[Test]
		public void SetsIsBodyHtml()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
				       "\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n " +
				       "<head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n  " +
				       "<title>Типа хтмл</title>\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>\r\n</head>\r\n" +
				       "<body style=\"margin: 0; padding: 0;\">\r\n " +
				       "<table border=\"1\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n  " +
				       "<tr>\r\n   <td>\r\n    Hello!\r\n   </td>\r\n  </tr>\r\n " +
				       "</table>\r\n" +
				       "</body>\r\n" +
				       "</html>",
				IsBodyHtml = true
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			SendMessage(message);

			var receivedMail = Server.GetReceivedMails()[0];

			receivedMail.Body.Should().Be(message.Body);
			receivedMail.IsBodyHtml.Should().BeTrue();
		}

		[Test]
		public void ShouldLogExceptionIfSslRequiredButCertificateHasNotBeenSet()
		{
			var message = new MailMessage(_fromAddress, _toAddress)
			{
				Subject = "Subject of email",
				Body = "Body of the best email ever."
			};

			message.To.Add(_toAddress);
			message.CC.Add(_toAddress2);

			Action send = () => SendMessage(message, true);

			send.ShouldThrow<SmtpException>().WithMessage("Server does not support secure connections.");
			LoggerSubstitute.Received(1).LogWarning("Stream was closed before QUIT command from server:\nClient has disconnected unexpectedly.");
		}
	}
}