using System;
using System.Linq;
using System.Text;
using SmtpServerStub.Dtos;
using SmtpServerStub.Enums;
using SmtpServerStub.SmtpApplication.Interfaces;
using SmtpServerStub.Utilities;
using SmtpServerStub.Utilities.Interfaces;

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
            SendServerReady();
            return ReceiveMessage();
        }

        private MailMessage ReceiveMessage()
        {
            var message = new MailMessage();
            var processingFinished = false;
            do
            {
                var nextLine = _clientController.Read();

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
            } while (!processingFinished);

            _clientController.Close();
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
            var response = ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvReady, _clientController.HostName);
            _clientController.Write(response);
            _clientController.SwitchToSslProtocol();
            return false;
        }

        private bool HandleQuit(MailMessage message, string nextLine)
        {
            return true;
        }

        private bool HandleHello(MailMessage message, string nextLine)
        {
            var response = ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.SrvHello, _clientController.HostName);
            _clientController.Write(response);
            return false;
        }

        private bool HandleMailTo(MailMessage message, string nextLine)
        {
            try
            {
                var recipient = EmailParser.ParseEmailFromRecipientCommand(nextLine);
                if (message.To.All(a => a.Address != recipient.Address))
                {
                    message.To.Add(recipient);
                }
                _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
                return false;
            }
            catch
            {
                _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed));
                return true;
            }
        }

        private bool HandleMailFrom(MailMessage message, string nextLine)
        {
            try
            {
                var sender = EmailParser.ParseEmailFromRecipientCommand(nextLine);
                message.From = sender;
                _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
                return false;
            }
            catch
            {
                _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.MbNameNotAllowed));
                return true;
            }
        }

        private bool HandleDataSection(MailMessage message, string nextLine)
        {
            _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.StrtInputEndWith));
            var messageData = new StringBuilder();

            string strMessage = _clientController.Read();
            _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));

            while (!strMessage.EndsWith("\r\n.\r\n"))
            {
                messageData.Append(strMessage);
                strMessage = _clientController.Read();
            }

            var msgDataStr = messageData.ToString();

            var cc = EmailParser.ParseEmailsFromDataCc(msgDataStr);
            cc.ForEach(a => message.CC.Add(a));

            var toList = EmailParser.ParseEmailsFromDataTo(msgDataStr);

            toList.ForEach(a =>
            {
                var to = message.To.FirstOrDefault(address => Equals(address, a));
                var indexOfTo = message.To.IndexOf(to);
                if (indexOfTo != -1)
                {
                    message.To[indexOfTo] = a;
                }
                else
                {
                    message.To.Add(a);
                }
            });

            //Console.WriteLine("\n\n\n----------------------------\n\n\n" + messageData.ToString() + "----------------------------\n\n\n");

            _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
            return true;
        }
    }
}