using System.Collections.Specialized;
using FluentAssertions;
using NUnit.Framework;
using SmtpServerStub.Utilities;
using SmtpServerStub.Utilities.Interfaces;

namespace SmtpServerStubUnitTests.Utilities
{
	public class EmailParserTests
	{
		private IEmailParser _emailParser;

		[SetUp]
		public void SetUp()
		{
			_emailParser = new EmailParser();
		}

		[Test]
		public void ParseBodyFromDataSection_ShouldReturnCorrectBodyFromDataSection()
		{
			//arrange
			var dataSection = "\r\nMIME - Version: 1.0\r\n" +
			                  "From: \"From Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "To: \"To Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "Cc: \"To Name\" <valentin.kostiuk@gmail.com>, \"To Name\"<valentin.kostiuk@gmail.com>\r\n" +
			                  "Date: 19 Dec 2017 17:36:49 +0200\r\n" +
			                  "Subject: Subject\r\n" +
			                  "Content-Type: text/plain; charset=us-ascii\r\n" +
			                  "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
			                  "Body of Message" +
			                  "\r\n.\r\n";
			
			//act
			var result = _emailParser.ParseBodyFromDataSection(dataSection);

			//assert
			result.Should().Be("Body of Message");
		}

		[Test]
		public void ParseBodyFromDataSection_ShouldReturnCorrectBodyFromDataSectionWithEmptyBody()
		{
			//arrange
			var dataSection = "\r\nMIME - Version: 1.0\r\n" +
			                  "From: \"From Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "To: \"To Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "Cc: \"To Name\" <valentin.kostiuk@gmail.com>, \"To Name\"<valentin.kostiuk@gmail.com>\r\n" +
			                  "Date: 19 Dec 2017 17:36:49 +0200\r\n" +
			                  "Subject: Subject\r\n" +
			                  "Content-Type: text/plain; charset=us-ascii\r\n" +
			                  "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
			                  "" +
			                  "\r\n.\r\n";
			
			//act
			var result = _emailParser.ParseBodyFromDataSection(dataSection);

			//assert
			result.Should().Be("");
		}

		[Test]
		public void ParseBodyFromDataSection_ShouldReturnCorrectBodyFromDataSectionWithMoreNewLines()
		{
			//arrange
			var dataSection = "\r\nMIME - Version: 1.0\r\n" +
			                  "From: \"From Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "To: \"To Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "Cc: \"To Name\" <valentin.kostiuk@gmail.com>, \"To Name\"<valentin.kostiuk@gmail.com>\r\n" +
			                  "Date: 19 Dec 2017 17:36:49 +0200\r\n" +
			                  "Subject: Subject\r\n" +
			                  "Content-Type: text/plain; charset=us-ascii\r\n" +
			                  "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
							  "\r\n.\r\n\r\n.\r\n" +
			                  "\r\n.\r\n";
			
			//act
			var result = _emailParser.ParseBodyFromDataSection(dataSection);

			//assert
			result.Should().Be("\r\n.\r\n\r\n.\r\n");
		}

		[Test]
		public void ParseSubjectFromDataSection_ShouldParseSubjectFromDataSection()
		{
			//arrange
			var headers = new NameValueCollection();
			headers.Add("Subject", "Subject of this letter");
			
			//act
			var result = _emailParser.ParseSubjectFromDataSection(headers);

			//assert
			result.Should().Be("Subject of this letter");
		}

		[Test]
		public void ParseSubjectFromDataSection_ShouldReturnNull()
		{
			//arrange
			var headers = new NameValueCollection();
			
			//act
			var result = _emailParser.ParseSubjectFromDataSection(headers);

			//assert
			result.Should().Be(null);
		}

		[Test]
		public void ParseHeadersFromDataSection_ShouldParseHeadersOfEmail()
		{
			//arrange
			var dataSection = "\r\nMIME - Version: 1.0\r\n" +
			                  "From: \"From Name\" <valentin.kostiuk@gmail.com>\r\n" +
			                  "To: \"To Name\" <valentin.kostiuk@gmail.com>\r\n" +
							  "Cc: \"To Name\" <valentin.kostiuk@gmail.com>,\r\n\"To Name\"<valentin.kostiuk@gmail.com>, \"To \r\nName\"<valentin.kostiuk@gmail.com>\r\n" +
			                  "Date: 19 Dec 2017 17:36:49 +0200\r\n" +
			                  "Subject: Subject of this letter\r\n" +
			                  "Content-Type: text/plain; charset=us-ascii\r\n" +
			                  "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
							  "\r\n.\r\n\r\n.\r\n" +
			                  "\r\n.\r\n";
			
			//act
			var result = _emailParser.ParseHeadersFromDataSection(dataSection);

			//assert
			result.Count.Should().Be(8);
			result["MIME - Version"].Should().Be("1.0");
			result["From"].Should().Be("\"From Name\" <valentin.kostiuk@gmail.com>");
			result["To"].Should().Be("\"To Name\" <valentin.kostiuk@gmail.com>");
			result["Cc"].Should().Be("\"To Name\" <valentin.kostiuk@gmail.com>,\r\n\"To Name\"<valentin.kostiuk@gmail.com>, \"To \r\nName\"<valentin.kostiuk@gmail.com>");
			result["Date"].Should().Be("19 Dec 2017 17:36:49 +0200");
			result["Subject"].Should().Be("Subject of this letter");
			result["Content-Type"].Should().Be("text/plain; charset=us-ascii");
			result["Content-Transfer-Encoding"].Should().Be("quoted-printable");
		}
	}
}