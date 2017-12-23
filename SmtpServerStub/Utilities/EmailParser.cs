using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SmtpServerStub.Utilities
{
    internal static class EmailParser
    {
        private static readonly Regex MailAndNameReges =
            new Regex(@"(?:\s*[""'](?<name>.+?)[""']\s*)?<?(?<address>[^<>]+)>?\s*",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex CcStringRegex = new Regex("(?s)^Cc:\\s*(.+?)\\w+:.+",
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex ToStringRegex = new Regex("(?s)^To:\\s*(.+?)\\w+:.+",
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static MailAddress ParseEmailFromRecipientCommand(string commandStr)
        {
            var startIndex = commandStr.IndexOf("<", StringComparison.Ordinal);
            var foo = commandStr.Substring(startIndex);
            var emailStr = foo.TrimEnd('>');
            var result = new MailAddress(emailStr);
            return result;
        }

        public static MailAddress ParseEmailFromEmailString(string commandStr)
        {
            var match = MailAndNameReges.Match(commandStr);
            var address = match.Groups["address"].Value;
            var name = match.Groups["name"].Value;
            return new MailAddress(address, name);
        }

        public static List<MailAddress> ParseEmailsFromString(string commandStr)
        {
            var formattedString = commandStr.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);
            var splittedMailPairs = formattedString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return splittedMailPairs.Select(ParseEmailFromEmailString).Distinct().ToList();
        }

        public static List<MailAddress> ParseEmailsFromDataCc(string commandStr)
        {
            var match = CcStringRegex.Match(commandStr).Groups[1].Value;
            return ParseEmailsFromString(match);
        }

        public static List<MailAddress> ParseEmailsFromDataTo(string commandStr)
        {
            var match = ToStringRegex.Match(commandStr).Groups[1].Value;
            return ParseEmailsFromString(match);
        }
    }
}