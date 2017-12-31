using System;
using System.Collections.Generic;
using System.Net.Mail;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using SmtpServerStub.Dtos;
using SmtpServerStub.Enums;
using SmtpServerStub.SmtpApplication;
using SmtpServerStub.SmtpApplication.Interfaces;
using SmtpServerStub.Utilities.Interfaces;

namespace SmtpServerStubUnitTests.SmtpApplication
{
    [TestFixture]
    public class SmtpServerClientProcessorTests
    {
        private ISmtpServerClientProcessor _clientProcessor;
        private ITcpClientController _clientController;
        private ILogger _logger;
        private IEmailParser _emailParser;
        private IRequestCommandsConverter _requestCommandsConverter;
        private IServerStatusCodesConverter _serverStatusCodesConverter;

        private string _hostName = "ServerHost";

	    private string _mailDataSection = "MIME-Version: 1.0\r\n" +
	                                     "From: \"From Name\" <valentin.kostiuk @gmail.com>\r\n" +
	                                     "To: \"To Name\" <valentin.kostiuk @gmail.com>\r\n" +
	                                     "Cc: \"To Name\" <valentin.kostiuk @gmail.com>, \"To Name\"<valentin.kostiuk @gmail.com>\r\n" +
	                                     "Date: 19 Dec 2017 17:36:49 +0200\r\n" +
	                                     "Subject: Subject\r\n" +
	                                     "Content-Type: text/plain; charset=us-ascii\r\n" +
	                                     "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
	                                     "Body of Message" +
	                                     "\r\n.\r\n";

		[SetUp]
        public void Setup()
        {
            _clientController = Substitute.For<ITcpClientController>();
            _logger = Substitute.For<ILogger>();
            _emailParser = Substitute.For<IEmailParser>();
            _requestCommandsConverter = Substitute.For<IRequestCommandsConverter>();
            _serverStatusCodesConverter = Substitute.For<IServerStatusCodesConverter>();

            _clientProcessor = new SmtpServerClientProcessor(_clientController, _logger);

            _clientProcessor.EmailParser = _emailParser;
            _clientProcessor.RequestCommandsConverter = _requestCommandsConverter;
            _clientProcessor.ServerStatusCodesConverter = _serverStatusCodesConverter;

            _clientController.HostName.Returns(_hostName);
        }

        [Test]
        public void Run_ShouldCorrectlyParseMailFrom()
        {
            //arrange
            var expectedAddress = new MailAddress("some@address.com", "display name");

            _clientController.Read().Returns("not empty string");
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.MailFrom, RequestCommands.Quit);
            _emailParser.ParseEmailFromRecipientCommand("not empty string").Returns(expectedAddress);

            //act
            var message = _clientProcessor.Run();

            //assert
            message.From.Should().Be(expectedAddress);
        }

        [Test]
        public void Run_ShouldRespondWithErrorIfFromIsIncorrect()
        {
            //arrange
            var expectedAddress = new MailAddress("some@address.com", "display name");

            _clientController.Read().Returns("not empty string");
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.MailFrom, RequestCommands.Quit);
            _emailParser.ParseEmailFromRecipientCommand("not empty string").ThrowsForAnyArgs(new Exception("very incorrect e-mail"));
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed).Returns("response with error");
            //act
            var message = _clientProcessor.Run();

            //assert
            message.From.Should().Be(null);
            _clientController.Received(1).Write("response with error");
        }

        [Test]
        public void Run_ShouldCorrectlyParseMailTo()
        {
            //arrange
            var expectedAddress = new MailAddress("some@address.com", "display name");
            var readLine = "not empty string";

            _clientController.Read().Returns(readLine);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.RcptTo, RequestCommands.Quit);
            _emailParser.ParseEmailFromRecipientCommand(readLine).Returns(expectedAddress);
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName).Returns("response2");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response3");

            //act
            var message = _clientProcessor.Run();

            //assert
            message.To[0].Should().Be(expectedAddress);
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted);
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName);
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName);
            _clientController.Received(1).Write("response1");
            _clientController.Received(1).Write("response2");
            _clientController.Received(1).Write("response3");
        }

        [Test]
        public void Run_ShouldCorrectlyRespondIfToNotParsed()
        {
            //arrange
            var readLine = "not empty string";

            _clientController.Read().Returns(readLine);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.RcptTo, RequestCommands.Quit);
            _emailParser.ParseEmailFromRecipientCommand(readLine).ThrowsForAnyArgs(new Exception("parse error message"));
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName).Returns("response2");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response3");

            //act
            var message = _clientProcessor.Run();

            //assert
            message.To.Count.Should().Be(0);
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed);
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName);
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName);
            _clientController.Received(1).Write("response1");
            _clientController.Received(1).Write("response2");
            _clientController.Received(1).Write("response3");
        }

        [Test]
        public void Run_ShouldSwitchToTlsAndRespond()
        {
            //arrange
            var readLine = "not empty string";

            _clientController.Read().Returns(readLine);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.StartTls, RequestCommands.Quit);
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response2");

            //act
            _clientProcessor.Run();

            //assert
            _clientController.Received(1).SwitchToTlsProtocol();
            _serverStatusCodesConverter.Received(1).GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName);
            _serverStatusCodesConverter.Received(2).GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName);
            _clientController.Received(1).Write("response1");
            _clientController.Received(2).Write("response2");
        }

        [Test]
        public void Run_SwitchToTlsLogsErrorAndQuit()
        {
            //arrange
            var readLine = "not empty string";

            _clientController.Read().Returns(readLine);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.StartTls, RequestCommands.Quit);
            _clientController.When(x => x.SwitchToTlsProtocol()).Do(x => throw new Exception("happens some times"));
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response2");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.AccessDenied).Returns("response3");

            //act
            _clientProcessor.Run();

            //assert
            _clientController.Received(1).SwitchToTlsProtocol();
            _logger.Received(1).LogError("Exception occurred while switching to TLS:\nhappens some times");
            _clientController.Received(2).Write("response2");
            _clientController.Received(1).Write("response1");
            _clientController.Received(1).Write("response3");
        }

        [Test]
        public void Run_RespondsWithCorrectHelloIfTlsIsAvailable()
        {
            //arrange
            var readLine = "not empty string";

            _clientController.Read().Returns(readLine);
            _clientController.IsTlsAvailable.Returns(true);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.Quit);
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHello, _hostName).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response2");

            //act
            _clientProcessor.Run();

            //assert
            _clientController.Received(1).Write("response1");
            _clientController.Received(1).Write("response2");
        }

		[Test]
        public void Run_SetsCcList()
        {
            //arrange
            var readLine = "not empty string";

	        var expectedCollection = new List<MailAddress> {new MailAddress("address@ololo.com", "Cc Name1"), new MailAddress("address2@ololo.com", "Cc Name2")};

			_clientController.Read().Returns(readLine, readLine, _mailDataSection);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.Data, RequestCommands.Quit);
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName).Returns("response2");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.StrtInputEndWith).Returns("response3");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted).Returns("response4");

	        _emailParser.ParseEmailsFromDataCc(_mailDataSection).Returns(expectedCollection);

			//act
			var message = _clientProcessor.Run();

            //assert
            _clientController.Received(1).Write("response1");
            _clientController.Received(1).Write("response2");
            _clientController.Received(1).Write("response3");
            _clientController.Received(2).Write("response4");
			_clientController.Received(5).Write(Arg.Any<string>());
			message.CC.ShouldAllBeEquivalentTo(expectedCollection);
        }

		[Test]
        public void Run_ReadsBodyTillEndReached()
        {
            //arrange
            var readLine = "not empty string";
	        var firstPart = "start of body";
	        var secondPart = "middleOfBody";
	        var endOfBody = "someText\r\n.\r\n";

			_clientController.Read().Returns(readLine, readLine, firstPart, secondPart, endOfBody);
            _requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.Data, RequestCommands.Quit);
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _hostName).Returns("response1");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHelloNoTls, _hostName).Returns("response2");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.StrtInputEndWith).Returns("response3");
            _serverStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted).Returns("response4");

			//act
			_clientProcessor.Run();

            //assert
	        _clientController.Received(5).Read();
        }
    }
}