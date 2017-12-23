namespace SmtpServerStub.Enums
{
    internal enum RequestCommands
	{
		Hello,
		MailFrom,
		RcptTo,
		Data,
		Rset,
		Vrfy,
		Noop,
		Auth,
		StartTls,
		Size,
		Help,
		Quit,
		ParseError
	}
}