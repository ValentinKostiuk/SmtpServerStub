using System.Collections.Generic;
using System.Net.Mail;

namespace SmtpServerStub.Utilities.Interfaces
{
    internal interface IEmailParser
    {
        MailAddress ParseEmailFromRecipientCommand(string commandStr);
        MailAddress ParseEmailFromEmailString(string commandStr);
        List<MailAddress> ParseEmailsFromString(string commandStr);
        List<MailAddress> ParseEmailsFromDataCc(string commandStr);
        List<MailAddress> ParseEmailsFromDataTo(string commandStr);
    }
}