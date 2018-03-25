using SmtpServerStub.Dtos;

namespace SmtpServerStub.SmtpApplication
{
	/// <summary>
	/// Empty implementation of ILogger. Used as null object in case logger was not passed.
	/// </summary>
	internal class NoopLogger: ILogger
    {
	    /// <inheritdoc />
		public void LogInfo(string message){}

	    /// <inheritdoc />
		public void LogError(string message){}

	    /// <inheritdoc />
		public void LogWarning(string message){}
    }
}
