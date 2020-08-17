using System;
using System.Collections.Generic;
using System.Xml;

namespace CherryUpdater
{
	public class Menu : MenuItem
	{
		#region Member variables

		private MenuItem[] mMenuItems;
		private readonly bool mReturnToMainMenu = false;

		#endregion

		#region Constructor

		private Menu(MenuItemBaseProperties baseProperties, bool? returnToMainMenu)
				: base(baseProperties)
		{
			if (returnToMainMenu.HasValue)
			{
				mReturnToMainMenu = returnToMainMenu.Value;
			}
		}

		#endregion

		#region Public static methods

		public static Menu CreateFromXML(XmlElement menuElement, Menu parentMenu)
		{
			if (menuElement.Name != "menu")
			{
				throw new ArgumentException("argument must be a menu element", "menuElement");
			}

			MenuItemBaseProperties baseProperties = new MenuItemBaseProperties();
			baseProperties.FillFromXml(menuElement, parentMenu);

			Menu menu = new Menu(baseProperties, TextParser.ParseNullableXmlBoolean(menuElement.Attributes["returnToMainMenu"]));

			List<MenuItem> menuItems = new List<MenuItem>();

			foreach (XmlNode node in menuElement)
			{
				if (node is XmlElement element)
				{
					switch (element.Name)
					{
						case "menu":
							menuItems.Add(CreateFromXML(element, menu));
							break;
						case "query":
							menuItems.Add(Query.CreateFromXML(element, menu));
							break;
						case "task":
							menuItems.Add(Task.CreateFromXML(element, menu));
							break;
					}
				}
			}

			menu.MenuItems = menuItems.ToArray();

			return menu;
		}

		#endregion

		#region Properties

		public MenuItem[] MenuItems
		{
			get
			{
				return mMenuItems;
			}
			private set
			{
				mMenuItems = value;
			}
		}

		public bool ReturnToMainMenu
		{
			get
			{
				return mReturnToMainMenu;
			}
		}

		#endregion
	}
}
