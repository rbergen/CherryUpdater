namespace CherryUpdater
{
	public interface IActivityProvider
	{
		#region Methods

		bool Adapt(string localDirectory, string file, string remoteDirectory);
		bool Adaptdir(string localDirectory, string fileMask, string remoteDirectory, bool continueOnError);
		void Start();
		void Stop();
		void Dispose();
		string ExecuteShellCommand(string command);
		ADBState GetADBState();
		bool HandleAbort();
		bool Install(string packagePath);
		bool Installdir(string packageDirectory, string packageMask, bool continueOnError);
		bool Push(string localFile, string remotePath);
		bool Pushdir(string localDirectory, string localFileMask, string remoteDirectory, bool continueOnError);
		bool Reboot();
		bool Remount();
		bool Shell(string shellCommand);
		bool KillServer();

		#endregion

		#region Properties

		bool IsDeviceConnected { get; }
		string ProductModel { get; }
		IActivityEventProvider EventProvider { get; set; }
		bool AbortRequested { get; set; }

		#endregion
	}
}
