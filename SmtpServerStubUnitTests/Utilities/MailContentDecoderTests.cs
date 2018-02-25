using System;
using FluentAssertions;
using NUnit.Framework;
using SmtpServerStub.Utilities;
using SmtpServerStub.Utilities.Interfaces;

namespace SmtpServerStubUnitTests.Utilities
{
	public class MailContentDecoderTests
	{
		private IMailContentDecoder _contentDecoder;

		[SetUp]
		public void SetUp()
		{
			_contentDecoder = new MailContentDecoder();
		}

		[Test]
		public void DecodesQoutedPrintableContent()
		{
			//arrange
			var data = "=0D=0A=0D=0A=20SomeMessage";
			var contentType = "text/plain; charset=us-ascii";
			var contentTransferType = "quoted-printable";

			//act
			var result = _contentDecoder.DecodeContent(contentType, contentTransferType, data);

			//assert

			result.Should().Be("\r\n\r\n SomeMessage");
		}

		[Test]
		public void DecodesQoutedPrintableContentNonAscii()
		{
			//arrange
			var data = "=0D=0A=D0=9D=D0=B5=D0=BA=D0=BE=D1=82=D0=BE=D1=80=D1=8B=D0=B9=20=D1=82=D0=B5=D0=BA=D1=81=D1=82";
			var contentType = "text/plain; charset=utf-8";
			var contentTransferType = "quoted-printable";

			//act
			var result = _contentDecoder.DecodeContent(contentType, contentTransferType, data);

			//assert

			result.Should().Be("\r\nНекоторый текст");
		}

		[Test]
		public void DecodesBase64Content()
		{
			//arrange
			var data = "DQoNCiDQndC10LrQvtGC0L7RgNGL0Lkg0YLQtdC60YHRgi4gVW5leHBlY3RlZCBzeW1ib2xzIGhlcmUu";
			var contentType = "text/plain; charset=utf-8";
			var contentTransferType = "base64";

			//act
			var result = _contentDecoder.DecodeContent(contentType, contentTransferType, data);

			//assert

			result.Should().Be("\r\n\r\n Некоторый текст. Unexpected symbols here.");
		}

		[Test]
		public void ThrowsExceptionAboutInvalidArguments()
		{
			//arrange
			var data = "DQoNCiDQndC10LrQvtGC0L7RgNGL0Lkg0YLQtdC60YHRgi4gVW5leHBlY3RlZCBzeW1ib2xzIGhlcmUu";
			var contentType = "text/plain; charset=utf-8";
			var contentTransferType = "some not implemented transfer encoding";

			//act
			Action act = ()=>
			{
				_contentDecoder.DecodeContent(contentType, contentTransferType, data);
			};

			//assert

			act.ShouldThrow<ArgumentException>().WithMessage("Unrecognizable transfer encoding.");
		}
	}
}
