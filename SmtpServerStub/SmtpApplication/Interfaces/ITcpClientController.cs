namespace SmtpServerStub.SmtpApplication.Interfaces
{
	internal interface ITcpClientController
	{
		void SwitchToTlsProtocol();
		bool IsTlsAvailable { get; }
		void Write(string message);
		string Read();
		void Close();
		string HostName { get; }
	}
}