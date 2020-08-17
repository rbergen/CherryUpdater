namespace Managed.Adb
{
	/// <summary>
	/// A Sync progress monitor that does nothing
	/// </summary>
	public sealed class NullSyncProgressMonitor : ISyncProgressMonitor
	{

		public void Start(long totalWork)
		{
		}

		public void Stop()
		{
		}

		public bool IsCanceled
		{
			get
			{
				return false;
			}
		}

		public void StartSubTask(string source, string destination)
		{
		}

		public void Advance(long work)
		{
		}
	}
}
