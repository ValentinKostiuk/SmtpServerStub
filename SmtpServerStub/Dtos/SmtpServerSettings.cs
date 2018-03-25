using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SmtpServerStub.Dtos
{
	/// <inheritdoc />
	public class SmtpServerSettings : ISmtpServerSettings
    {
	    /// <summary>
	    /// Creates settings with default parameters.
	    /// </summary>
	    public SmtpServerSettings()
        {
            IpAddress = IPAddress.Any;
            Port = 25;
        }

	    /// <inheritdoc />
	    public IPAddress IpAddress { get; set; }

	    /// <inheritdoc />
	    public int Port { get; set; }

	    /// <inheritdoc />
	    public X509Certificate2 Certificate { get; set; }
    }
}