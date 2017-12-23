using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SmtpServerStub.Dtos
{
    public interface IMailMessage
    {
        string Subject { get; set; }

        //
        // Summary:
        //     Gets or sets the sender's address for this e-mail message.
        //
        // Returns:
        //     A System.Net.Mail.MailAddress that contains the sender's address information.
        MailAddress Sender { get; set; }

        //
        // Summary:
        //     Gets or sets the list of addresses to reply to for the mail message.
        //
        // Returns:
        //     The list of the addresses to reply to for the mail message.
        MailAddressCollection ReplyToList { get; }

        //
        // Summary:
        //     Gets or sets the priority of this e-mail message.
        //
        // Returns:
        //     A System.Net.Mail.MailPriority that contains the priority of this message.
        MailPriority Priority { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether the mail message body is in Html.
        //
        // Returns:
        //     true if the message body is in Html; else false. The default is false.
        bool IsBodyHtml { get; set; }

        //
        // Summary:
        //     Gets or sets the encoding used for the user-defined custom headers for this e-mail
        //     message.
        //
        // Returns:
        //     The encoding used for user-defined custom headers for this e-mail message.
        Encoding HeadersEncoding { get; set; }

        //
        // Summary:
        //     Gets the e-mail headers that are transmitted with this e-mail message.
        //
        // Returns:
        //     A System.Collections.Specialized.NameValueCollection that contains the e-mail
        //     headers.
        NameValueCollection Headers { get; }

        //
        // Summary:
        //     Gets or sets the from address for this e-mail message.
        //
        // Returns:
        //     A System.Net.Mail.MailAddress that contains the from address information.
        MailAddress From { get; set; }

        //
        // Summary:
        //     Gets the address collection that contains the carbon copy (CC) recipients for
        //     this e-mail message.
        //
        // Returns:
        //     A writable System.Net.Mail.MailAddressCollection object.
        MailAddressCollection CC { get; }

        //
        // Summary:
        //     Gets or sets the encoding used for the subject content for this e-mail message.
        //
        // Returns:
        //     An System.Text.Encoding that was used to encode the System.Net.Mail.MailMessage.Subject
        //     property.
        Encoding SubjectEncoding { get; set; }

        //
        // Summary:
        //     Gets or sets the transfer encoding used to encode the message body.
        //
        // Returns:
        //     Returns System.Net.Mime.TransferEncoding. A System.Net.Mime.TransferEncoding
        //     applied to the contents of the System.Net.Mail.MailMessage.Body.
        TransferEncoding BodyTransferEncoding { get; set; }

        //
        // Summary:
        //     Gets or sets the encoding used to encode the message body.
        //
        // Returns:
        //     An System.Text.Encoding applied to the contents of the System.Net.Mail.MailMessage.Body.
        Encoding BodyEncoding { get; set; }

        //
        // Summary:
        //     Gets or sets the message body.
        //
        // Returns:
        //     A System.String value that contains the body text.
        string Body { get; set; }

        //
        // Summary:
        //     Gets the address collection that contains the blind carbon copy (BCC) recipients
        //     for this e-mail message.
        //
        // Returns:
        //     A writable System.Net.Mail.MailAddressCollection object.
        MailAddressCollection Bcc { get; }

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

        //
        // Summary:
        //     Gets or sets the delivery notifications for this e-mail message.
        //
        // Returns:
        //     A System.Net.Mail.DeliveryNotificationOptions value that contains the delivery
        //     notifications for this message.
        DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }

        //
        // Summary:
        //     Gets the address collection that contains the recipients of this e-mail message.
        //
        // Returns:
        //     A writable System.Net.Mail.MailAddressCollection object.
        MailAddressCollection To { get; }
        
        //
        // Summary:
        //     Date header of received E-mail message
        //
        // Returns:
        //     DateTime
        DateTime DateSent { get; set; }
    }
}