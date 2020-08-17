using Managed.Adb;
using System;
using System.IO;
using System.Threading;

namespace CherryUpdater
{
	class ADBLogger : ILogOutput
	{
		#region Member variables

		private StreamWriter mLogWriter;
		private bool mEnabled = false;

		#endregion

		#region Destructor

		~ADBLogger()
		{
			Enabled = false;
		}

		#endregion

		#region Public methods

		public void Write(LogLevel.LogLevelInfo logLevel, string tag, string message)
		{
			if (!mEnabled || string.IsNullOrEmpty(message))
			{
				return;
			}

			try
			{
				mLogWriter.WriteLine(string.Format("[{0}|{1}|{2:D2}|{3}]{4}{5}", DateTime.Now.ToString("u"), logLevel.Letter, Thread.CurrentThread.ManagedThreadId, tag, message[0] == '[' ? "" : " ", message));
				mLogWriter.Flush();
			}
			catch { }
		}

		public void WriteAndPromptLog(LogLevel.LogLevelInfo logLevel, string tag, string message)
		{
			Write(logLevel, tag, message);
		}

		#endregion

		#region Properties

		public bool Enabled
		{
			get
			{
				return mEnabled;
			}
			set
			{
				if (value == mEnabled)
				{
					return;
				}

				mEnabled = value;

				try
				{
					if (mEnabled)
					{
						mLogWriter = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "madb.log"), true);
					}
					else
					{
						mLogWriter.Close();
						mLogWriter.Dispose();
					}
				}
				catch { }
			}
		}

		#endregion
	}
}
