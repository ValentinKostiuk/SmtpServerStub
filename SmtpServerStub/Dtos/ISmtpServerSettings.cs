using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SmtpServerStub.Dtos
{
	/// <summary>
	/// Initial SMTP server settings
	/// </summary>
	public interface ISmtpServerSettings
    {
		/// <summary>
		/// IP address of SMTP server. Instance of <see cref="IPAddress"/>
		/// </summary>
		IPAddress IpAddress { get; set; }

	    /// <summary>
	    /// SMTP server port. Will be used for TLS and unsafe channels.
	    /// </summary>
	    int Port { get; set; }

		/// <summary>
		/// SSL certificate will be used to authorize client. <see cref="X509Certificate2"/>
		/// Usually created from *.pfx file. Should be added to allowed certificates on your machine.
		/// </summary>
		X509Certificate2 Certificate { get; set; }
    }
}