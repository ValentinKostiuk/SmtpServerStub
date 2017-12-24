using System.Collections.Generic;
using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication.Interfaces
{
    /// <summary>
    /// Main class which allows stub SMTP server to check sent emails
    /// </summary>
    public interface ISmtpServer
    {
        /// <summary>
        /// Starts server. Creates new TcpListener.
        /// Usually should be started in separate Thread:
        /// Thread = new Thread(Server.Start);
        /// Thread.Start();
        /// </summary>
        void Start();

        /// <summary>
        /// Stops TcpListener created on start.
        /// Should be called before aborting server thread.
        /// Because that will not any take affect, and server will continue running.  
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns collection of fully received e-mails on current moment
        /// </summary>
        /// <returns><c>List</c>of<c>MailMessage</c></returns>
        List<IMailMessage> GetReceivedMails();
    }
}
