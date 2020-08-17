using System;

namespace CherryUpdater
{
	public class MenuItemSelectedEventArgs : EventArgs
	{
		#region Member variables

		private readonly MenuItem mSelectedMenuItem;

		#endregion

		#region Constructor

		public MenuItemSelectedEventArgs(MenuItem selectedMenuItem)
		{
			mSelectedMenuItem = selectedMenuItem;
		}

		#endregion

		#region Properties

		public MenuItem SelectedMenuItem
		{
			get
			{
				return mSelectedMenuItem;
			}
		}

		#endregion
	}
}
