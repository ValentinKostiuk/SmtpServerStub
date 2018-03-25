namespace SmtpServerStub.Utilities.Interfaces
{
    interface IMailContentDecoder
    {
	    string DecodeContent(string contentType, string contentTransferEncoding, string dataToDecode);
    }
}
