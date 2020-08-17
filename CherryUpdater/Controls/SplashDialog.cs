using System;
using System.Reflection;
using System.Windows.Forms;

namespace CherryUpdater
{
	public partial class SplashDialog : Form
	{
		#region Constructor

		public SplashDialog()
		{
			InitializeComponent();

			mCreditLabel1.Text = Resources.LblSplashCredits1;
			mCreditLabel2.Text = Resources.LblSplashCredits2;
			mDisclaimerLabel.Text = Resources.LblSplashDisclaimer;
			mInitializingLabel.Text = Resources.LblSplashInitializing;
			mOKButton.Text = Resources.LblOkButton;

			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			object[] customAttributes = executingAssembly.GetCustomAttributes(false);

			string title = null;
			string product = null;
			string copyright = null;

			foreach (object attribute in customAttributes)
			{
				if (title == null && attribute is AssemblyTitleAttribute titleAttribute)
				{
					title = titleAttribute.Title;
				}
				else if (copyright == null && attribute is AssemblyCopyrightAttribute assemblyCopyrightAttribute)
				{
					copyright = assemblyCopyrightAttribute.Copyright;
				}
				else if (product == null && attribute is AssemblyProductAttribute assemblyProductAttribute)
				{
					product = assemblyProductAttribute.Product;
				}

				if (title != null && copyright != null && product != null)
				{
					break;
				}
			}

			Version version = executingAssembly.GetName().Version;
			product += " " + version.ToString(2);

			Text = title;
			mProductLabel.Text = product;
			mVersionLabel.Text = string.Format(Resources.LblSplashVersion, version.ToString());
			mCopyrightLabel.Text = copyright;
		}

		#endregion

		#region Public methods

		public void ShowInitializeDone()
		{
			mInitializingLabel.Text = "";
			mOKButton.Enabled = true;
		}

		#endregion

		#region Event handlers

		private void MCreditLabel1_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, Resources.MsgCreditsNameList, Resources.CptCreditsNameList, MessageBoxButtons.OK, MessageBoxIcon.None);
		}

		#endregion
	}
}
