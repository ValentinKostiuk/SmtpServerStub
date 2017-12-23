using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SmtpServerStub.Dtos
{
    public class MailMessage: IMailMessage
    {
        public MailMessage()
        {
            ReplyToList = new MailAddressCollection();
            Headers = new NameValueCollection();
            CC = new MailAddressCollection();
            Bcc = new MailAddressCollection();
            To = new MailAddressCollection();
        }

        public string Subject { get; set; }
        public MailAddress Sender { get; set; }
        public MailAddressCollection ReplyToList { get; }
        public MailPriority Priority { get; set; }
        public bool IsBodyHtml { get; set; }
        public Encoding HeadersEncoding { get; set; }
        public NameValueCollection Headers { get; }
        public MailAddress From { get; set; }
        public MailAddressCollection CC { get; }
        public Encoding SubjectEncoding { get; set; }
        public TransferEncoding BodyTransferEncoding { get; set; }
        public Encoding BodyEncoding { get; set; }
        public string Body { get; set; }
        public MailAddressCollection Bcc { get; }
        public DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }
        public MailAddressCollection To { get; }
        public DateTime DateSent { get; set; }
    }
}
