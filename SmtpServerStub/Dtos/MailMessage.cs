using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SmtpServerStub.Dtos
{
	/// <inheritdoc />
	public class MailMessage : IMailMessage
	{
		/// <summary>
		/// Creates instance with empty collections 
		/// </summary>
		public MailMessage()
		{
			ReplyToList = new List<MailAddress>();
			Headers = new NameValueCollection();
			CC = new List<MailAddress>();
			Bcc = new List<MailAddress>();
			To = new List<MailAddress>();
		}

		/// <inheritdoc />
		public string Subject { get; set; }

		/// <inheritdoc />
		public MailAddress Sender { get; set; }

		/// <inheritdoc />
		public List<MailAddress> ReplyToList { get; set; }

		/// <inheritdoc />
		public MailPriority Priority { get; set; }

		/// <inheritdoc />
		public bool IsBodyHtml { get; set; }

		/// <inheritdoc />
		public Encoding HeadersEncoding { get; set; }

		/// <inheritdoc />
		public NameValueCollection Headers { get; set; }

		/// <inheritdoc />
		public MailAddress From { get; set; }

		/// <inheritdoc />
		public List<MailAddress> CC { get; set; }

		/// <inheritdoc />
		public Encoding SubjectEncoding { get; set; }

		/// <inheritdoc />
		public TransferEncoding BodyTransferEncoding { get; set; }

		/// <inheritdoc />
		public Encoding BodyEncoding { get; set; }

		/// <inheritdoc />
		public string Body { get; set; }

		/// <inheritdoc />
		public List<MailAddress> Bcc { get; set; }

		/// <inheritdoc />
		public DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }

		/// <inheritdoc />
		public List<MailAddress> To { get; set; }

		/// <inheritdoc />
		public DateTime DateSent { get; set; }

		/// <inheritdoc />
		public string MailMessageDataSection { get; set; }
	}
}