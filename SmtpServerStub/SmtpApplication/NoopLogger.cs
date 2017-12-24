using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication
{
    internal class NoopLogger: ILogger
    {
        public void LogInfo(string message){}

        public void LogError(string message){}

        public void LogWarning(string message){}
    }
}
