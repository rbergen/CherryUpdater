using System;

namespace CherryUpdater
{
	public class ActivityStatusEventArgs : EventArgs
	{
		#region Member variables

		private readonly int mActivityIndex;
		private readonly int mActivityCount;

		#endregion

		#region Constructor

		public ActivityStatusEventArgs(int activityIndex, int activityCount)
		{
			mActivityIndex = activityIndex;
			mActivityCount = activityCount;
		}

		#endregion

		#region Properties

		public int ActivityIndex
		{
			get
			{
				return mActivityIndex;
			}
		}

		public int ActivityCount
		{
			get
			{
				return mActivityCount;
			}
		}

		#endregion
	}
}
