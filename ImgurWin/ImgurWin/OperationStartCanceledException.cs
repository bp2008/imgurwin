using System;
using System.Runtime.Serialization;

namespace ImgurWin
{
	[Serializable]
	internal class OperationStartCanceledException : Exception
	{
		public OperationStartCanceledException()
		{
		}

		public OperationStartCanceledException(string message) : base(message)
		{
		}

		public OperationStartCanceledException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected OperationStartCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}