using Managed.Adb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace CherryUpdater
{
	public class AdbExeException : Exception
	{
		#region Member variables

		private readonly ADBState mState;
		private readonly string mMessage;

		#endregion

		#region Constructor

		public AdbExeException(ADBState state) : this(state, null) { }

		public AdbExeException(string message) : this(ADBState.Error, message) { }

		public AdbExeException(ADBState state, string message)
		{
			mState = state;
			mMessage = message;
		}

		#endregion

		#region Properties

		public ADBState ADBState
		{
			get
			{
				return mState;
			}
		}

		public override string Message
		{
			get
			{
				return mMessage ?? (mState == ADBState.CannotRun ? "Cannot run adb.exe" : "Error running adb.exe");
			}
		}

		#endregion
	}

	class AdbExeActivityProvider : ActivityProviderBase
	{
		#region Constants

		private const string cLogTag = "AdbExeAP";
		private const string cDeviceListRegex = @"^([^\s]+)\s+(device|offline|unknown|bootloader|recovery|download)$";

		#endregion

		#region Static variables

		private static string mProductModel = null;
		private static bool mIsDeviceConnected = false;
		private static readonly object mSyncLock = new object();
		private static DeviceMonitor mDeviceMonitor = null;
		private static StateKeeper mStateKeeper = null;

		#endregion

		#region Member variables

		private readonly ReportOutputTracker mReportOutputTracker;

		#endregion

		#region Constructor

		public AdbExeActivityProvider()
		{
			mReportOutputTracker = new ReportOutputTracker(this);
		}

		#endregion

		#region Public methods

		public override void Dispose()
		{
			lock (mSyncLock)
			{
				if (mDeviceMonitor != null)
				{
					mDeviceMonitor.Abort();
					mDeviceMonitor = null;
				}
			}
		}

		public override bool Push(string localFile, string remotePath)
		{
			ValidateState();

			if (!File.Exists(localFile))
			{
				mEventProvider.OnActivityFailed(new ActivityFailedEventArgs(Resources.MsgSyncResultNoLocalFile));
				return false;
			}

			ExecuteAdbCommand(string.Format("push {0} {1}", localFile, remotePath), true);
			return true;
		}

		public override bool Install(string packagePath)
		{
			ValidateState();

			if (!File.Exists(packagePath))
			{
				mEventProvider.OnActivityFailed(new ActivityFailedEventArgs(Resources.MsgSyncResultNoLocalFile));
				return false;
			}

			ExecuteAdbCommand(string.Format("install {0}", packagePath), true);
			return true;
		}

		public override bool Remount()
		{
			ValidateState();

			ExecuteAdbCommand("remount", false);
			return true;
		}

		public override void Start()
		{
			Log.V(cLogTag, "Starting ADB daemon");
			if (new OutputTriggerExecutor(this, "start-server", "* daemon started successfully *", true).ExecuteAndWait())
			{
				Log.V(cLogTag, "Starting ADB daemon successful");
			}
			else
			{
				Log.V(cLogTag, "Starting ADB daemon failed");
			}

			InitializeDeviceUpdate();
		}

		public override void Stop()
		{
			DeviceMonitor deviceMonitor;
			lock (mSyncLock)
			{
				deviceMonitor = mDeviceMonitor;
			}

			if (deviceMonitor != null)
			{
				deviceMonitor.Abort();
			}
		}

		public override bool KillServer()
		{
			ValidateState();

			ExecuteAdbCommand("kill-server", false);
			return true;
		}

		public override ADBState GetADBState()
		{
			try
			{
				Log.V(cLogTag, ">> Entering GetADBState()");

				lock (mSyncLock)
				{
					if (mStateKeeper == null)
					{
						mStateKeeper = new StateKeeper();
					}
					else if (!mStateKeeper.IsExpired)
					{
						Log.V(cLogTag, "Returning stored state");
						return mStateKeeper.State;
					}
				}

				Log.V(cLogTag, "Stored state not available or expired; establishing state");

				DeviceInfo[] devices = GetDevices();
				UpdateDeviceData(devices);

				if (devices.Length == 0)
				{

					return mStateKeeper.SetState(ADBState.NoDevices);
				}
				else if (devices.Length > 1)
				{
					return mStateKeeper.SetState(ADBState.MoreDevices);
				}

				if (!devices[0].Online)
				{
					return ADBState.InvalidState;
				}

				if (!GetDeviceRooted())
				{
					return mStateKeeper.SetState(ADBState.NoRoot);
				}

				return mStateKeeper.SetState(ADBState.AllOK);
			}
			catch (AdbExeException adbExeEx)
			{
				return adbExeEx.ADBState;
			}
			catch
			{
				return mStateKeeper.SetState(ADBState.CannotRun);
			}
			finally
			{
				Log.V(cLogTag, "<< Exiting GetADBState()");
			}
		}

		#endregion

		#region Protected methods

		protected override bool InstalldirEntry(string package)
		{
			ExecuteAdbCommand(string.Format("install {0}", package), true);
			return true;
		}

		protected override bool PushdirEntry(string filePath, string remoteDirectory)
		{
			ExecuteAdbCommand(string.Format("push {0} {1}", filePath, remoteDirectory), true);
			return true;
		}

		protected override string ExecuteShellCommand(string command, bool reportOutput)
		{
			return ExecuteAdbCommand("shell " + command, reportOutput);
		}

		#endregion

		#region Private methods

		private DeviceInfo[] GetDevices()
		{
			Log.V(cLogTag, ">> Entering getDevices()");
			try
			{
				string deviceListText = ExecuteAdbCommand("devices", false);

				string[] deviceListLines = deviceListText.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				bool deviceListStarted = false;
				List<DeviceInfo> deviceList = new List<DeviceInfo>();
				DeviceInfo currentDevice;

				foreach (string currentLine in deviceListLines)
				{
					if (deviceListStarted)
					{
						if ((currentDevice = DeviceInfo.CreateFromAdbDeviceListData(currentLine)) != null)
						{
							Log.V(cLogTag, "Found device: {0}", currentDevice);
							deviceList.Add(currentDevice);
						}

						continue;
					}

					if (currentLine.Trim() == "List of devices attached")
					{
						deviceListStarted = true;
					}
				}

				return deviceList.ToArray();
			}
			finally
			{
				Log.V(cLogTag, "<< Exiting getDevices()");
			}
		}

		private string GetProductModel()
		{
			try
			{
				string[] outputLines = ExecuteShellCommand("getprop ro.product.model", false).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				return outputLines[outputLines.Length - 1];
			}
			catch { }

			return null;
		}

		private bool GetDeviceRooted()
		{
			string[] outputLines = ExecuteAdbCommand("root", false).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			string trimmedLine;
			foreach (string line in outputLines)
			{
				trimmedLine = line.Trim();

				if (trimmedLine == "adbd is already running as root" || trimmedLine == "restarting adbd as root")
				{
					return true;
				}
			}

			return false;
		}

		private string ExecuteAdbCommand(string command, bool reportOutput)
		{
			return ExecuteAdbCommand(command, reportOutput, mReportOutputTracker);
		}

		private string ExecuteAdbCommand(string command, bool reportOutput, IExecuteAdbCommandTracker tracker)
		{
			ProcessStartInfo processInfo = new ProcessStartInfo(GetADBPath(), command)
			{
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			};

			int processId = -1;

			DataReceivedEventHandler dataHandler = tracker.DataHandler;

			try
			{
				lock (tracker.ProcessLock)
				{
					using (Process process = Process.Start(processInfo))
					{
						Log.I(cLogTag, "[Process {0}] Executing command: {1}", process.Id, command);

						tracker.SetProcessData(process, reportOutput);

						process.OutputDataReceived += dataHandler;
						process.ErrorDataReceived += dataHandler;

						OutputInfo outputInfo = new OutputInfo(reportOutput);

						processId = process.Id;

						process.BeginOutputReadLine();
						process.BeginErrorReadLine();

						process.WaitForExit();

						Log.V(cLogTag, "[Process {0}] Process {1} {2} ended with exit code {3}", processId, GetADBPath(), command, process.ExitCode);

						string returnValue = tracker.GetResultText(processId);

						if (process.ExitCode == 0)
						{
							return returnValue;
						}

						throw new AdbExeException(ADBState.Error, returnValue);
					}
				}

			}
			catch (AdbExeException)
			{
				throw;
			}
			catch { }
			finally
			{
				tracker.ClearProcessData(processId);
			}

			Log.E(cLogTag, "Unable to execute {0} {1}", GetADBPath(), command);
			throw new AdbExeException(ADBState.CannotRun);
		}

		private void InitializeDeviceUpdate()
		{
			DeviceMonitor deviceMonitor;

			lock (mSyncLock)
			{
				if (mDeviceMonitor != null && mDeviceMonitor.IsBound)
				{
					return;
				}

				if (mDeviceMonitor == null || mDeviceMonitor.StopRequested)
				{
					mDeviceMonitor = new DeviceMonitor();
				}

				mDeviceMonitor.ActivityProvider = this;

				deviceMonitor = mDeviceMonitor;
			}

			deviceMonitor.DeviceStatusChanged += MDeviceMonitor_DeviceStatusChanged;

			deviceMonitor.Start();
		}

		private void MDeviceMonitor_DeviceStatusChanged(object sender, DeviceStatusChangedEventArgs statusChangedEventArgs)
		{
			lock (mSyncLock)
			{
				if (mStateKeeper != null)
				{
					mStateKeeper.Reset();
				}
			}

			try
			{
				UpdateDeviceData(GetDevices());
			}
			catch (AdbExeException adbExeEx)
			{
				Log.E(cLogTag, "An error occured while trying to update device data: {0}", adbExeEx.Message);
			}

			mEventProvider.OnDevicesStateChanged(new EventArgs());

		}

		private void UpdateDeviceData(DeviceInfo[] devices)
		{
			try
			{
				Log.V(cLogTag, ">> Entering updateDeviceData({0})", DeviceInfo.ArrayToString(devices));
				lock (mSyncLock)
				{
					mProductModel = devices.Length == 1 ? GetProductModel() : null;
					mIsDeviceConnected = devices.Length == 1;
				}
			}
			finally
			{
				Log.V(cLogTag, "<< Exiting updateDeviceData({0})", DeviceInfo.ArrayToString(devices));
			}
		}

		#endregion

		#region Properties

		public override bool IsDeviceConnected
		{
			get
			{
				return mIsDeviceConnected;
			}
		}

		public override string ProductModel
		{
			get
			{
				lock (mSyncLock)
				{
					return mProductModel;
				}
			}
		}

		#endregion

		#region Private interfaces

		private interface IExecuteAdbCommandTracker
		{
			#region Properties

			DataReceivedEventHandler DataHandler { get; }
			void SetProcessData(Process process, bool reportOutput);
			object ProcessLock { get; }
			void ClearProcessData(int processId);
			string GetResultText(int processId);

			#endregion
		}

		#endregion

		#region Private classes

		private class OutputInfo
		{
			#region Member variables

			private readonly bool mReportOutput;
			private readonly StringBuilder mOutput;

			#endregion

			#region Public methods

			public OutputInfo(bool reportOutput)
			{
				mReportOutput = reportOutput;
				mOutput = new StringBuilder(string.Empty);
			}

			public static OutputInfo operator +(OutputInfo info, string text)
			{
				info.AppendOutput(text);

				return info;
			}

			public void AppendOutput(string text)
			{
				mOutput.Append(text);
				mOutput.Append(Environment.NewLine);
			}

			#endregion

			#region Properties

			public bool ReportOutput
			{
				get
				{
					return mReportOutput;
				}
			}

			public string Output
			{
				get
				{
					return mOutput.ToString();
				}
			}

			#endregion
		}

		private class DeviceInfo
		{
			#region Member variables

			private readonly string mSerialNumber;
			private bool mOnline;

			#endregion

			#region Constructor

			public DeviceInfo(string serialNumber, bool online)
			{
				mSerialNumber = serialNumber;
				mOnline = online;
			}

			#endregion

			#region Public methods

			public static string ArrayToString(DeviceInfo[] devices)
			{
				if (devices == null)
				{
					return "<none>";
				}

				StringBuilder builder = new StringBuilder();

				foreach (DeviceInfo device in devices)
				{
					if (builder.Length != 0)
					{
						builder.Append(" | ");
					}
					builder.Append(device.ToString());
				}

				return string.Format("[{0}]", builder.ToString());
			}

			public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("serial number={0},online status={1}", mSerialNumber, mOnline);
			}

			public static DeviceInfo CreateFromAdbDeviceListData(string adbDeviceListLine)
			{
				Regex regex = new Regex(cDeviceListRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
				Match match = regex.Match(adbDeviceListLine);
				if (match.Success)
				{
					return new DeviceInfo(match.Groups[1].Value, match.Groups[2].Value == "device");
				}

				return null;
			}

			public override bool Equals(object obj)
			{
				return obj is DeviceInfo info && mOnline == info.mOnline && mSerialNumber == info.mSerialNumber;
			}

			#endregion

			#region Properties

			public bool Online
			{
				get
				{
					return mOnline;
				}
				set
				{
					mOnline = value;
				}
			}

			public string SerialNumber
			{
				get
				{
					return mSerialNumber;
				}
			}

			#endregion
		}

		private class StateKeeper
		{
			#region Constants

			private const double cStateExpirationPeriod = 1;

			#endregion

			#region Member variables

			private ADBState mState;
			private DateTime mTimeStamp = DateTime.MinValue;
			private readonly object mSyncLock = new object();

			#endregion

			#region Public methods

			public ADBState SetState(ADBState state)
			{
				State = state;
				return state;
			}

			public void Reset()
			{
				lock (mSyncLock)
				{
					mTimeStamp = DateTime.MinValue;
				}
			}

			#endregion

			#region Properties

			public bool IsExpired
			{
				get
				{
					lock (mSyncLock)
					{
						return DateTime.Now - mTimeStamp > TimeSpan.FromSeconds(cStateExpirationPeriod);
					}
				}
			}

			public ADBState State
			{
				get
				{
					lock (mSyncLock)
					{
						return mState;
					}
				}
				set
				{
					lock (mSyncLock)
					{
						mState = value;
						mTimeStamp = DateTime.Now;
					}
				}
			}

			#endregion
		}

		private class OutputTriggerExecutor : IExecuteAdbCommandTracker
		{
			#region Constants

			private const int cKillTimeout = 1000;

			#endregion

			#region Member variables

			private Process mProcess;
			private int mProcessId;
			private readonly bool mExactMatch;
			private readonly string mCommand;
			private readonly string mTrigger;
			private readonly object mProcessLock = new object();
			private Exception mResultException;
			private readonly AutoResetEvent mTriggerEvent = new AutoResetEvent(false);
			private readonly AdbExeActivityProvider mActivityProvider;
			private bool mExecutionSuccessful = false;
			private readonly object mSyncLock = new object();
			private readonly Dictionary<int, Timer> mKillTimerMap = new Dictionary<int, Timer>();

			#endregion

			#region Constructor

			public OutputTriggerExecutor(AdbExeActivityProvider activityProvider, string command, string trigger, bool exactMatch)
			{
				mActivityProvider = activityProvider;
				mCommand = command;
				mTrigger = trigger;
				mExactMatch = exactMatch;
			}

			#endregion

			#region Public methods

			public void SetProcessData(Process process, bool reportOutput)
			{
				lock (mSyncLock)
				{
					mProcess = process;
					mProcessId = process.Id;
				}
			}

			public void ClearProcessData(int processId)
			{
				lock (mSyncLock)
				{
					if (mProcessId == processId)
					{
						mProcess = null;
					}
				}
			}

			public string GetResultText(int processId)
			{
				return null;
			}

			public bool ExecuteAndWait()
			{
				lock (mSyncLock)
				{
					mTriggerEvent.Reset();
					mExecutionSuccessful = false;
					mResultException = null;
				}

				Log.V(cLogTag, "Executing command {0} in a separate thread, waiting for text \"{1}\"", mCommand, mTrigger);

				new Thread(delegate ()
				{
					try
					{
						mActivityProvider.ExecuteAdbCommand(mCommand, false, this);
						mExecutionSuccessful = true;
					}
					catch (Exception ex)
					{
						mResultException = ex;
					}

					lock (mSyncLock)
					{
						mKillTimerMap.TryGetValue(mProcessId, out Timer killTimer);

						if (killTimer != null)
						{
							Log.V(cLogTag, "[Process {0}] Process exited, cancelling kill timer for separate thread command {1}", mProcessId, mCommand);
							killTimer.Change(Timeout.Infinite, Timeout.Infinite);
							mKillTimerMap.Remove(mProcessId);
						}

						mTriggerEvent.Set();
					}
				}).Start();

				mTriggerEvent.WaitOne();

				lock (mSyncLock)
				{
					return mExecutionSuccessful;
				}
			}

			#endregion

			#region Private methods

			private void OutputTriggerDataHandler(object sender, DataReceivedEventArgs dataReceivedEventArgs)
			{
				lock (mSyncLock)
				{
					if (dataReceivedEventArgs.Data == null || mExecutionSuccessful)
					{
						return;
					}
				}

				Log.V(cLogTag, "[Process {0}][OTE] * {2}", ((Process)sender).Id, mCommand, dataReceivedEventArgs.Data);

				if (mExactMatch ? dataReceivedEventArgs.Data.Trim() == mTrigger : dataReceivedEventArgs.Data.Contains(mTrigger))
				{
					Log.V(cLogTag, "[Process {0}] Found trigger \"{1}\" for separate thread command {2}, setting kill timer", ((Process)sender).Id, mTrigger, mCommand);

					lock (mSyncLock)
					{
						mKillTimerMap[((Process)sender).Id] = new Timer(ProcessKiller, null, cKillTimeout, Timeout.Infinite);
						mExecutionSuccessful = true;
					}

					mTriggerEvent.Set();
				}

			}

			private void ProcessKiller(object state)
			{
				lock (mSyncLock)
				{
					Log.V(cLogTag, "[Process {0}] Kill timer for separate thread command {1} expired. Killing process.", mProcess.Id, mCommand);

					try
					{
						if (mProcess != null)
						{
							mProcess.Kill();
						}
					}
					catch (Exception ex)
					{
						Log.V(cLogTag, "[Process {0}] Unable to kill process: {1}", mProcess.Id, ex.Message);
					}

					mKillTimerMap.Remove(mProcessId);
				}
			}

			#endregion

			#region Properties

			public DataReceivedEventHandler DataHandler
			{
				get
				{
					return OutputTriggerDataHandler;
				}
			}

			public object ProcessLock
			{
				get
				{
					return mProcessLock;
				}
			}

			public Exception ResultException
			{
				get
				{
					return mResultException;
				}
			}

			#endregion
		}

		private class ReportOutputTracker : IExecuteAdbCommandTracker
		{
			#region Static variables

			private static readonly object mProcessLock = new object();
			private static readonly Dictionary<int, OutputInfo> mOutputInfoMap = new Dictionary<int, OutputInfo>();

			#endregion

			#region Member variables

			private readonly AdbExeActivityProvider mActivityProvider;

			#endregion

			#region Constructor

			public ReportOutputTracker(AdbExeActivityProvider activityProvider)
			{
				mActivityProvider = activityProvider;
			}

			#endregion

			#region Public methods

			public void SetProcessData(Process process, bool reportOutput)
			{
				lock (mOutputInfoMap)
				{
					mOutputInfoMap[process.Id] = new OutputInfo(reportOutput);
				}
			}

			public void ClearProcessData(int processId)
			{
				lock (mOutputInfoMap)
				{
					mOutputInfoMap.Remove(processId);
				}
			}

			public string GetResultText(int processId)
			{
				OutputInfo outputInfo = GetOutputInfo(processId);

				return outputInfo?.Output.Trim();
			}

			#endregion

			#region Private methods

			private OutputInfo GetOutputInfo(int processId)
			{
				OutputInfo outputInfo;

				lock (mOutputInfoMap)
				{
					mOutputInfoMap.TryGetValue(processId, out outputInfo);
				}

				return outputInfo;
			}

			private void ReportOutputDataHandler(object sender, DataReceivedEventArgs dataReceivedEventArgs)
			{
				if (string.IsNullOrEmpty(dataReceivedEventArgs.Data))
				{
					return;
				}

				OutputInfo outputInfo = GetOutputInfo(((Process)sender).Id);

				if (outputInfo == null)
				{
					return;
				}

				if (outputInfo.ReportOutput)
				{
					mActivityProvider.EventProvider.OnActivityLog(new ActivityLogEventArgs(dataReceivedEventArgs.Data + Environment.NewLine));
				}

				outputInfo += dataReceivedEventArgs.Data;

				Log.V(cLogTag, "[Process {0}] * {1}", ((Process)sender).Id, outputInfo);
			}

			#endregion

			#region Properties

			public DataReceivedEventHandler DataHandler
			{
				get
				{
					return ReportOutputDataHandler;
				}
			}

			public object ProcessLock
			{
				get
				{
					return mProcessLock;
				}
			}

			#endregion
		}

		private class DeviceStatusChangedEventArgs : EventArgs
		{
			#region Member variables

			private readonly string mStatus;

			#endregion

			#region Constructor

			public DeviceStatusChangedEventArgs(string status)
			{
				mStatus = status;
			}

			#endregion

			#region Properties

			public string Status
			{
				get
				{
					return mStatus;
				}
			}

			#endregion
		}

		private class DeviceMonitor : IExecuteAdbCommandTracker
		{

			#region Events

			public EventHandler<DeviceStatusChangedEventArgs> DeviceStatusChanged;

			#endregion

			#region Static variables

			private static readonly object mProcessLock = new object();

			#endregion

			#region Member variables

			private Process mProcess;
			private string mStatus = "";
			private bool mStopRequested;
			private AdbExeActivityProvider mActivityProvider;
			private readonly object mSyncLock = new object();

			#endregion

			#region Public methods

			public void Abort()
			{
				Process process;

				lock (mSyncLock)
				{
					mStopRequested = true;
					process = mProcess;
					mProcess = null;
				}

				if (process != null)
				{
					try
					{
						Log.V(cLogTag, "Stopping device monitor thread. Ignore any \"Unable to execute\" message that may follow.");

						process.Kill();
					}
					catch { }
					finally
					{
						process.Close();
						process.Dispose();
					}
				}
			}

			public void Start()
			{
				Thread monitorThread = new Thread(delegate ()
				{
					while (!StopRequested)
					{
						try
						{
							mActivityProvider.ExecuteAdbCommand("status-window", false, this);
						}
						catch (AdbExeException adbExeEx)
						{
							if (!StopRequested)
							{
								Log.E(cLogTag, "An error occured trying to monitor device status: {0}. Waiting 5 seconds before retrying.", adbExeEx.Message);
								Thread.Sleep(5000);
							}
						}
						catch { }
					}

					Log.I(cLogTag, "Stop requested for device monitor thread, exiting");

					lock (mSyncLock)
					{
						mStopRequested = false;
					}
				})
				{
					IsBackground = true,
					Name = "AdbExeAP Monitor"
				};

				Log.I(cLogTag, "Starting device status monitor thread");
				monitorThread.Start();
			}

			public void SetProcessData(Process process, bool reportOutput)
			{
				lock (mSyncLock)
				{
					mProcess = process;
				}
			}

			public void ClearProcessData(int processId) { }

			public string GetResultText(int processId)
			{
				return null;
			}

			#endregion

			#region Private methods

			private void StatusWindowProcessDataHandler(object sender, DataReceivedEventArgs dataReceivedEventArgs)
			{
				if (dataReceivedEventArgs.Data == null)
				{
					return;
				}

				Log.V(cLogTag, "[Process {0}][DM] * {1}", ((Process)sender).Id, dataReceivedEventArgs.Data);

				if (dataReceivedEventArgs.Data.StartsWith("State:"))
				{
					string line = dataReceivedEventArgs.Data;
					string newStatus = line.Substring(line.IndexOf(' ') + 1);

					lock (mSyncLock)
					{
						if (newStatus == mStatus)
						{
							return;
						}

						Log.V(cLogTag, "[Process {0}] Device status changed from {1} to {2}", ((Process)sender).Id, string.IsNullOrEmpty(mStatus) ? "<unset>" : mStatus, string.IsNullOrEmpty(newStatus) ? "<unset>" : newStatus);

						mStatus = newStatus;
					}

					OnStatusChanged(new DeviceStatusChangedEventArgs(newStatus));
				}
			}

			private void OnStatusChanged(DeviceStatusChangedEventArgs eventArgs)
			{
				DeviceStatusChanged?.Invoke(this, eventArgs);
			}

			#endregion
			#region Properties

			public AdbExeActivityProvider ActivityProvider
			{
				set
				{
					lock (mSyncLock)
					{
						mActivityProvider = value;
					}
				}
			}

			public DataReceivedEventHandler DataHandler
			{
				get
				{
					return StatusWindowProcessDataHandler;
				}
			}

			public object ProcessLock
			{
				get
				{
					return mProcessLock;
				}
			}

			public bool IsBound
			{
				get
				{
					lock (mSyncLock)
					{
						return mProcess != null;
					}
				}
			}

			public bool StopRequested
			{
				get
				{
					lock (mSyncLock)
					{
						return mStopRequested;
					}
				}
			}

			#endregion

		}

		#endregion
	}
}