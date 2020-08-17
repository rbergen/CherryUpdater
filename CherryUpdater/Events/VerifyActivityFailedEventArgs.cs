using System;

namespace CherryUpdater
{
	public class VerifyActivityFailedEventArgs : EventArgs
	{
		#region Member variables

		private readonly ADBState mADBState = ADBState.AllOK;
		private bool mRetryVerification = false;

		#endregion

		#region Constructor

		public VerifyActivityFailedEventArgs(ADBState state)
		{
			mADBState = state;
		}

		#endregion

		#region Public methods

		public void SetRetryVerification()
		{
			mRetryVerification = true;
		}

		#endregion

		#region Properties

		public ADBState ADBState
		{
			get
			{
				return mADBState;
			}
		}

		public bool RetryVerificationSet
		{
			get
			{
				return mRetryVerification;
			}
		}

		#endregion
	}
}
