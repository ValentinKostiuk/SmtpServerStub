using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;

namespace SmtpServerStub.Utilities.Interfaces
{
	internal interface IEmailParser
	{
		MailAddress ParseEmailFromString(string commandStr);
		MailAddress ParseEmailFromEmailString(string commandStr);
		List<MailAddress> ParseEmailsFromString(string commandStr);
		List<MailAddress> ParseEmailsFromDataCc(NameValueCollection headers);
		List<MailAddress> ParseEmailsFromDataTo(NameValueCollection headers);
		List<MailAddress> ParseEmailsFromDataFrom(NameValueCollection headers);
		string ParseSubjectFromDataSection(NameValueCollection headers);
		string ParseBodyFromDataSection(string dataSection);
		NameValueCollection ParseHeadersFromDataSection(string dataSection);
	}
}