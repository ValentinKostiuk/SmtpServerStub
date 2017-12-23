using SmtpServerStub.Enums;

namespace SmtpServerStub.Utilities
{
    internal static class RequestCommandsConverter
    {
		public static RequestCommands ToRequestCommandCode(string commandText)
		{
			if (commandText.StartsWith("EHLO") || commandText.StartsWith("HELO"))
			{
				return RequestCommands.Hello;
			}
			if (commandText.StartsWith("MAIL FROM"))
			{
				return RequestCommands.MailFrom;
			}
			if (commandText.StartsWith("RCPT TO"))
			{
				return RequestCommands.RcptTo;
			}
			if (commandText.StartsWith("DATA"))
			{
				return RequestCommands.Data;
			}
			if (commandText.StartsWith("RSET"))
			{
				return RequestCommands.Rset;
			}
			if (commandText.StartsWith("VRFY"))
			{
				return RequestCommands.Vrfy;
			}
			if (commandText.StartsWith("NOOP"))
			{
				return RequestCommands.Noop;
			}
			if (commandText.StartsWith("AUTH"))
			{
				return RequestCommands.Auth;
			}
			if (commandText.StartsWith("STARTTLS"))
			{
				return RequestCommands.StartTls;
			}
			if (commandText.StartsWith("SIZE"))
			{
				return RequestCommands.Size;
			}
			if (commandText.StartsWith("HELP"))
			{
				return RequestCommands.Help;
			}
			if (commandText.StartsWith("QUIT"))
			{
				return RequestCommands.Quit;
			}
			return RequestCommands.ParseError;
		}
	}
}