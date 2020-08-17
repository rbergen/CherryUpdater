using System;
namespace CherryUpdater
{
	public interface IActivityEventProvider
	{
		#region Events

		event EventHandler<ActivityStatusEventArgs> ActivityExecuted;
		event EventHandler<ActivityFailedEventArgs> ActivityFailed;
		event EventHandler<ActivityLogEventArgs> ActivityLog;
		event EventHandler<ActivityProgressUpdatedEventArgs> ActivityProgressUpdated;
		event EventHandler<ActivityStatusEventArgs> ActivityStarting;
		event EventHandler<EventArgs> DevicesStateChanged;

		#endregion

		#region Event raisers

		void OnActivityExecuted(ActivityStatusEventArgs activityStatusEventArgs);
		void OnActivityFailed(ActivityFailedEventArgs activityFailedEventArgs);
		void OnActivityLog(ActivityLogEventArgs activityLogEventArgs);
		void OnActivityProgressUpdated(ActivityProgressUpdatedEventArgs activityProgressUpdatedEventArgs);
		void OnActivityStarting(ActivityStatusEventArgs activityStatusEventArgs);
		void OnDevicesStateChanged(EventArgs eventArgs);
		void OnVerifyActivityFailed(VerifyActivityFailedEventArgs eventArgs);

		#endregion

		#region Properties

		bool SuppressVerifyFailedEvents { get; set; }

		#endregion
	}
}
