using Managed.Adb;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CherryUpdater
{
	public partial class ConfigurationDialog : Form
	{
		#region Member variables

		private readonly List<LogLevelEntry> mSelectableLogLevels = new List<LogLevelEntry>();

		#endregion

		#region Constructor

		public ConfigurationDialog()
		{
			InitializeComponent();

			mSelectableLogLevels.Add(new LogLevelEntry(Resources.LblLogLevelVerbose, LogLevel.Verbose));
			mSelectableLogLevels.Add(new LogLevelEntry(Resources.LblLogLevelWarn, LogLevel.Warn));
			mSelectableLogLevels.Add(new LogLevelEntry(Resources.LblLogLevelError, LogLevel.Error));

			Text = Resources.CptConfigurationDialog;
			mUseAdbProviderCheckBox.Text = Resources.LblUseAdbProvider;
			mIgnoreProductModelCheckBox.Text = Resources.LblIgnoreProductModel;
			mShowHelpCheckBox.Text = Resources.LblShowHelp;
			mEnableLoggingCheckBox.Text = Resources.LblEnableLogging;
			mLogLevelComboBox.DataSource = mSelectableLogLevels;
			mLogLevelComboBox.DisplayMember = "Label";
			mLogLevelComboBox.SelectedIndex = 0;

			mOkButton.Text = Resources.LblOkButton;
			mCancelButton.Text = Resources.LblCancelButton;
		}

		#endregion

		#region Properties

		public bool UseAdbProvider
		{
			get
			{
				return mUseAdbProviderCheckBox.Checked;
			}
			set
			{
				mUseAdbProviderCheckBox.Checked = value;
			}
		}

		public bool IgnoreProductModel
		{
			get
			{
				return mIgnoreProductModelCheckBox.Checked;
			}
			set
			{
				mIgnoreProductModelCheckBox.Checked = value;
			}
		}

		public bool ShowHelp
		{
			get
			{
				return mShowHelpCheckBox.Checked;
			}
			set
			{
				mShowHelpCheckBox.Checked = value;
			}
		}

		public bool EnableLogging
		{
			get
			{
				return mEnableLoggingCheckBox.Checked;
			}
			set
			{
				mEnableLoggingCheckBox.Checked = value;
			}
		}

		public LogLevel.LogLevelInfo LogLevelInfo
		{
			get
			{
				return ((LogLevelEntry)mLogLevelComboBox.SelectedItem).LogLevelInfo;
			}
			set
			{
				LogLevelEntry foundLogLevel = null;

				foreach (LogLevelEntry currentEntry in mSelectableLogLevels)
				{
					if (currentEntry.LogLevelInfo.Priority == value.Priority)
					{
						foundLogLevel = currentEntry;
						break;
					}

					if (foundLogLevel == null)
					{
						if (currentEntry.LogLevelInfo.Priority > (foundLogLevel != null ? foundLogLevel.LogLevelInfo.Priority : -1)
								&& currentEntry.LogLevelInfo.Priority < value.Priority)
						{
							foundLogLevel = currentEntry;
						}
					}
				}

				mLogLevelComboBox.SelectedItem = foundLogLevel;
			}
		}

		#endregion

		#region Event handlers

		private void MEnableLoggingCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			mLogLevelComboBox.Enabled = mEnableLoggingCheckBox.Checked;
		}

		#endregion

		#region Inner classes

		private class LogLevelEntry
		{
			readonly string mLabel;
			readonly LogLevel.LogLevelInfo mLogLevelInfo;

			public LogLevelEntry(string label, LogLevel.LogLevelInfo logLevelInfo)
			{
				mLabel = label;
				mLogLevelInfo = logLevelInfo;
			}

			public string Label
			{
				get
				{
					return mLabel;
				}
			}

			public LogLevel.LogLevelInfo LogLevelInfo
			{
				get
				{
					return mLogLevelInfo;
				}
			}
		}

		#endregion


	}
}
