using Managed.Adb;
using Managed.Adb.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CherryUpdater
{
	class MadbActivityProvider : ActivityProviderBase
	{
		#region Constants

		private const string cProductModelProperty = "ro.product.model";

		#endregion

		#region Static variables

		private static AndroidDebugBridge mAndroidDebugBridge = null;
		private static bool mDisposed = false;

		#endregion

		#region Public methods

		public override void Dispose()
		{
			if (!mDisposed)
			{
				ADB.Stop();
				AndroidDebugBridge.Terminate();
				mDisposed = true;
			}
		}

		public override ADBState GetADBState()
		{
			try
			{
				List<Device> deviceList = ADB.Devices;

				if (deviceList.Count == 0)
				{
					return ADBState.NoDevices;
				}
				else if (deviceList.Count > 1)
				{
					return ADBState.MoreDevices;
				}

				Device device = deviceList[0];

				if (!device.IsOnline)
				{
					return ADBState.InvalidState;
				}

				if (!device.CanSU())
				{
					return ADBState.NoRoot;
				}

				string result = ExecuteShellCommand("id", false);
				if (result != null && result.Trim().StartsWith("uid=0"))
				{
					return ADBState.AllOK;
				}

				ExecuteShellCommand("setprop service.adb.root 1", false);
				Thread.Sleep(3000);
				return ADBState.AllOK;

			}
			catch
			{
				return ADBState.CannotRun;
			}

		}

		public override bool Install(string packagePath)
		{
			ValidateState();

			mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(string.Format(Resources.LblActivityInstallPackage, packagePath), 0, -1));

			try
			{
				ADBDevice.InstallPackage(packagePath, false);
			}
			finally
			{
				mEventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Resources.LblActivityInstallFinished, 1, 1));
			}

			return true;
		}

		public override bool Pushdir(string localDirectory, string localFileMask, string remoteDirectory, bool continueOnError)
		{
			ValidateState();

			return ProcessSyncResult(ADBDevice.SyncService.Push(Directory.GetFiles(localDirectory, localFileMask), ADBDevice.FileListingService.FindFileEntry(remoteDirectory), new MultipleFileProgressMonitor(this, localDirectory)));
		}

		public override bool Push(string localFile, string remotePath)
		{
			ValidateState();

			string remoteFile = remotePath;

			try
			{
				if (ADBDevice.FileListingService.FindFileEntry(remotePath).IsDirectory)
				{
					remoteFile = LinuxPath.Combine(remotePath, Path.GetFileName(localFile));
				}
			}
			catch (FileNotFoundException) { }

			return ProcessSyncResult(ADBDevice.SyncService.PushFile(localFile, remoteFile, new SingleFileProgressMonitor(this, localFile)));
		}

		public override bool KillServer()
		{
			return true;
		}

		public override bool Remount()
		{
			ValidateState();

			ADBDevice.RemountMountPoint("/system", false);

			return true;
		}

		public override void Start()
		{
			if (mAndroidDebugBridge != null)
			{
				mAndroidDebugBridge.Start();
			}
		}

		public override void Stop()
		{
			if (mAndroidDebugBridge != null)
			{
				mAndroidDebugBridge.Stop();
			}
		}

		#endregion

		#region Proctected methods

		protected override bool InstalldirEntry(string packagePath)
		{
			ADBDevice.InstallPackage(packagePath, false);
			return true;
		}

		protected override string ExecuteShellCommand(string command, bool reportOutput)
		{
			MultilineShellOutputReceiver outputReceiver = new MultilineShellOutputReceiver(this, reportOutput);

			try
			{
				ADBDevice.ExecuteRootShellCommand(command, outputReceiver);
			}
			catch (OperationCanceledException)
			{
				HandleAbort();

				return null;
			}

			return outputReceiver.CombinedOutput;
		}

		#endregion

		#region Private methods

		private void ReconnectDeviceStateChangedHandler()
		{
			if (ADBDevice != null)
			{
				ADBDevice.StateChanged -= ADBDevice_StateChanged;
				ADBDevice.StateChanged += ADBDevice_StateChanged;
			}
		}

		private bool ProcessSyncResult(SyncResult result)
		{
			string message = null;

			switch (result.Code)
			{
				case ErrorCodeHelper.RESULT_OK:
					return true;

				case ErrorCodeHelper.RESULT_CANCELED:
					HandleAbort();
					return false;

				case ErrorCodeHelper.RESULT_CONNECTION_ERROR:
					message = Resources.MsgSyncResultConnectionError;
					break;

				case ErrorCodeHelper.RESULT_FILE_READ_ERROR:
					message = Resources.MsgSyncResultFileReadError;
					break;

				case ErrorCodeHelper.RESULT_FILE_WRITE_ERROR:
					message = Resources.MsgSyncResultFileWriteError;
					break;

				case ErrorCodeHelper.RESULT_LOCAL_IS_DIRECTORY:
					message = Resources.MsgSyncResultLocalIsDirectory;
					break;

				case ErrorCodeHelper.RESULT_NO_DIR_TARGET:
					message = Resources.MsgSyncResultNoDirTarget;
					break;

				case ErrorCodeHelper.RESULT_NO_LOCAL_FILE:
					message = Resources.MsgSyncResultNoLocalFile;
					break;

				case ErrorCodeHelper.RESULT_NO_REMOTE_OBJECT:
					message = Resources.MsgSyncResultNoRemoteObject;
					break;

				case ErrorCodeHelper.RESULT_REMOTE_IS_FILE:
					message = Resources.MsgSyncResultRemoteIsFile;
					break;

				case ErrorCodeHelper.RESULT_REMOTE_PATH_ENCODING:
					message = Resources.MsgSyncResultRemotePathEncoding;
					break;

				case ErrorCodeHelper.RESULT_REMOTE_PATH_LENGTH:
					message = Resources.MsgSyncResultRemotePathLength;
					break;

				case ErrorCodeHelper.RESULT_TARGET_IS_FILE:
					message = Resources.MsgSyncResultTargetIsFile;
					break;

				case ErrorCodeHelper.RESULT_UNKNOWN_ERROR:
					message = Resources.MsgSyncResultUnknownError;
					break;
			}

			mEventProvider.OnActivityFailed(new ActivityFailedEventArgs(message));

			return false;
		}

		#endregion

		#region Properties

		public override string ProductModel
		{
			get
			{
				return ADBDevice?.GetProperty(cProductModelProperty);
			}
		}

		public override bool IsDeviceConnected
		{
			get
			{
				return mAndroidDebugBridge.Devices.Count == 1;
			}
		}

		private Device ADBDevice
		{
			get
			{
				return ADB.Devices.Count == 1 ? ADB.Devices[0] : null;
			}
		}

		private AndroidDebugBridge ADB
		{
			get
			{
				if (mAndroidDebugBridge == null)
				{
					try
					{
						AndroidDebugBridge.Initialize(false);
						mAndroidDebugBridge = AndroidDebugBridge.CreateBridge(GetADBPath(), true);
						mAndroidDebugBridge.DeviceConnected += MAndroidDebugBridge_DeviceConnected;
						mAndroidDebugBridge.DeviceDisconnected += MAndroidDebugBridge_DeviceDisconnected;
					}
					catch { }
				}

				return mAndroidDebugBridge;
			}
		}

		#endregion

		#region Event handlers

		private void ADBDevice_StateChanged(object sender, EventArgs e)
		{
			mEventProvider.OnDevicesStateChanged(e);
		}

		private void MAndroidDebugBridge_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			mEventProvider.OnDevicesStateChanged(e);

			ReconnectDeviceStateChangedHandler();
		}

		private void MAndroidDebugBridge_DeviceConnected(object sender, DeviceEventArgs e)
		{
			mEventProvider.OnDevicesStateChanged(e);

			ReconnectDeviceStateChangedHandler();
		}

		#endregion

		#region Inner classes

		private class MultipleFileProgressMonitor : ISyncProgressMonitor
		{
			#region Member variables

			private readonly MadbActivityProvider mActivityProvider;
			private long mTotalWork = 0;
			private long mPreviousWork = 0;
			private long mCurrentWork = 0;
			private string mCurrentPath = null;
			private readonly string mLocalDirectory;

			#endregion

			#region Constructor

			public MultipleFileProgressMonitor(MadbActivityProvider activityProvider, string localDirectory)
			{
				mActivityProvider = activityProvider;
				mLocalDirectory = localDirectory;
			}

			#endregion

			#region Public methods

			public void Advance(long work)
			{
				mCurrentWork = work;
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Message, mPreviousWork + work, mTotalWork));
			}

			public void Start(long totalWork)
			{
				mTotalWork = totalWork;
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Message, 0, totalWork));
			}

			public void StartSubTask(string source, string destination)
			{
				mCurrentPath = Path.Combine(mLocalDirectory, Path.GetFileName(source));
				mPreviousWork += mCurrentWork;
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Message, mPreviousWork, mTotalWork));
			}

			public void Stop()
			{
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Message, mTotalWork, mTotalWork));
			}

			#endregion

			#region Properties

			private string Message
			{
				get
				{
					return mCurrentPath == null ? Resources.LblActivityPushFiles : string.Format(Resources.LblActivityPushFile, mCurrentPath);
				}
			}

			public bool IsCanceled
			{
				get
				{
					return mActivityProvider.AbortRequested;
				}
			}

			#endregion
		}

		private class SingleFileProgressMonitor : ISyncProgressMonitor
		{
			#region Member variables

			private readonly MadbActivityProvider mActivityProvider;
			private readonly string mMessage;
			private long mTotalWork;

			#endregion

			#region Constructor

			public SingleFileProgressMonitor(MadbActivityProvider activityProvider, string fileName)
			{
				mActivityProvider = activityProvider;
				mMessage = string.Format(Resources.LblActivityPushFile, fileName);
			}

			#endregion

			#region Public methods

			public void Advance(long work)
			{
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(mMessage, work, mTotalWork));
			}

			public void Start(long totalWork)
			{
				mTotalWork = totalWork;
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(mMessage, 0, totalWork));
			}

			public void StartSubTask(string source, string destination) { }

			public void Stop()
			{
				mActivityProvider.EventProvider.OnActivityProgressUpdated(new ActivityProgressUpdatedEventArgs(Resources.LblActivityPushFinished, mTotalWork, mTotalWork));
			}

			#endregion

			#region Properties

			public bool IsCanceled
			{
				get
				{
					return mActivityProvider.AbortRequested;
				}
			}

			#endregion
		}

		private class MultilineShellOutputReceiver : MultiLineReceiver
		{
			#region Member variables

			private readonly MadbActivityProvider mActivityProvider;
			private readonly bool mReportOutput;
			private readonly List<string> mOutput;

			#endregion

			#region Constructor

			public MultilineShellOutputReceiver(MadbActivityProvider activityProvider, bool reportOutput)
			{
				TrimLines = false;
				mActivityProvider = activityProvider;
				mOutput = new List<string>();
				mReportOutput = reportOutput;
			}

			#endregion

			#region Protected methods

			protected override void ProcessNewLines(string[] lines)
			{
				mOutput.AddRange(lines);

				if (mReportOutput)
				{
					mActivityProvider.EventProvider.OnActivityLog(new ActivityLogEventArgs(CombineLines(lines)));
				}
			}

			#endregion

			#region Private methods

			private string CombineLines(string[] lines)
			{
				StringBuilder builder = new StringBuilder();

				foreach (string line in lines)
				{
					if (builder.Length == 0)
					{
						builder.Append(Environment.NewLine);
					}

					builder.AppendLine(line);
				}

				return builder.ToString();
			}

			#endregion

			#region Properties

			public override bool IsCancelled
			{
				get
				{
					return mActivityProvider.AbortRequested;
				}
			}

			public string CombinedOutput
			{
				get
				{
					return mOutput == null ? null : CombineLines(mOutput.ToArray());
				}
			}

			public string[] Output
			{
				get
				{
					return mOutput.ToArray();
				}
			}

			#endregion
		}

		#endregion

	}
}
