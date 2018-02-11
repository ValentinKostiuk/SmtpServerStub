using System.Collections.Generic;
using System.Threading;
using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
	/// <summary>
	/// Interface of <see cref="SmtpServer"/>
	/// </summary>
	public interface ISmtpServer
    {
        /// <summary>
        /// Event raised when server completely received new e-mail.
        /// Handler should implement <see cref="EmailReceivedEventHandler"/>
        /// </summary>
        event EmailReceivedEventHandler OnEmailReceived;

		/// <summary>
		/// Starts server in new Thread. Creates new TcpListener.
		/// </summary>
		/// <returns>System.Threading.Thread</returns>
		Thread Start();

        /// <summary>
        /// Blocks calling thread until Server finishes all started mail perceptions.
        /// Stops TcpListener created on start.
        /// Waits Server thread finishes.
        /// </summary>
        void Stop();

	    /// <summary>
	    /// Blocks calling thread until Server thread finishes.
	    /// </summary>
	    void Join();

		/// <summary>
		/// Returns collection of fully received e-mails on current moment.
		/// </summary>
		/// <returns><c>List</c>of<c>MailMessage</c></returns>
		List<IMailMessage> GetReceivedMails();

	    /// <summary>
	    /// Blocks calling thread until all started perceptions finish.
	    /// Clears all assigned event listeners.
	    /// Clears received mails collection.
	    /// </summary>
		void ResetState();
    }
}
