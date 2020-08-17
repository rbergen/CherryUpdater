using System;

namespace CherryUpdater
{
	public class ActivityFailedEventArgs : EventArgs
	{
		#region Member variables

		private readonly Exception mException;
		private readonly string mMessage;

		#endregion

		#region Constructor

		public ActivityFailedEventArgs(Exception exception)
		{
			mException = exception;
		}

		public ActivityFailedEventArgs(string message)
		{
			mMessage = message;
		}

		public ActivityFailedEventArgs(Exception exception, string message)
		{
			mException = exception;
			mMessage = message;
		}

		#endregion

		#region Properties

		public Exception Exception
		{
			get
			{
				return mException;
			}
		}

		public string Message
		{
			get
			{
				return mMessage;
			}
		}

		#endregion
	}
}
