namespace CherryUpdater
{
	public class QueryResult
	{
		#region Member variables

		private readonly bool mSuccess;
		private readonly string mMessage;

		#endregion

		#region Constructor

		public QueryResult(bool success, string message)
		{
			mSuccess = success;
			mMessage = message;
		}

		#endregion

		#region Properties

		public bool Success
		{
			get
			{
				return mSuccess;
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
