using System;

namespace SmtpServerStub.Exceptions
{
	internal class ClientControllerException : Exception
	{
		public ClientControllerException()
		{
		}

		public ClientControllerException(string message)
			: base(message)
		{
		}

		public ClientControllerException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}