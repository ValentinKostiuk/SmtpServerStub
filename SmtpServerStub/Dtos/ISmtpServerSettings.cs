using System.Net;

namespace SmtpServerStub.Dtos
{
    public interface ISmtpServerSettings
    {
        IPAddress IpAddress { get; set; }
        int Port { get; set; }
        string SslCertificateFilePath { get; set; }
        string SslCertificatePassword { get; set; }
        bool EnableLogging { get; set; }
    }
}