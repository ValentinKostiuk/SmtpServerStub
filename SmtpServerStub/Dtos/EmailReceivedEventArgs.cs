using System;
using SmtpServerStub.SmtpApplication.Interfaces;

namespace SmtpServerStub.Dtos
{
    /// <summary>
    /// Used as argument in OnEmailReceived event of <see cref="ISmtpServer"/>
    /// </summary>
    public class EmailReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains received email object
        /// <see cref="MailMessage"/>
        /// </summary>
        public MailMessage MailMessage;
    }
}
