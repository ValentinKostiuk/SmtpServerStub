using SmtpServerStub.Enums;

namespace SmtpServerStub.Utilities.Interfaces
{
    internal interface IServerStatusCodesConverter
    {
        string GetTextResponseForStatus(ResponseCodes code, params string[] args);
    }
}