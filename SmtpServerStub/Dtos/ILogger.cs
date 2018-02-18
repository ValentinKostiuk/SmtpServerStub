namespace SmtpServerStub.Dtos
{	
	/// <summary>
	/// Implementation of this interface will allow to log all internal events of SMTP server stub
	/// </summary>
	public interface ILogger
    {
	    /// <summary>
	    /// Used to log non critical information. Like port server.
	    /// </summary>
		void LogInfo(string message);
	    /// <summary>
	    /// Logs information about errors encountered bu server while receiving messages.
	    /// </summary>
		void LogError(string message);
	    /// <summary>
	    /// Used to log information about non critical errors. Like connection was closed by client before server reported to it about successful message perception. 
	    /// </summary>
		void LogWarning(string message);
    }
}
