namespace SmtpServerStub.Enums
{
	public enum RequestCommands
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