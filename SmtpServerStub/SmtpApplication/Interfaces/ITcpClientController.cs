namespace SmtpServerStub.SmtpApplication.Interfaces
{
    public interface ITcpClientController
    {
        void SwitchToSslProtocol();
        void Write(string message);
        string Read();
        void Close();
        string HostName { get; }
    }
}
