using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SmtpServerStub.Utilities.Interfaces;

namespace SmtpServerStub.Utilities
{
    class MailContentDecoder: IMailContentDecoder
	{
		private static readonly Regex CharsetRegex =
			new Regex("charset=(?<charset>.+)",
				RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public string DecodeContent(string contentType, string contentTransferEncoding, string dataToDecode)
		{
			var charsetMatch = CharsetRegex.Match(contentType);
			var charset = charsetMatch.Groups["charset"].Value;

			switch (contentTransferEncoding)
			{
				case "quoted-printable":
				{
					return DecodeQuotedPrintable(dataToDecode, charset);
				}
				case "base64":
				{
					return DecodeBase64(dataToDecode, charset);
				}
				default:
				{
						throw new ArgumentException("Unrecognizable transfer encoding.");
				}
			}
		}

		private static string DecodeQuotedPrintable(string data, string bodycharset)
		{
			var i = 0;
			var output = new List<byte>();
			while (i < data.Length)
			{
				if (data[i] == '=' && data[i + 1] == '\r' && data[i + 2] == '\n')
				{
					//Skip
					i += 3;
				}
				else if (data[i] == '=')
				{
					var sHex = data;
					sHex = sHex.Substring(i + 1, 2);
					var hex = Convert.ToInt32(sHex, 16);
					var b = Convert.ToByte(hex);
					output.Add(b);
					i += 3;
				}
				else
				{
					output.Add((byte)data[i]);
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

		private static string DecodeBase64(string data, string bodycharset)
		{
			var output = Convert.FromBase64String(data);

			if (string.IsNullOrEmpty(bodycharset))
			{
				return Encoding.UTF8.GetString(output);
			}
			if (String.Compare(bodycharset, "ISO-2022-JP", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Encoding.GetEncoding("Shift_JIS").GetString(output);
			}
			return Encoding.GetEncoding(bodycharset).GetString(output);
		}
	}
}
