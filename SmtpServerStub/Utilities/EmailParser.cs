using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using SmtpServerStub.Utilities.Interfaces;

namespace SmtpServerStub.Utilities
{
	internal class EmailParser : IEmailParser
	{
		private static readonly Regex MailAndNameRegex =
			new Regex(@"(?:\s*[""'](?<name>.+?)[""']\s*)?<?(?<address>[^<>]+)>?\s*",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex MailAddressRegex =
			new Regex(@"<(?<address>[^<>]+?)>",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex CharsetRegex =
			new Regex("charset=(?<charset>.+)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex BodyStringPlainMessageRegex = new Regex("(?s)\\\r\\\n\\\r\\\n(?<body>.*)\\\r\\\n\\.\\\r\\\n",
			RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly Regex HeadersParsingRegex = new Regex("(?s)(?<name>[A-Za-z -]+?):\\s*(?<value>.+?)(?=(?:\\\r\\\n[A-Za-z -]+?:)|(?:$))",
			RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public virtual MailAddress ParseEmailFromString(string commandStr)
		{
			var match = MailAddressRegex.Match(commandStr);
			var address = match.Groups["address"].Value;
			return new MailAddress(address);
		}

		public virtual MailAddress ParseEmailFromEmailString(string commandStr)
		{
			var match = MailAndNameRegex.Match(commandStr);
			var address = match.Groups["address"].Value;
			var name = match.Groups["name"].Value;
			return new MailAddress(address, name);
		}

		public virtual List<MailAddress> ParseEmailsFromString(string commandStr)
		{
			var formattedString = commandStr.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);
			var splittedMailPairs = formattedString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
			return splittedMailPairs.Select(ParseEmailFromEmailString).Distinct().ToList();
		}

		public virtual List<MailAddress> ParseEmailsFromDataCc(NameValueCollection headers)
		{
			var match = headers.Get("Cc");
			return ParseEmailsFromString(match);
		}

		public virtual List<MailAddress> ParseEmailsFromDataTo(NameValueCollection headers)
		{
			var match = headers.Get("To");
			return ParseEmailsFromString(match);
		}

		public virtual List<MailAddress> ParseEmailsFromDataFrom(NameValueCollection headers)
		{
			var match = headers.Get("From");
			return ParseEmailsFromString(match);
		}

		public virtual string ParseBodyFromDataSection(string dataSection)
		{
			if (DataSectionHasBoundaries(dataSection))
			{
				return GetBodyFromMessageWithAttachments(dataSection);
			}
			return GetBodyFromPlainEmail(dataSection);
		}

		public virtual string ParseSubjectFromDataSection(NameValueCollection headers)
		{
			return headers.Get("Subject");
		}

		public NameValueCollection ParseHeadersFromDataSection(string dataSection)
		{
			var result = new NameValueCollection();

			var indexOfSectionEnd = dataSection.IndexOf("\r\n\r\n", StringComparison.Ordinal);

			if (indexOfSectionEnd != -1)
			{
				var headersSection = dataSection.Substring(0, indexOfSectionEnd);
				var matches = HeadersParsingRegex.Matches(headersSection);
				foreach (Match match in matches)
				{
					result.Add(match.Groups["name"].Value.Trim(), match.Groups["value"].Value.Trim());
				}
			}

			return result;
		}

		private string GetBodyFromPlainEmail(string dataSection)
		{
			var headers = ParseHeadersFromDataSection(dataSection);
			var match = BodyStringPlainMessageRegex.Match(dataSection);
			var body = match.Groups["body"].Value?.Trim();

			var contentType = headers["Content-Type"];
			var charsetMatch = CharsetRegex.Match(contentType);
			var charset = charsetMatch.Groups["charset"].Value;
			var decodedBody = DecodeQuotedBody(body, charset);
			return decodedBody;
		}

		private string GetBodyFromMessageWithAttachments(string dataSection)
		{
			return "";
		}

		private bool DataSectionHasBoundaries(string dataSection)
		{
			return dataSection.Contains("boundary=");
		}

		private static string DecodeQuotedBody(string bodyContent, string bodycharset)
		{
			var i = 0;
			var output = new List<byte>();
			while (i < bodyContent.Length)
			{
				if (bodyContent[i] == '=' && bodyContent[i + 1] == '\r' && bodyContent[i + 2] == '\n')
				{
					//Skip
					i += 3;
				}
				else if (bodyContent[i] == '=')
				{
					var sHex = bodyContent;
					sHex = sHex.Substring(i + 1, 2);
					var hex = Convert.ToInt32(sHex, 16);
					var b = Convert.ToByte(hex);
					output.Add(b);
					i += 3;
				}
				else
				{
					output.Add((byte) bodyContent[i]);
					i++;
				}
			}


			if (string.IsNullOrEmpty(bodycharset))
			{
				return Encoding.UTF8.GetString(output.ToArray());
			}
			if (String.Compare(bodycharset, "ISO-2022-JP", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Encoding.GetEncoding("Shift_JIS").GetString(output.ToArray());
			}
			return Encoding.GetEncoding(bodycharset).GetString(output.ToArray());
		}
	}
}