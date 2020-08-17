using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CherryUpdater
{
	public partial class TaskProgressForm : Form
	{
		#region Member variables

		private Task mTask;
		private ActivityExecutor mActivityExecutor;

		#endregion

		#region Delegates

		private delegate void handleExecutionEndDelegate(bool success, DecoratedMessage postMessage);

		#endregion

		#region Constructor

		public TaskProgressForm()
		{
			InitializeComponent();

			mStatusGroupBox.Text = Resources.LblStatusGroupBox;
			mLogGroupBox.Text = Resources.LblLogGroupBox;
			mStartButton.Text = Resources.LblStartButton;
			mAbortButton.Text = Resources.LblAbortButton;
			mCloseButton.Text = Resources.LblCloseButton;
		}

		#endregion

		#region Properties

		public Task Task
		{
			get
			{
				return mTask;
			}
			set
			{
				mTask = value;
				SuspendLayout();
				Text = mTask != null ? mTask.Title : string.Empty;
				mTaskProgressBar.Maximum = mTask != null ? mTask.Activities.Length - 1 : 0;
				mTaskProgressBar.Value = 0;
				mLogTextBox.Text = "";
				ResumeLayout(true);
			}
		}

		public ActivityExecutor ActivityExecutor
		{
			get
			{
				return mActivityExecutor;
			}
			set
			{
				if (mActivityExecutor == value)
				{
					return;
				}

				if (mActivityExecutor != null)
				{
					mActivityExecutor.ActivityStarting -= MActivityExecutor_ActivityStarting;
					mActivityExecutor.ActivityProgressUpdated -= MActivityExecutor_ActivityProgressUpdated;
					mActivityExecutor.ActivityLog -= MActivityExecutor_ActivityLog;
					mActivityExecutor.ActivityExecuted -= MActivityExecutor_ActivityExecuted;
					mActivityExecutor.ActivityFailed -= MActivityExecutor_ActivityFailed;
				}

				mActivityExecutor = value;

				if (mActivityExecutor != null)
				{
					mActivityExecutor.ActivityStarting += MActivityExecutor_ActivityStarting;
					mActivityExecutor.ActivityProgressUpdated += MActivityExecutor_ActivityProgressUpdated;
					mActivityExecutor.ActivityLog += MActivityExecutor_ActivityLog;
					mActivityExecutor.ActivityExecuted += MActivityExecutor_ActivityExecuted;
					mActivityExecutor.ActivityFailed += MActivityExecutor_ActivityFailed;
				}
			}
		}

		#endregion

		#region Private methods

		private void AddLogText(string text)
		{
			mLogTextBox.Text += text;
			Control activeControl = ActiveControl;
			mLogTextBox.Focus();
			mLogTextBox.SelectionStart = mLogTextBox.Text.Length;
			mLogTextBox.SelectionLength = 0;
			mLogTextBox.ScrollToCaret();
			ActiveControl = activeControl;
		}

		private void HandleExecutionEnd(bool success, DecoratedMessage postMessage)
		{
			if (InvokeRequired)
			{
				Invoke(new handleExecutionEndDelegate(HandleExecutionEnd), success, postMessage);
				return;
			}

			SuspendLayout();
			mTaskLabel.Text = success ? Resources.LblExecutionSuccessful : Resources.LblExecutionNotSuccessful;
			UseWaitCursor = false;
			mAbortButton.Enabled = false;
			mCloseButton.Enabled = true;
			ResumeLayout(true);

			if (postMessage != null && (success || postMessage.ShowIfFailed))
			{
				MessageBox.Show(this, postMessage.Message, postMessage.Caption, postMessage.Buttons, postMessage.Type);
			}

		}

		#endregion

		#region Event handlers

		private void This_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				if (mTask == null)
				{
					throw new InvalidOperationException("Task cannot be null when form is shown");
				}

				SuspendLayout();
				mTaskLabel.Text = Resources.LblClickToStart;
				mActivityProgressLabel.Text = "";
				mActivityProgressBar.Style = ProgressBarStyle.Continuous;
				mActivityProgressBar.Maximum = 1;
				mActivityProgressBar.Value = 0;
				mStartButton.Enabled = true;
				mAbortButton.Enabled = false;
				mCloseButton.Enabled = true;
				ResumeLayout(true);
			}
		}

		private void MStartButton_Click(object sender, EventArgs e)
		{
			SuspendLayout();
			mTaskLabel.Text = string.Format(Resources.LblTaskExecution, mTask.ProgressCaption);
			UseWaitCursor = true;
			mLogTextBox.UseWaitCursor = false;
			mStartButton.Enabled = false;
			mAbortButton.Enabled = true;
			mAbortButton.UseWaitCursor = false;
			mCloseButton.Enabled = false;
			ResumeLayout(true);

			new Thread(delegate ()
			{
				HandleExecutionEnd(mActivityExecutor.ExecuteTask(mTask), mTask.PostMessage);
			}).Start();
		}

		private void MAbortButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, Resources.MsgConfirmAbortQuestion, Resources.CptConfirmAbortQuestion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				mAbortButton.Enabled = false;
				mActivityExecutor.AbortRequested = true;
			}
		}

		private void MActivityExecutor_ActivityStarting(object sender, ActivityStatusEventArgs eventArgs)
		{
			if (!Visible)
			{
				return;
			}

			if (InvokeRequired)
			{
				Invoke(new EventHandler<ActivityStatusEventArgs>(MActivityExecutor_ActivityStarting), sender, eventArgs);
				return;
			}

			mActivityProgressLabel.Text = "";
			mActivityProgressBar.Style = ProgressBarStyle.Continuous;
			mActivityProgressBar.Maximum = 1;
			mActivityProgressBar.Value = 0;

			if (mLogTextBox.Text != string.Empty)
			{
				string doubleNewline = Environment.NewLine + Environment.NewLine;

				if (!mLogTextBox.Text.EndsWith(Environment.NewLine))
				{
					mLogTextBox.Text += Environment.NewLine;
				}

				if (!mLogTextBox.Text.EndsWith(doubleNewline))
				{
					AddLogText(Environment.NewLine);
				}
			}
		}

		private void MActivityExecutor_ActivityProgressUpdated(object sender, ActivityProgressUpdatedEventArgs eventArgs)
		{
			if (!Visible)
			{
				return;
			}

			if (InvokeRequired)
			{
				Invoke(new EventHandler<ActivityProgressUpdatedEventArgs>(MActivityExecutor_ActivityProgressUpdated), sender, eventArgs);
				return;
			}

			SuspendLayout();
			mActivityProgressLabel.Text = eventArgs.StatusText;
			if (eventArgs.TotalWork == -1)
			{
				mActivityProgressBar.Style = ProgressBarStyle.Marquee;
				mActivityProgressBar.Maximum = 1;
				mActivityProgressBar.Value = 0;
			}
			else
			{
				long totalWork = eventArgs.TotalWork;
				long doneWork = eventArgs.DoneWork;

				while (totalWork > int.MaxValue)
				{
					totalWork /= 1000;
					doneWork /= 1000;
				}
				if (doneWork > totalWork)
				{
					totalWork = doneWork;
				}

				mActivityProgressBar.Style = ProgressBarStyle.Continuous;
				mActivityProgressBar.Maximum = (int)totalWork;
				mActivityProgressBar.Value = (int)doneWork;
			}

			ResumeLayout(true);
		}

		private void MActivityExecutor_ActivityExecuted(object sender, ActivityStatusEventArgs eventArgs)
		{
			if (!Visible)
			{
				return;
			}

			if (InvokeRequired)
			{
				Invoke(new EventHandler<ActivityStatusEventArgs>(MActivityExecutor_ActivityExecuted), sender, eventArgs);
				return;
			}

			SuspendLayout();
			AddLogText(!mLogTextBox.Text.EndsWith(Environment.NewLine) ? Environment.NewLine + Resources.MsgActivityCompletedSuccess : Resources.MsgActivityCompletedSuccess);
			mTaskProgressBar.Maximum = eventArgs.ActivityCount;
			mTaskProgressBar.Value = eventArgs.ActivityIndex + 1;
			ResumeLayout(true);
		}

		private void MActivityExecutor_ActivityFailed(object sender, ActivityFailedEventArgs eventArgs)
		{
			if (!Visible)
			{
				return;
			}

			if (InvokeRequired)
			{
				Invoke(new EventHandler<ActivityFailedEventArgs>(MActivityExecutor_ActivityFailed), sender, eventArgs);
				return;
			}

			string text = Resources.MsgActivityFailed;

			if (eventArgs.Message != null)
			{
				text += " " + eventArgs.Message;
			}

			Exception exception = eventArgs.Exception;
			Exception handlingException = null;
			string path = Path.Combine(Environment.CurrentDirectory, "exception.log");

			if (exception != null)
			{
				if (!mTask.ContinueOnError)
				{
					try
					{
						using (StreamWriter writer = new StreamWriter(path, true))
						{
							writer.WriteLine(Resources.TxtActivityException, exception.GetType().FullName, exception.Message);
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
				}

				text += Environment.NewLine + string.Format(Resources.MsgActivityExceptionOccured, exception.GetType().Name, exception.Message);
			}

			string logMessage = mTask.ContinueOnError ? text : string.Format(Resources.MsgActivityCompletedFailure, text);
			AddLogText(mLogTextBox.Text.EndsWith(Environment.NewLine) ? logMessage : Environment.NewLine + logMessage);

			if (!mTask.ContinueOnError)
			{
				if (exception != null)
				{
					text += Environment.NewLine;

					if (handlingException == null)
					{
						text += string.Format(Resources.MsgDetailErrorLogWritten, path);
					}
					else
					{
						text += string.Format(Resources.MsgDetailErrorLogException, handlingException.GetType().Name, handlingException.Message);
					}
				}

				MessageBox.Show(this, text, Resources.CptActivityFailed, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void MActivityExecutor_ActivityLog(object sender, ActivityLogEventArgs eventArgs)
		{
			if (!Visible)
			{
				return;
			}

			if (InvokeRequired)
			{
				Invoke(new EventHandler<ActivityLogEventArgs>(MActivityExecutor_ActivityLog), sender, eventArgs);
				return;
			}

			AddLogText(eventArgs.IsLine ? eventArgs.Text + Environment.NewLine : eventArgs.Text);
		}

		#endregion
	}
}
