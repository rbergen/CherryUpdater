using System;

namespace CherryUpdater
{
	public class ActivityProgressUpdatedEventArgs : EventArgs
	{
		#region Member variables

		private readonly string mStatusText;
		private readonly long mDoneWork;
		private readonly long mTotalWork;

		#endregion

		#region Constructor

		public ActivityProgressUpdatedEventArgs(string statusText, long doneWork, long totalWork)
		{
			mStatusText = statusText;
			mDoneWork = doneWork;
			mTotalWork = totalWork;
		}

		#endregion

		#region Properties

		public string StatusText
		{
			get
			{
				return mStatusText;
			}
		}

		public long DoneWork
		{
			get
			{
				return mDoneWork;
			}
		}

		public long TotalWork
		{
			get
			{
				return mTotalWork;
			}
		}

		#endregion
	}
}
