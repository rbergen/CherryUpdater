using CherryUpdater.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CherryUpdater
{
	public partial class MainForm : Form
	{
		#region Delegates

		private delegate void SingleParameterDelegate<T>(T parameter);

		#endregion

		#region Constants

		private const string cDriverPath64Bit = "pdanet\\PdaNetA4012.exe";
		private const string cDriverPath32Bit = cDriverPath64Bit;

		#endregion

		#region Static variables

		private static readonly Dictionary<bool, Type> mActivityProviderTypeMap;
		private static readonly Dictionary<bool, IActivityProvider> mActivityProviderMap;

		#endregion

		#region Member variables

		private Commands mCommands;
		private Menu mCurrentMenu;
		private int mShownItemsCount = 0;
		private readonly List<MenuItemControl> mMenuItemControls;
		private TaskProgressForm mTaskProgressForm;
		private ActivityExecutor mActivityExecutor;
		private readonly SplashDialog mSplashDialog;
		private ADBState? mStoredVerifyState;
		private ConfigurationDialog mSettingsDialog = null;
		private readonly int mIntrinsicHeight;
		private readonly int mMenuItemWidth;
		private bool mShowedIncompatibleProductModel = false;

		#endregion

		#region Static initializer

		static MainForm()
		{
			mActivityProviderTypeMap = new Dictionary<bool, Type>();
			mActivityProviderMap = new Dictionary<bool, IActivityProvider>();

			mActivityProviderTypeMap[false] = typeof(MadbActivityProvider);
			mActivityProviderTypeMap[true] = typeof(AdbExeActivityProvider);
		}

		#endregion

		#region Constructor

		public MainForm()
		{
			InitializeComponent();

			mMenuItemControls = new List<MenuItemControl>();

			mIntrinsicHeight = Height - mMenuGroupBox.Height;
			mMenuItemWidth = mFirstMenuItemControl.Width;

			mBackButton.Text = Resources.LblBackButton;
			mCloseButton.Text = Resources.LblCloseButton;
			mConfigurationMenuItem.Text = Resources.MnuConfiguration;
			mAboutMenuItem.Text = Resources.MnuAbout;

			mSplashDialog = new SplashDialog();
			mStoredVerifyState = null;
			mStatusLabel.Text = "";

			if (Settings.Default.UpgradeRequired)
			{
				Settings.Default.Upgrade();
				Settings.Default.UpgradeRequired = false;
				Settings.Default.Save();
			}
		}

		#endregion

		#region Public static methods

		public static bool IsEnvironmentSane(out string message)
		{
			if (!ActivityExecutor.IsEnvironmentSane(out message))
			{
				return false;
			}

			if (!File.Exists(GetCommandsPath()))
			{
				message = string.Format(Resources.MsgCommandsFileNotFound, GetCommandsPath());
				return false;
			}

			message = null;
			return true;
		}

		#endregion

		#region Private static methods

		private static string GetCommandsPath()
		{
			return "commands.xml";
		}

		private static IActivityProvider GetActivityProvider(bool key)
		{
			lock (mActivityProviderMap)
			{
				if (!mActivityProviderMap.ContainsKey(key))
				{
					mActivityProviderMap[key] = (IActivityProvider)Activator.CreateInstance(mActivityProviderTypeMap[key]);
				}

				return mActivityProviderMap[key];
			}
		}

		#endregion

		#region Private methods

		private void HandleDeviceConnection()
		{
			mStatusLabel.Text = mActivityExecutor.DeviceStatusText;

			CheckProductModelCompatibility();

			SetCurrentMenu(mCurrentMenu, mActivityExecutor.ProductModel);
		}

		private void SetCurrentMenu(Menu menu, string productModel)
		{
			int oldItemCount = mShownItemsCount;

			mCurrentMenu = menu;

			string groupBoxText = menu.Title;
			while (menu.ParentMenu != null)
			{
				menu = menu.ParentMenu;
				groupBoxText = menu.Title + " ► " + groupBoxText;
			}

			SuspendLayout();

			mMenuGroupBox.Text = groupBoxText;

			int controlIndex = 0;

			foreach (MenuItem menuItem in mCurrentMenu.MenuItems)
			{
				if (!menuItem.RequirementsValidated || (!Settings.Default.IgnoreProductModel && !menuItem.IsProductModelCompatible(productModel, mCommands.IsProductModelCompatible(productModel))))
				{
					continue;
				}

				if (controlIndex == mMenuItemControls.Count)
				{
					MenuItemControl menuItemControl = new MenuItemControl
					{
						Width = mMenuItemWidth,
						TabIndex = controlIndex
					};
					menuItemControl.MenuItemSelected += MMenuItemControl_MenuItemSelected;
					menuItemControl.ToolTip = mToolTip;
					mMenuItemControls.Add(menuItemControl);
				}

				mMenuItemControls[controlIndex].MenuItem = menuItem;
				mMenuItemControls[controlIndex].ShowHelp = Settings.Default.ShowHelp;

				if (controlIndex >= oldItemCount)
				{
					mMenuItemFlowPanel.Controls.Add(mMenuItemControls[controlIndex]);
				}

				controlIndex++;
			}

			mShownItemsCount = controlIndex;

			while (controlIndex < oldItemCount)
			{
				mMenuItemFlowPanel.Controls.Remove(mMenuItemControls[controlIndex]);
				controlIndex++;
			}

			mBackButton.TabIndex = mCurrentMenu.MenuItems.Length;
			mBackButton.Visible = mCurrentMenu.ParentMenu != null;
			mCloseButton.TabIndex = mBackButton.Visible ? mBackButton.TabIndex + 1 : mCurrentMenu.MenuItems.Length;

			ActiveControl = null;

			ResumeLayout(true);

			Height = mIntrinsicHeight + mMenuGroupBox.Height;

			PerformLayout();
		}

		private void UpdateLoggingSettings()
		{
			mActivityExecutor.EnableLogging = Settings.Default.EnableLogging;
			mActivityExecutor.LogLevel = Settings.Default.LogLevelInfo;
		}

		private void CheckProductModelCompatibility()
		{
			if (mActivityExecutor.IsDeviceConnected && !mCommands.IsProductModelCompatible(mActivityExecutor.ProductModel))
			{
				if (mShowedIncompatibleProductModel)
				{
					return;
				}

				mShowedIncompatibleProductModel = true;

				MessageBox.Show(this, Resources.MsgIncompatibleProductModel, Resources.CptIncompatibleProductModel, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				mShowedIncompatibleProductModel = false;
			}
		}

		private bool HandleFailedVerification(ADBState state, bool offerRetry)
		{
			switch (state)
			{
				case ADBState.CannotRun:
					MessageBox.Show(this, Resources.MsgCannotRun, Resources.CptConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				case ADBState.Error:
					MessageBox.Show(this, Resources.MsgAdbError, Resources.CptConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				case ADBState.MoreDevices:
					MessageBox.Show(this, Resources.MsgMoreDevices, Resources.CptConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				case ADBState.InvalidState:
					MessageBox.Show(this, Resources.MsgInvalidState, Resources.CptConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				case ADBState.NoRoot:
					MessageBox.Show(this, Resources.MsgNoRoot, Resources.CptConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				case ADBState.NoDevices:
					string driverPath = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") == "AMD64"
							|| Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432") == "AMD64"
							? cDriverPath64Bit : cDriverPath32Bit;

					if (File.Exists(driverPath))
					{
						if (MessageBox.Show(this, string.Format("{0}\n\n{1}", Resources.MsgNoDevices, Resources.MsgInstallDriverQuestion), Resources.CptConnectionError, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						{
							try
							{
								mActivityExecutor.ActivityProvider.Stop();
								mActivityExecutor.ActivityProvider.KillServer();
							}
							catch { }

							try
							{
								Process.Start(driverPath).WaitForExit();
							}
							catch { }

							mActivityExecutor.ActivityProvider.Start();

							if (offerRetry)
							{
								return MessageBox.Show(this, Resources.MsgDriverInstallationFinished, Resources.CptDriverInstallationFinished, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
							}
						}
					}
					else
					{
						MessageBox.Show(this, Resources.MsgNoDevices, Resources.CptConnectionError, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}

					return false;
			}

			return false;
		}

		private void HandleQueryException(Exception exception)
		{
			if (InvokeRequired)
			{
				Invoke(new SingleParameterDelegate<Exception>(HandleQueryException), exception);
				return;
			}

			Exception handlingException = null;
			string path = Path.Combine(Environment.CurrentDirectory, "exception.log");
			string text = Resources.MsgQueryFailed;

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

			text += Environment.NewLine + string.Format(Resources.MsgActivityExceptionOccured, exception.GetType().Name, exception.Message) + Environment.NewLine;

			if (handlingException == null)
			{
				text += string.Format(Resources.MsgDetailErrorLogWritten, path);
			}
			else
			{
				text += string.Format(Resources.MsgDetailErrorLogException, handlingException.GetType().Name, handlingException.Message);
			}

			MessageBox.Show(this, text, Resources.CptQueryFailed, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		#endregion

		#region Event handlers

		private void This_Load(object sender, EventArgs e)
		{
			mCommands = Commands.CreateFromXML(GetCommandsPath());
			mMenuItemFlowPanel.Controls.Remove(mFirstMenuItemControl);
			SetCurrentMenu(mCommands.MainMenu, null);
		}

		private void This_Shown(object sender, EventArgs e)
		{
			new Thread(delegate ()
			{
				mActivityExecutor = new ActivityExecutor
				{
					DeviceNameProvider = mCommands
				};
				UpdateLoggingSettings();
				mActivityExecutor.ActivityProvider = GetActivityProvider(Settings.Default.UseAdbProvider);
				mActivityExecutor.VerifyActivityFailed += MActivityExecutor_VerifyActivityFailed;

				for (int attempt = 0; attempt < 5; attempt++)
				{
					Thread.Sleep(1000);

					if (mActivityExecutor.GetConnectionReady())
					{
						break;
					}
				}

				mActivityExecutor.ExecuteActivities(mCommands.StartActivities);
				Invoke(new MethodInvoker(delegate ()
				{
					mSplashDialog.ShowInitializeDone();
				}));
			}).Start();

			mSplashDialog.ShowDialog(this);

			HandleDeviceConnection();

			mActivityExecutor.DevicesStateChanged += MActivityExecutor_DevicesStateChanged;

			if (mStoredVerifyState != null)
			{
				HandleFailedVerification(mStoredVerifyState.Value, false);
				mStoredVerifyState = null;
			}
		}

		private void This_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (mActivityExecutor != null)
			{
				mActivityExecutor.VerifyActivityFailed -= MActivityExecutor_VerifyActivityFailed;
				mActivityExecutor.ExecuteActivities(mCommands.ExitActivities);
				mActivityExecutor.Dispose();
			}
		}

		private void MActivityExecutor_DevicesStateChanged(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler<EventArgs>(MActivityExecutor_DevicesStateChanged), sender, e);
				return;
			}

			HandleDeviceConnection();
		}

		private void MActivityExecutor_VerifyActivityFailed(object sender, VerifyActivityFailedEventArgs eventArgs)
		{
			if (InvokeRequired)
			{
				Invoke(new EventHandler<VerifyActivityFailedEventArgs>(MActivityExecutor_VerifyActivityFailed), sender, eventArgs);
				return;
			}

			if (mSplashDialog.Visible)
			{
				mStoredVerifyState = eventArgs.ADBState;
			}
			else if (HandleFailedVerification(eventArgs.ADBState, true))
			{
				eventArgs.SetRetryVerification();
			}
		}

		private void MMenuItemControl_MenuItemSelected(object sender, MenuItemSelectedEventArgs eventArgs)
		{
			MenuItem selectedItem = eventArgs.SelectedMenuItem;

			DecoratedMessage preMessage = selectedItem.PreMessage;

			if (preMessage != null && !(preMessage.ShowOnce && preMessage.Shown))
			{
				DialogResult preMessageResult = MessageBox.Show(this, preMessage.Message, preMessage.Caption, preMessage.Buttons, preMessage.Type);

				preMessage.Shown = true;

				if (preMessageResult == DialogResult.No || preMessageResult == DialogResult.Cancel)
				{
					return;
				}
			}

			if (selectedItem is Menu menu)
			{
				SetCurrentMenu(menu, mActivityExecutor.ProductModel);
			}
			else if (selectedItem is Task task)
			{
				if (mTaskProgressForm == null)
				{
					mTaskProgressForm = new TaskProgressForm
					{
						ActivityExecutor = mActivityExecutor
					};
				}

				mTaskProgressForm.Task = task;
				mTaskProgressForm.ShowDialog(this);

				if (selectedItem.ParentMenu.ReturnToMainMenu)
				{
					SetCurrentMenu(mCommands.MainMenu, mActivityExecutor.ProductModel);
				}
			}
			else
			{
				try
				{
					Query query = (Query)selectedItem;
					QueryResult result = mActivityExecutor.ExecuteQuery(query);
					if (!result.Success)
					{
						MessageBox.Show(this, result.Message, Resources.CptQueryFailed, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					if (query.OfferClipboardCopy)
					{
						if (MessageBox.Show(this, string.Format(Resources.MsgQueryOfferClipboardCopy, result.Message), Resources.CptQueryCompleted, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							Clipboard.SetData(DataFormats.Text, result.Message);
						}
					}
					else
					{
						MessageBox.Show(this, result.Message, Resources.CptQueryCompleted, MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
				catch (Exception exception)
				{
					HandleQueryException(exception);
				}
			}
		}

		private void MBackButton_Click(object sender, EventArgs e)
		{
			if (mCurrentMenu.ParentMenu != null)
			{
				SetCurrentMenu(mCurrentMenu.ParentMenu, mActivityExecutor.ProductModel);
			}
		}

		private void MCloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void MConfigurationMenuItem_Click(object sender, EventArgs e)
		{
			if (mSettingsDialog == null)
			{
				mSettingsDialog = new ConfigurationDialog();
			}

			mSettingsDialog.UseAdbProvider = Settings.Default.UseAdbProvider;
			mSettingsDialog.IgnoreProductModel = Settings.Default.IgnoreProductModel;
			mSettingsDialog.EnableLogging = Settings.Default.EnableLogging;
			mSettingsDialog.LogLevelInfo = Settings.Default.LogLevelInfo;
			mSettingsDialog.ShowHelp = Settings.Default.ShowHelp;

			if (mSettingsDialog.ShowDialog() == DialogResult.OK)
			{
				Settings.Default.UseAdbProvider = mSettingsDialog.UseAdbProvider;
				Settings.Default.IgnoreProductModel = mSettingsDialog.IgnoreProductModel;
				Settings.Default.EnableLogging = mSettingsDialog.EnableLogging;
				Settings.Default.LogLevelInfo = mSettingsDialog.LogLevelInfo;
				Settings.Default.ShowHelp = mSettingsDialog.ShowHelp;
				Settings.Default.Save();

				UpdateLoggingSettings();

				Enabled = false;
				UseWaitCursor = true;
				mStatusLabel.Text = Resources.LblStatusApplyingSettings;

				new Thread(delegate ()
				{
					mActivityExecutor.ActivityProvider = GetActivityProvider(Settings.Default.UseAdbProvider);

					Invoke(new MethodInvoker(delegate ()
									{
							SetCurrentMenu(mCurrentMenu, mActivityExecutor.ProductModel);
							mStatusLabel.Text = mActivityExecutor.DeviceStatusText;
							UseWaitCursor = false;
							Enabled = true;
						}));
				}).Start();
			}
		}

		private void MAboutMenuItem_Click(object sender, EventArgs e)
		{
			mSplashDialog.ShowDialog();
		}

		#endregion
	}
}
