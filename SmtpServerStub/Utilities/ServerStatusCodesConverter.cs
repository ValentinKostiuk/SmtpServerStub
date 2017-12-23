using System.Collections.Generic;
using SmtpServerStub.Enums;

namespace SmtpServerStub.Utilities
{
	internal static class ServerStatusCodesConverter
    {
		private static readonly Dictionary<ResponseCodes, string> CommandsTemplates = new Dictionary<ResponseCodes, string>
		{
			{ResponseCodes.NonStdSuccess, "200 Nonstandard success"},
			{ResponseCodes.SysHelp, "211 System status {0}"},
			{ResponseCodes.HelpMsg, "214 Help message {0}"},
			{ResponseCodes.SrvReady, "220 {0} service ready"},
			{ResponseCodes.SrvClosingChannel, "221 {0} service closing transmission channel"},
			{ResponseCodes.SrvHello, "250-{0}Requested mail action okay, completed\r\n250 STARTTLS"},
			{ResponseCodes.RqstActOkCompleted, "250 Requested mail action okay, completed"},
			{ResponseCodes.UsrWillForward, "251 User not local; will forward to {0}"},
			{ResponseCodes.CantVrfyUserAttemptDelivery, "252 Cannot VRFY user, but will accept message and attempt delivery"},
			{ResponseCodes.StrtInputEndWith, "354 Start mail input; end with <CRLF>.<CRLF>"},
			{ResponseCodes.SrvNotAvailableClose, "421 {0} Service not available, closing transmission channel"},
			{ResponseCodes.RqstMailActNotTakenMbUnavailable, "450 Requested mail action not taken: mailbox unavailable"},
			{ResponseCodes.RqstActAbortErProcessing, "451 Requested action aborted: local error in processing"},
			{ResponseCodes.RqstActNotTakenSysStorage, "452 Requested action not taken: insufficient system storage"},
			{ResponseCodes.SyntaxErrorCommand, "500 Syntax error, command unrecognized"},
			{ResponseCodes.SyntaxErrorParam, "501 Syntax error in parameters or arguments"},
			{ResponseCodes.CommandNotImplemented, "502 Command not implemented"},
			{ResponseCodes.BadCommandsSequence, "503 Bad sequence of commands"},
			{ResponseCodes.CommandParamNotImplemented, "504 Command parameter not implemented"},
			{ResponseCodes.DoesNotAcceptMail, "521 {0} does not accept mail"},
			{ResponseCodes.AccessDenied, "530 Access denied"},
			{ResponseCodes.RqstActNotTakenMbUnavailable, "550 Requested action not taken: mailbox unavailable"},
			{ResponseCodes.TryFwd, "551 User not local; please try {0}"},
			{ResponseCodes.StorageAllocationLimit, "552 Requested mail action aborted: exceeded storage allocation"},
			{ResponseCodes.MbNameNotAllowed, "553 Requested action not taken: mailbox name not allowed"},
			{ResponseCodes.TransactionFailed, "554 Transaction failed"}
		};

		public static string GetTextResponseForStatus(ResponseCodes code, params string[] args)
		{
			var template = CommandsTemplates[code];
			template = string.Format(template, args);
			return template;
		}
	}
}