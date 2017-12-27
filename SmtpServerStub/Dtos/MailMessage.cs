using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SmtpServerStub.Dtos
{
	public class MailMessage : IMailMessage
	{
		public MailMessage()
		{
			ReplyToList = new List<MailAddress>();
			Headers = new NameValueCollection();
			CC = new List<MailAddress>();
			Bcc = new List<MailAddress>();
			To = new List<MailAddress>();
		}

		public string Subject { get; set; }
		public MailAddress Sender { get; set; }
		public List<MailAddress> ReplyToList { get; set; }
		public MailPriority Priority { get; set; }
		public bool IsBodyHtml { get; set; }
		public Encoding HeadersEncoding { get; set; }
		public NameValueCollection Headers { get; set; }
		public MailAddress From { get; set; }
		public List<MailAddress> CC { get; set; }
		public Encoding SubjectEncoding { get; set; }
		public TransferEncoding BodyTransferEncoding { get; set; }
		public Encoding BodyEncoding { get; set; }
		public string Body { get; set; }
		public List<MailAddress> Bcc { get; set; }
		public DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }
		public List<MailAddress> To { get; set; }
		public DateTime DateSent { get; set; }
		public string MailMessageDataSection { get; set; }
	}
}