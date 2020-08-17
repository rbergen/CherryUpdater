using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CherryUpdater
{
	static class Program
	{
		#region Private static methods

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


			if (!MainForm.IsEnvironmentSane(out string message))
			{
				MessageBox.Show(string.Format(Resources.MsgEnvironmentNotSane, message), Resources.CptEnvironmentNotSane, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Application.Run(new MainForm());
		}

		private static void HandleException(Exception exception)
		{
			Exception handlingException = null;
			string path = Path.Combine(Environment.CurrentDirectory, "exception.log");

			try
			{
				using (StreamWriter writer = new StreamWriter(path, true))
				{
					writer.WriteLine(Resources.TxtUnhandledException, exception.GetType().FullName, exception.Message);
					writer.WriteLine(Resources.TxtStackTrace, exception.StackTrace);

					if (exception.InnerException != null)
					{
						writer.WriteLine(Resources.TxtInnerException, exception.InnerException);
					}

					if (exception.Data.Count > 0)
					{
						writer.WriteLine(Resources.TxtExceptionData);

						foreach (DictionaryEntry entry in exception.Data)
						{
							writer.WriteLine(string.Concat(new object[] { "  * ", entry.Key, " = ", entry.Value }));
						}
					}

					writer.WriteLine();
				}
			}
			catch (Exception ex)
			{
				handlingException = ex;
			}

			string text = string.Format(Resources.MsgFatalErrorOccured, exception.GetType().Name, exception.Message);

			if (handlingException == null)
			{
				text += string.Format(Resources.MsgDetailErrorLogWritten, path);
			}
			else
			{
				text += string.Format(Resources.MsgDetailErrorLogException, handlingException.GetType().Name, handlingException.Message);
			}

			MessageBox.Show(text, Resources.CptUnhandledProgramError, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Application.Exit();
		}

		#endregion

		#region Event handlers

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			HandleException(e.Exception);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			HandleException((Exception)e.ExceptionObject);
		}

		#endregion
	}
}
