using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SmtpServerStub.Dtos
{
    public class SmtpServerSettings : ISmtpServerSettings
    {
        public SmtpServerSettings()
        {
            IpAddress = IPAddress.Any;
            Port = 25;
            EnableLogging = false;
        }

        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public X509Certificate Certificate { get; set; }
        public bool EnableLogging { get; set; }
    }
}