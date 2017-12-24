using SmtpServerStub.Enums;

namespace SmtpServerStub.Utilities.Interfaces
{
    internal interface IRequestCommandsConverter
    {
        RequestCommands ToRequestCommandCode(string commandText);
    }
}