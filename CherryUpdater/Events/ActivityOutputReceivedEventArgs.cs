using System;

namespace CherryUpdater
{
	public class ActivityLogEventArgs : EventArgs
	{
		#region Member variables

		private readonly string mText;
		private readonly bool mIsLine;

		#endregion

		#region Constructor

		public ActivityLogEventArgs(string text, bool isLine)
		{
			mText = text;
			mIsLine = isLine;
		}

		public ActivityLogEventArgs(string text) : this(text, false) { }

		#endregion

		#region Properties

		public bool IsLine
		{
			get
			{
				return mIsLine;
			}
		}

		public string Text
		{
			get
			{
				return mText;
			}
		}

		#endregion
	}
}
