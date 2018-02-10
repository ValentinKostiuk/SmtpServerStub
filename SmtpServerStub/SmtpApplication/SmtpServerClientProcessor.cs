﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using SmtpServerStub.Dtos;
using SmtpServerStub.Enums;
using SmtpServerStub.SmtpApplication.Interfaces;
using SmtpServerStub.Utilities;
using SmtpServerStub.Utilities.Interfaces;
using MailMessage = SmtpServerStub.Dtos.MailMessage;

[assembly: InternalsVisibleTo("SmtpServerStubUnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace SmtpServerStub.SmtpApplication
{
	internal class SmtpServerClientProcessor : ISmtpServerClientProcessor
	{
		private readonly ITcpClientController _clientController;
		public IEmailParser EmailParser { get; set; }
		public IRequestCommandsConverter RequestCommandsConverter { get; set; }
		public IServerStatusCodesConverter ServerStatusCodesConverter { get; set; }
		public ILogger Logger { get; set; }

		public SmtpServerClientProcessor(ITcpClientController clientController, ILogger logger)
		{
			_clientController = clientController;
			EmailParser = new EmailParser();
			RequestCommandsConverter = new RequestCommandsConverter();
			ServerStatusCodesConverter = new ServerStatusCodesConverter();
			Logger = logger;
		}

		public MailMessage Run()
		{
			try
			{
				return ReceiveMessage();
			}
			finally
			{
				_clientController.Close();
			}
		}

		private MailMessage ReceiveMessage()
		{
			var message = new MailMessage();
			var processingFinished = false;

			SendServerReady();

			while (!processingFinished)
			{
				string nextLine;

				try
				{
					nextLine = _clientController.Read();
				}
				catch (Exception e)
				{
					Logger.LogWarning("Stream was closed before QUIT command from server:\n" + e.Message);
					break;
				}

				if (nextLine.Length > 0)
				{
					var messageCode = RequestCommandsConverter.ToRequestCommandCode(nextLine);
					switch (messageCode)
					{
						case RequestCommands.Hello:
							processingFinished = HandleHello(message, nextLine);
							break;
						case RequestCommands.MailFrom:
							processingFinished = HandleMailFrom(message, nextLine);
							break;
						case RequestCommands.RcptTo:
							processingFinished = HandleMailTo(message, nextLine);
							break;
						case RequestCommands.Data:
							processingFinished = HandleDataSection(message, nextLine);
							break;
						case RequestCommands.Quit:
							processingFinished = HandleQuit(message, nextLine);
							break;
						case RequestCommands.StartTls:
							processingFinished = SwitchToTls(message, nextLine);
							break;
						case RequestCommands.ParseError:
							processingFinished = true;
							break;
						default:
							break;
					}
				}
			}

			return message;
		}

		private void SendServerReady()
		{
			var response =
				ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _clientController.HostName);
			_clientController.Write(response);
		}

		private bool SwitchToTls(MailMessage message, string nextLine)
		{
			_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _clientController.HostName));
			try
			{
				_clientController.SwitchToTlsProtocol();
			}
			catch (Exception e)
			{
				Logger.LogError(string.Format("Exception occurred while switching to TLS:\n{0}", e.Message));
				_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.AccessDenied));
				return true;
			}
			return false;
		}

		private bool HandleQuit(MailMessage message, string nextLine)
		{
			return true;
		}

		private bool HandleHello(MailMessage message, string nextLine)
		{
			var responseStatus = _clientController.IsTlsAvailable ? ResponseCodes.SrvHello : ResponseCodes.SrvHelloNoTls;
			var response = ServerStatusCodesConverter.GetTextResponseForStatus(responseStatus, _clientController.HostName);
			_clientController.Write(response);
			return false;
		}

		private bool HandleMailTo(MailMessage message, string nextLine)
		{
			MailAddress recipient;

			try
			{
				recipient = EmailParser.ParseEmailFromString(nextLine);
			}
			catch
			{
				_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed));
				return true;
			}

			if (message.To.All(a => a.Address != recipient.Address))
			{
				message.To.Add(recipient);
			}
			_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
			return false;
		}

		private bool HandleMailFrom(MailMessage message, string nextLine)
		{
			MailAddress sender;
			try
			{
				sender = EmailParser.ParseEmailFromString(nextLine);
			}
			catch
			{
				_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed));
				return true;
			}

			message.From = sender;
			_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
			return false;
		}

		private bool HandleDataSection(MailMessage message, string nextLine)
		{
			_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.StrtInputEndWith));
			var messageData = new StringBuilder();

			var strMessage = _clientController.Read();
			_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));

			while (!strMessage.Contains("\r\n.\r\n"))//Contains because some times QUIT is added right to the body
			{
				messageData.Append(strMessage);
				strMessage = _clientController.Read();
			}
			messageData.Append(strMessage);

			var msgDataStr = messageData.ToString();

			var headers = EmailParser.ParseHeadersFromDataSection(msgDataStr);
			var cc = EmailParser.ParseEmailsFromDataCc(headers);
			var parsedToList = EmailParser.ParseEmailsFromDataTo(headers);
			var parsedFromList = EmailParser.ParseEmailsFromDataFrom(headers);
			var toList = MergeToList(message.To, parsedToList);

			message.To = toList;
			message.CC = cc;
			message.From = GetMailAddressByAddress(message.From, parsedFromList);

			message.Body = EmailParser.ParseBodyFromDataSection(msgDataStr);
			message.Subject = EmailParser.ParseSubjectFromDataSection(headers);
			message.Headers = headers;

			message.MailMessageDataSection = msgDataStr.Trim();

			_clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
			return false;
		}

		private List<MailAddress> MergeToList(IList<MailAddress> list1, IList<MailAddress> list2)
		{
			if (list2 == null || list2.Count == 0)
			{
				return list1.ToList();
			}

			var summary = list1.Concat(list2);
			var resultList = summary.Where(a => summary.Count(x => x.Address == a.Address) == 1 || !string.IsNullOrEmpty(a.DisplayName)).ToList();

			return resultList;
		}

		private MailAddress GetMailAddressByAddress(MailAddress address, IList<MailAddress> listOfMails)
		{
			if (address == null || listOfMails == null)
			{
				return address;
			}

			var foundMail = listOfMails.FirstOrDefault(m => m.Address == address.Address && !string.IsNullOrEmpty(m.DisplayName));
			return foundMail ?? address;
		}
	}
}