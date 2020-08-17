using Managed.Adb;
using System;
using System.Collections.Generic;
using Log = Managed.Adb.Log;

namespace CherryUpdater
{
	public class ActivityExecutor : IActivityEventProvider
	{
		#region Constants

		private const string cActivityVerify = "verify";
		private const string cActivityReboot = "reboot";
		private const string cActivityRemount = "remount";
		private const string cActivityShell = "shell";
		private const string cActivityPush = "push";
		private const string cActivityPushdir = "pushdir";
		private const string cActivityInstall = "install";
		private const string cActivityInstalldir = "installdir";
		private const string cActivityAdapt = "adapt";
		private const string cActivityAdaptdir = "adaptdir";
		private const string cActivityKillServer = "kill-server";

		private const string cLogTag = "ActivityExecutor";

		#endregion

		#region Private member variables

		private IActivityProvider mActivityProvider;
		private IDeviceNameProvider mDeviceNameProvider;
		private bool mSuppressVerifyFailedEvents = false;

		#endregion

		#region Events

		public event EventHandler<ActivityLogEventArgs> ActivityLog;
		public event EventHandler<ActivityProgressUpdatedEventArgs> ActivityProgressUpdated;
		public event EventHandler<ActivityStatusEventArgs> ActivityExecuted;
		public event EventHandler<ActivityStatusEventArgs> ActivityStarting;
		public event EventHandler<ActivityFailedEventArgs> ActivityFailed;
		public event EventHandler<VerifyActivityFailedEventArgs> VerifyActivityFailed;

		public event EventHandler<EventArgs> DevicesStateChanged;

		#endregion

		#region Public static methods

		public static bool IsEnvironmentSane(out string message)
		{
			return ActivityProviderBase.IsEnvironmentSane(out message);
		}

		#endregion

		#region Public methods

		public bool GetConnectionReady()
		{
			ValidateState();

			return mActivityProvider.GetADBState() == ADBState.AllOK;
		}

		public QueryResult ExecuteQuery(Query query)
		{
			if (!mActivityProvider.IsDeviceConnected)
			{
				return new QueryResult(false, string.Format("{0} {1}", Resources.MsgQueryFailed, Resources.MsgNoDevices));
			}

			string resultText = query.Process(mActivityProvider.ExecuteShellCommand(query.Command));

			if (string.IsNullOrEmpty(resultText))
			{
				return new QueryResult(false, Resources.MsgQueryFailed);
			}

			return new QueryResult(true, resultText);
		}

		public bool ExecuteActivities(string[] activities)
		{
			return ExecuteActivities(activities, false);
		}

		public bool ExecuteTask(Task task)
		{
			return ExecuteActivities(task.Activities, task.ContinueOnError);
		}

		public bool ExecuteActivities(string[] activities, bool continueOnError)
		{
			ValidateState();

			if (activities == null || activities.Length == 0)
			{
				return true;
			}

			mActivityProvider.AbortRequested = false;

			if (activities[0] != cActivityVerify && activities[0] != cActivityKillServer && !mActivityProvider.IsDeviceConnected)
			{
				OnActivityFailed(new ActivityFailedEventArgs(Resources.MsgNoDevices));
				return false;
			}

			for (int activityIndex = 0; activityIndex < activities.Length; activityIndex++)
			{
				OnActivityStarting(new ActivityStatusEventArgs(activityIndex, activities.Length));

				Log.V(cLogTag, "Executing activity {0} ({1} of {2})", activities[activityIndex], activityIndex + 1, activities.Length);

				if (ExecuteActivity(activities[activityIndex], activityIndex, activities.Length, continueOnError))
				{
					Log.V(cLogTag, "Successfully completed activity {0} ({1} of {2})", activities[activityIndex], activityIndex + 1, activities.Length);
					OnActivityExecuted(new ActivityStatusEventArgs(activityIndex, activities.Length));
				}
				else
				{
					if (!continueOnError)
					{
						Log.V(cLogTag, "Aborting after failed activity {0} ({1} of {2})", activities[activityIndex], activityIndex + 1, activities.Length);
						return false;
					}

					Log.V(cLogTag, "Continuing after failed activity {0} ({1} of {2})", activities[activityIndex], activityIndex + 1, activities.Length);
				}

				if (mActivityProvider.HandleAbort() || !mActivityProvider.IsDeviceConnected)
				{
					Log.V(cLogTag, "Aborting due to user request or connectivity loss after activity {0} ({1} of {2})", activities[activityIndex], activityIndex + 1, activities.Length);
					return false;
				}
			}

			return true;
		}

		public bool VerifyConnection()
		{
			ValidateState();

			ADBState state;

			while (true)
			{
				state = mActivityProvider.GetADBState();

				if (state == ADBState.AllOK)
				{
					return true;
				}

				VerifyActivityFailedEventArgs eventArgs = new VerifyActivityFailedEventArgs(state);

				OnVerifyActivityFailed(eventArgs);

				if (!eventArgs.RetryVerificationSet)
				{
					return false;
				}
			}
		}

		public void Dispose()
		{
			ValidateState();

			mActivityProvider.Dispose();
		}

		#endregion

		#region Private methods

		private bool ExecuteActivity(string activity, int activityIndex, int activityCount, bool continueOnError)
		{
			int spaceIndex = activity.IndexOf(' ');

			string activityCommand;
			string activityParameterstring;
			string[] activityParameters;

			if (spaceIndex == -1)
			{
				activityCommand = activity;
				activityParameterstring = null;
				activityParameters = new string[0];
			}
			else
			{
				activityCommand = activity.Substring(0, spaceIndex);
				activityParameterstring = activity.Substring(spaceIndex + 1).Trim();
				activityParameters = activityParameterstring.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			}

			if (!VerifyParameterCount(activityCommand, activityParameters.Length))
			{
				OnActivityLog(new ActivityLogEventArgs(string.Format(Resources.MsgActivitySkipped, activityIndex + 1, activityCount, activityCommand, activityParameters.Length, activity), true));
				return false;
			}

			OnActivityLog(new ActivityLogEventArgs(string.Format(Resources.MsgActivityStart, activityIndex + 1, activityCount, string.Format(GetActivityStartMessage(activityCommand, activityParameterstring, activityParameters))), true));

			try
			{
				switch (activityCommand)
				{
					case cActivityVerify:
						bool verifyResult = VerifyConnection();

						if (!verifyResult)
						{
							OnActivityFailed(new ActivityFailedEventArgs(Resources.MsgVerifyFailed));
						}

						return verifyResult;

					case cActivityReboot:
						return mActivityProvider.Reboot();

					case cActivityRemount:
						return mActivityProvider.Remount();

					case cActivityShell:
						return mActivityProvider.Shell(activityParameterstring);

					case cActivityPush:
						return mActivityProvider.Push(activityParameters[0], activityParameters[1]);

					case cActivityPushdir:
						return mActivityProvider.Pushdir(activityParameters[0], activityParameters[1], activityParameters[2], continueOnError);

					case cActivityInstall:
						return mActivityProvider.Install(activityParameters[0]);

					case cActivityInstalldir:
						return mActivityProvider.Installdir(activityParameters[0], activityParameters[1], continueOnError);

					case cActivityAdapt:
						return mActivityProvider.Adapt(activityParameters[0], activityParameters[1], activityParameters[2]);

					case cActivityAdaptdir:
						return mActivityProvider.Adaptdir(activityParameters[0], activityParameters[1], activityParameters[2], continueOnError);

					case cActivityKillServer:
						return mActivityProvider.KillServer();
				}
			}
			catch (Exception ex)
			{
				OnActivityFailed(new ActivityFailedEventArgs(ex));
				return false;
			}

			return true;
		}

		private bool VerifyParameterCount(string activityCommand, int parameterCount)
		{
			switch (activityCommand)
			{
				case cActivityReboot:
				case cActivityRemount:
				case cActivityKillServer:
					return parameterCount == 0;

				case cActivityShell:
					return parameterCount > 0;

				case cActivityInstall:
					return parameterCount == 1;

				case cActivityPush:
				case cActivityInstalldir:
					return parameterCount == 2;

				case cActivityPushdir:
				case cActivityAdapt:
				case cActivityAdaptdir:
					return parameterCount == 3;

				default:
					return true;
			}
		}

		private string GetActivityStartMessage(string activityCommand, string activityParameterstring, string[] activityParameters)
		{
			string message;
			switch (activityCommand)
			{
				case cActivityKillServer:
					message = Resources.MsgActivityStartKillServer;
					break;

				case cActivityVerify:
					message = Resources.MsgActivityStartVerify;
					break;

				case cActivityReboot:
					message = Resources.MsgActivityStartReboot;
					break;

				case cActivityRemount:
					message = Resources.MsgActivityStartRemount;
					break;

				case cActivityShell:
					message = Resources.MsgActivityStartShell;
					break;

				case cActivityPush:
					message = Resources.MsgActivityStartPush;
					break;

				case cActivityPushdir:
					message = Resources.MsgActivityStartPushdir;
					break;

				case cActivityInstall:
					message = Resources.MsgActivityStartInstall;
					break;

				case cActivityInstalldir:
					message = Resources.MsgActivityStartInstalldir;
					break;

				case cActivityAdapt:
					message = Resources.MsgActivityStartAdapt;
					break;

				case cActivityAdaptdir:
					message = Resources.MsgActivityStartAdaptdir;
					break;

				default:
					message = Resources.MsgActivityStartUnknown;
					break;
			}

			List<string> parameterInfo = new List<string>(activityParameters.Length + 1)
			{
				activityParameterstring
			};
			parameterInfo.AddRange(activityParameters);

			return string.Format(message, parameterInfo.ToArray());
		}

		private void ValidateState()
		{
			if (mActivityProvider == null)
			{
				throw new InvalidOperationException("ActivityProvider cannot be null");
			}
		}

		#endregion

		#region Public properties

		public IActivityProvider ActivityProvider
		{
			get
			{
				return mActivityProvider;
			}
			set
			{
				if (value == mActivityProvider)
				{
					return;
				}

				if (mActivityProvider != null)
				{
					mActivityProvider.Stop();
					mActivityProvider.EventProvider = null;
				}

				if (value != null)
				{
					value.EventProvider = this;
					value.Start();
				}

				mActivityProvider = value;
			}
		}

		public IDeviceNameProvider DeviceNameProvider
		{
			get
			{
				return mDeviceNameProvider;
			}

			set
			{
				mDeviceNameProvider = value;
			}
		}

		public string DeviceStatusText
		{
			get
			{
				ValidateState();

				ADBState state = mActivityProvider.GetADBState();

				switch (state)
				{
					case ADBState.NoDevices:
						return Resources.LblStatusNoDevices;

					case ADBState.MoreDevices:
						return Resources.LblStatusMoreDevices;

					case ADBState.CannotRun:
					case ADBState.Error:
						return Resources.LblStatusUnknown;

					default:
						string productName = mDeviceNameProvider != null ? mDeviceNameProvider.GetDeviceName(mActivityProvider.ProductModel) : mActivityProvider.ProductModel;

						return string.Format(state == ADBState.InvalidState ? Resources.LblStatusOneDeviceNotOnline : Resources.LblStatusOneDevice, productName ?? Resources.LblStatusUnknownModel);

				}
			}
		}

		public bool SuppressVerifyFailedEvents
		{
			get
			{
				return mSuppressVerifyFailedEvents;
			}
			set
			{
				mSuppressVerifyFailedEvents = value;
			}
		}

		public bool AbortRequested
		{
			get
			{
				ValidateState();

				return mActivityProvider.AbortRequested;
			}
			set
			{
				ValidateState();

				mActivityProvider.AbortRequested = value;
			}
		}

		public string ProductModel
		{
			get
			{
				ValidateState();

				return mActivityProvider.ProductModel;
			}
		}

		public bool EnableLogging
		{
			get
			{
				return (Log.LogOutput as ADBLogger) != null && ((ADBLogger)Log.LogOutput).Enabled;
			}
			set
			{
				if (value == EnableLogging)
				{
					return;
				}

				if ((Log.LogOutput as ADBLogger) == null)
				{
					Log.LogOutput = new ADBLogger();
				}

					((ADBLogger)Log.LogOutput).Enabled = value;
			}
		}

		public LogLevel.LogLevelInfo LogLevel
		{
			get
			{
				return Log.Level;
			}
			set
			{
				Log.Level = value;
			}
		}

		public bool IsDeviceConnected
		{
			get
			{
				ValidateState();

				return mActivityProvider.IsDeviceConnected;
			}
		}

		#endregion

		#region Event raisers

		public void OnActivityStarting(ActivityStatusEventArgs activityStatusEventArgs)
		{
			ActivityStarting?.Invoke(this, activityStatusEventArgs);
		}

		public void OnActivityLog(ActivityLogEventArgs activityLogEventArgs)
		{
			ActivityLog?.Invoke(this, activityLogEventArgs);
		}

		public void OnActivityFailed(ActivityFailedEventArgs activityFailedEventArgs)
		{
			ActivityFailed?.Invoke(this, activityFailedEventArgs);
		}

		public void OnActivityExecuted(ActivityStatusEventArgs activityStatusEventArgs)
		{
			ActivityExecuted?.Invoke(this, activityStatusEventArgs);
		}

		public void OnActivityProgressUpdated(ActivityProgressUpdatedEventArgs activityProgressUpdatedEventArgs)
		{
			ActivityProgressUpdated?.Invoke(this, activityProgressUpdatedEventArgs);
		}

		public void OnVerifyActivityFailed(VerifyActivityFailedEventArgs eventArgs)
		{
			if (!mSuppressVerifyFailedEvents && VerifyActivityFailed != null)
			{
				VerifyActivityFailed(typeof(ActivityExecutor), eventArgs);
			}
		}

		public void OnDevicesStateChanged(EventArgs eventArgs)
		{
			DevicesStateChanged?.Invoke(this, eventArgs);
		}

		#endregion
	}
}