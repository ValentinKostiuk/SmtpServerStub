using System.Net;

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
        public string SslCertificateFilePath { get; set; }
        public bool EnableLogging { get; set; }
        public string SslCertificatePassword { get; set; }
    }
}