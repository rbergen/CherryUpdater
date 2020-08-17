using System;
using System.Windows.Forms;

namespace CherryUpdater
{
	public partial class MenuItemControl : UserControl
	{
		#region Events

		public event EventHandler<MenuItemSelectedEventArgs> MenuItemSelected;

		#endregion

		#region Member variables

		private MenuItem mMenuItem;
		private ToolTip mToolTip;
		private bool mShowHelp;

		#endregion

		#region Constructor

		public MenuItemControl()
		{
			InitializeComponent();
		}

		public MenuItemControl(MenuItem menuItem)
				: this()
		{
			mMenuItem = menuItem;
		}

		#endregion

		#region Private methods

		private void SetToolTipText()
		{
			if (mToolTip == null)
			{
				return;
			}

			mToolTip.SetToolTip(mQuestionLabel, mMenuItem?.HelpText);
		}

		private void SetQuestionLabelVisibility()
		{
			mQuestionLabel.Visible = mShowHelp && mMenuItem != null && mMenuItem.HelpText != null;
		}

		#endregion

		#region Properties

		public ToolTip ToolTip
		{
			get
			{
				return mToolTip;
			}
			set
			{
				if (value == null)
				{
					SetToolTipText();
				}

				mToolTip = value;

				SetToolTipText();
			}
		}

		public MenuItem MenuItem
		{
			get
			{
				return mMenuItem;
			}
			set
			{
				mMenuItem = value;

				if (mMenuItem != null)
				{
					mTitleLabel.Text = "●  " + mMenuItem.Title;
					if (mMenuItem is Menu)
					{
						mExecuteButton.Text = Resources.LblMenuButton;
					}
					else if (mMenuItem is Task)
					{
						mExecuteButton.Text = Resources.LblTaskButton;
					}
					else
					{
						mExecuteButton.Text = Resources.LblQueryButton;
					}
					mExecuteButton.Enabled = true;
				}
				else
				{
					mTitleLabel.Text = "";
					mExecuteButton.Text = "";
					mExecuteButton.Enabled = false;
				}

				SetQuestionLabelVisibility();
				SetToolTipText();
			}
		}

		public bool ShowHelp
		{
			get
			{
				return mShowHelp;
			}
			set
			{
				mShowHelp = value;
				SetQuestionLabelVisibility();
			}
		}

		#endregion

		#region Event raisers

		protected void OnMenuItemSelected(MenuItemSelectedEventArgs menuItemSelectedEventArgs)
		{
			MenuItemSelected?.Invoke(this, menuItemSelectedEventArgs);
		}

		#endregion

		#region Event handlers

		private void MExecuteButtonClicked(object sender, EventArgs e)
		{
			OnMenuItemSelected(new MenuItemSelectedEventArgs(mMenuItem));
		}

		#endregion
	}
}
