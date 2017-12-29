using FluentAssertions;
using NSubstitute;
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
		}

		[Test]
		public void Run_ShouldCorrectlyParseMailFrom()
		{
			_clientController.Read().Returns("not empty string");
			_requestCommandsConverter.ToRequestCommandCode(Arg.Any<string>()).Returns(RequestCommands.Hello, RequestCommands.MailFrom, RequestCommands.Quit);

			var message = _clientProcessor.Run();

			message.From.Should().Be("ololo");
		}
	}
}
