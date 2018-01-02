using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SmtpServerStub.Dtos
{
	/// <summary>
	/// </summary>
	public interface IMailMessage
    {
	    /// <summary>
	    /// <returns>Subject of received e-mail</returns>
	    /// </summary>
	    string Subject { get; set; }
		    
		/// <summary>
		/// Gets or sets the sender's address for this e-mail message.
		/// <returns>A <see cref="System.Net.Mail.MailAddress"/> that contains the sender's address information.</returns>
		/// </summary>
		MailAddress Sender { get; set; }
		
		/// <summary>
		/// Gets or sets the list of addresses to reply to for the mail message.
		/// <returns>The list of the addresses to reply to for the mail message.</returns>
		/// </summary>
		List<MailAddress> ReplyToList { get; set; }

		/// <summary>
		/// Gets or sets the priority of this e-mail message.
		/// <returns> A <see cref="System.Net.Mail.MailPriority"/> that contains the priority of this message.</returns>
		/// </summary>
		MailPriority Priority { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether the mail message body is in Html.
		/// <returns>true if the message body is in Html; else false. The default is false.</returns>
		/// </summary>
		bool IsBodyHtml { get; set; }
		
		/// <summary>
		/// Gets or sets the encoding used for the user-defined custom headers for this e-mail message.
		/// <returns>The encoding used for user-defined custom headers for this e-mail message.</returns>
		/// </summary>
		Encoding HeadersEncoding { get; set; }
		
		/// <summary>
		/// Gets the e-mail headers that are transmitted with this e-mail message.
		/// <returns>A <see cref="System.Collections.Specialized.NameValueCollection"/> that contains the e-mail headers.</returns>
		/// </summary>
		NameValueCollection Headers { get; set; }
		
		/// <summary>
		/// Gets or sets the from address for this e-mail message.
		/// <returns>A <see cref="System.Net.Mail.MailAddress"/> that contains the from address information.</returns>
		/// </summary>
		MailAddress From { get; set; }
		
		/// <summary>
		/// Gets the address collection that contains the carbon copy (CC) recipients for this e-mail message.
		/// <returns>A writable <see cref="System.Net.Mail.MailAddressCollection"/> object.</returns>
		/// </summary>
		List<MailAddress> CC { get; set; }
		
		/// <summary>
		/// Gets or sets the encoding used for the subject content for this e-mail message.
		/// <returns>An <see cref="System.Text.Encoding"/> that was used to encode the <see cref="System.Net.Mail.MailMessage.Subject"/> property.</returns>
		/// </summary>
		Encoding SubjectEncoding { get; set; }
		
		/// <summary>
		/// Gets or sets the transfer encoding used to encode the message body.
		/// <returns>Returns <see cref="System.Net.Mime.TransferEncoding"/>. A <see cref="System.Net.Mime.TransferEncoding"/> applied to the contents of the body.</returns>
		/// </summary>
		TransferEncoding BodyTransferEncoding { get; set; }
		
		/// <summary>
		/// Gets or sets the encoding used to encode the message body.
		/// <returns>An <see cref="System.Text.Encoding"/> applied to the contents of the body.</returns>
		/// </summary>
		Encoding BodyEncoding { get; set; }
		
		/// <summary>
		/// Gets or sets the message body.
		/// <returns>A <see cref="System.String"/> value that contains the body text.</returns>
		/// </summary>
		string Body { get; set; }
		
		/// <summary>
		/// Gets the address collection that contains the blind carbon copy (BCC) recipients for this e-mail message.
		/// <returns>A writable <see cref="System.Net.Mail.MailAddressCollection"/> object.</returns>
		/// </summary>
		List<MailAddress> Bcc { get; set; }

		//TODO: implement attachment collection
		//
		// Summary:
		//     Gets the attachment collection used to store data attached to this e-mail message.
		//
		// Returns:
		//     A writable System.Net.Mail.AttachmentCollection.
		//AttachmentCollection Attachments { get; }

		//TODO: implementAlternative views if required 
		//
		// Summary:
		//     Gets the attachment collection used to store alternate forms of the message body.
		//
		// Returns:
		//     A writable System.Net.Mail.AlternateViewCollection.
		//AlternateViewCollection AlternateViews { get; }
		
		/// <summary>
		/// Gets or sets the delivery notifications for this e-mail message.
		/// <returns>A <see cref="System.Net.Mail.DeliveryNotificationOptions"/> value that contains the delivery notifications for this message.</returns>
		/// </summary>
		DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }
		
		/// <summary>
		/// Gets the address collection that contains the recipients of this e-mail message.
		/// <returns>A writable <see cref="System.Net.Mail.MailAddressCollection"/> object.</returns>
		/// </summary>
		List<MailAddress> To { get; set; }
		
		/// <summary>
		/// Date header of received E-mail message
		/// <returns><see cref="DateTime"/></returns>
		/// </summary>
		DateTime DateSent { get; set; }

		/// <summary>
		/// Returns whole content of DATA SMTP protocol part (content of message including headers and attachments)
		/// <returns><see cref="String"/></returns>
		/// </summary>
		string MailMessageDataSection { get; set; }
	}
}