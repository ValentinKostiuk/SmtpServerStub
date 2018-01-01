using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SmtpServerStub.Dtos
{
    public interface ISmtpServerSettings
    {
        IPAddress IpAddress { get; set; }
        int Port { get; set; }
        X509Certificate2 Certificate { get; set; }
        bool EnableLogging { get; set; }
    }
}