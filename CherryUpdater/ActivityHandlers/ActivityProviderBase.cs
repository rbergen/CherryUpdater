using Managed.Adb.IO;
using System;
using System.IO;
using System.Threading;

namespace CherryUpdater
{
	abstract class ActivityProviderBase : IActivityProvider
	{
		#region Delegates

		protected delegate bool PerformDirEntryAction(string filePath, params string[] commandParams);

		#endregion

		#region Protected member variables

		protected IActivityEventProvider mEventProvider;
		protected bool mAbortRequested;

		#endregion

		#region Public static methods

		public static bool IsEnvironmentSane(out string message)
		{
			if (!File.Exists(GetADBPath()))
			{
				message = string.Format(Resources.MsgADBNotFound, GetADBPath());
				return false;
			}

			message = null;
			return true;
		}

		#endregion

		#region Protected static methods

		protected static string GetADBPath()
		{
			return "adb.exe";
		}

		#endregion

		#region Public methods

		public virtual bool HandleAbort()
		{
			if (AbortRequested)
			{
				AbortRequested = false;

				return true;
			}

			return false;
		}

		public virtual bool Adaptdir(string localDirectory, string fileMask, string remoteDirectory, bool continueOnError)
		{
			ValidateState();

			mEventProvider.OnActivityLog(new ActivityLogEventArgs(Resources.MsgActivityAdaptPushFiles, true));
			if (!Pushdir(localDirectory, fileMask, remoteDirectory, continueOnError) && !continueOnError)
			{
				return false;
			}

			mEventProvider.OnActivityLog(new ActivityLogEventArgs(Resources.MsgActivityAdaptChangePermissions, true));

			int fileCount = 0;

			try
			{
				string[] filePaths = Directory.GetFiles(localDirectory, fileMask);

				fileCount = filePaths.Length;

				for (int index = 0; index < fileCount; index++)
				{
					string fileName = Path.GetFileName(filePaths[index]);

					mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(string.Format(Resources.LblActivityAdaptChangePermission, fileName), index, fileCount));

					if (!Shell(string.Format("chmod 644 {0}", LinuxPath.Combine(remoteDirectory, fileName))) && !continueOnError)
					{
						return false;
					}
				}
			}
			finally
			{
				mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Resources.LblActivityAdapChangePermissionsFinished, fileCount, fileCount));
			}

			return true;
		}

		public virtual bool Adapt(string localDirectory, string file, string remoteDirectory)
		{
			ValidateState();

			mEventProvider.OnActivityLog(new ActivityLogEventArgs(Resources.MsgActivityAdaptPushFile, true));
			if (!Push(Path.Combine(localDirectory, file), remoteDirectory))
			{
				return false;
			}

			mEventProvider.OnActivityLog(new ActivityLogEventArgs(Resources.MsgActivityAdaptChangePermissions, true));

			return Shell(string.Format("chmod 644 {0}", LinuxPath.Combine(remoteDirectory, file)));
		}

		public virtual bool Reboot()
		{
			ValidateState();

			ExecuteShellCommand("reboot", false);

			mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Resources.LblWaitForReboot, 0, -1));

			do
			{
				Thread.Sleep(1000);

				if (GetADBState() == ADBState.NoDevices)
				{
					break;
				}
			}
			while (!AbortRequested);

			mEventProvider.SuppressVerifyFailedEvents = true;
			while (!AbortRequested)
			{
				Thread.Sleep(1000);

				if (GetADBState() == ADBState.AllOK && CheckBootComplete())
				{
					break;
				}
			}
			mEventProvider.SuppressVerifyFailedEvents = false;

			bool result = true;

			if (HandleAbort())
			{
				result = false;
			}

			mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Resources.LblRebootFinished, 1, 1));

			return result;
		}

		public virtual bool Shell(string shellCommand)
		{
			ValidateState();

			string output = ExecuteShellCommand(shellCommand, true);
			if (output == null)
			{
				return false;
			}

			return true;
		}

		public virtual bool Installdir(string packageDirectory, string packageMask, bool continueOnError)
		{
			return PerformDirAction(packageDirectory, packageMask, continueOnError, Resources.LblActivityInstallPackage, Resources.LblActivityInstallFinished, PerformInstallAction);
		}

		public virtual bool Pushdir(string localDirectory, string localFileMask, string remoteDirectory, bool continueOnError)
		{
			return PerformDirAction(localDirectory, localFileMask, continueOnError, Resources.LblActivityPushFile, Resources.LblActivityPushFinished, PerformPushAction, remoteDirectory);
		}

		public virtual void Dispose() { }

		public virtual void Start() { }

		public virtual void Stop() { }

		public string ExecuteShellCommand(string command)
		{
			return ExecuteShellCommand(command, false);
		}

		#endregion

		#region Protected methods

		protected virtual void ValidateState()
		{
			if (mEventProvider == null)
			{
				throw new InvalidOperationException("EventProvider cannot be null");
			}
		}

		protected virtual bool InstalldirEntry(string package)
		{
			throw new NotImplementedException();
		}

		protected virtual bool PushdirEntry(string filePath, string remoteDirectory)
		{
			throw new NotImplementedException();
		}

		protected bool PerformDirAction(string localDirectory, string fileMask, bool continueOnError, string updateLabelFormat, string finishLabel, PerformDirEntryAction performDirEntryAction, params string[] commandParams)
		{
			ValidateState();
			string[] filePaths;
			try
			{
				filePaths = Directory.GetFiles(localDirectory, fileMask);
			}
			catch (Exception ex)
			{
				mEventProvider.OnActivityFailed(new ActivityFailedEventArgs(ex, Resources.MsgCantLoadDirectoryFiles));
				return false;
			}

			int fileCount = filePaths.Length;
			try
			{
				for (int index = 0; index < fileCount; index++)
				{
					mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(string.Format(updateLabelFormat, filePaths[index]), index, fileCount));

					try
					{
						if (!performDirEntryAction(filePaths[index], commandParams) && !continueOnError)
						{
							return false;
						}
					}
					catch (Exception exception)
					{
						mEventProvider.OnActivityFailed(new ActivityFailedEventArgs(exception));
						if (!continueOnError)
						{
							return false;
						}
					}
				}
			}
			finally
			{
				mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(finishLabel, fileCount, fileCount));
			}

			return true;
		}

		#endregion

		#region Private methods

		private bool CheckBootComplete()
		{
			string bootComplete = ExecuteShellCommand("getprop dev.bootcomplete", false);

			return !string.IsNullOrEmpty(bootComplete) && bootComplete.Trim() == "1";
		}

		private bool PerformInstallAction(string filePath, params string[] commandParams)
		{
			return InstalldirEntry(filePath);
		}

		private bool PerformPushAction(string filePath, params string[] commandParams)
		{
			return PushdirEntry(filePath, commandParams[0]);
		}

		#endregion

		#region Public abstract methods

		public abstract ADBState GetADBState();
		public abstract bool Install(string packagePath);
		public abstract bool IsDeviceConnected { get; }
		public abstract string ProductModel { get; }
		public abstract bool Push(string localFile, string remotePath);
		public abstract bool Remount();
		public abstract bool KillServer();

		#endregion

		#region Protected abstract methods

		protected abstract string ExecuteShellCommand(string command, bool reportOutput);

		#endregion

		#region Properties

		public IActivityEventProvider EventProvider
		{
			get
			{
				return mEventProvider;
			}
			set
			{
				mEventProvider = value;
			}
		}

		public virtual bool AbortRequested
		{
			get
			{
				return mAbortRequested;
			}
			set
			{
				mAbortRequested = value;
			}
		}

		#endregion
	}
}
