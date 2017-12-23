using System.Linq;
using System.Text;
using SmtpServerStub.Dtos;
using SmtpServerStub.Enums;
using SmtpServerStub.SmtpApplication.Interfaces;
using SmtpServerStub.Utilities;

namespace SmtpServerStub.SmtpApplication
{
    public class SmtpServerClientProcessor : ISmtpServerClientProcessor
    {
        private readonly ITcpClientController _clientController;

        public SmtpServerClientProcessor(ITcpClientController clientController)
        {
            _clientController = clientController;
        }

        public IMailMessage Run()
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
            string strMessage;

            do
            {
                strMessage = _clientController.Read();
                messageData.Append(strMessage);
            } while (!strMessage.EndsWith("\r\n.\r\n"));

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

            _clientController.Write(ServerStatusCodesConverter.GetTextResponseForStatus(ResponseCodes.RqstActOkCompleted));
            return true;
        }
    }
}