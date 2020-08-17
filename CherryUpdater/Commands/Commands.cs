using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CherryUpdater
{
	public class Commands : IDeviceNameProvider
	{
		#region Member variables

		private readonly IDictionary<string, string> mDeviceMap;
		private readonly string[] mStartActivities;
		private readonly string[] mExitActivities;
		private readonly string[] mCompatibleProductModels;
		private readonly Menu mMainMenu;

		#endregion

		#region Constructor

		private Commands(IDictionary<string, string> deviceMap, string[] startActivities, string[] exitActivities, Menu mainMenu, string[] compatibleProductModels)
		{
			mDeviceMap = deviceMap;
			mStartActivities = startActivities;
			mExitActivities = exitActivities;
			mMainMenu = mainMenu ?? throw new ArgumentNullException("mainMenu");
			mCompatibleProductModels = compatibleProductModels;
		}

		#endregion

		#region Public static methods

		public static Commands CreateFromXML(XmlElement commandsElement)
		{
			if (commandsElement.Name != "commands")
			{
				throw new ArgumentException("argument must be a commands element", "commandsElement");
			}

			IDictionary<string, string> deviceMap = new SortedDictionary<string, string>();
			XmlElement deviceMapElement = commandsElement["deviceMap"];
			if (deviceMapElement != null)
			{
				foreach (XmlNode node in deviceMapElement)
				{
					if (node is XmlElement entryElement)
					{
						deviceMap[entryElement.Attributes["model"].Value] = entryElement.Attributes["name"].Value;
					}
				}
			}

			XmlElement startActivitiesNode = commandsElement["startupActivities"];
			string[] startActivities = null;
			if (startActivitiesNode != null)
			{
				startActivities = TextParser.ParseActivities(startActivitiesNode.InnerText);
			}

			XmlElement exitActivitiesNode = commandsElement["exitActivities"];
			string[] exitActivities = null;
			if (exitActivitiesNode != null)
			{
				exitActivities = TextParser.ParseActivities(exitActivitiesNode.InnerText);
			}

			XmlAttribute compatibleProductModelsAttribute = commandsElement.Attributes["compatibleProductModels"];
			string[] compatibleProductModels = null;
			if (compatibleProductModelsAttribute != null)
			{
				compatibleProductModels = TextParser.ParseProductModels(compatibleProductModelsAttribute.Value);
			}

			return new Commands(deviceMap, startActivities, exitActivities, Menu.CreateFromXML(commandsElement["menu"], null), compatibleProductModels);
		}


		public static Commands CreateFromXML(string configFilePath)
		{
			XmlReaderSettings readerSettings = new XmlReaderSettings();
			readerSettings.Schemas.Add(null, XmlReader.Create(new StringReader(Resources.XsdCommandFile)));
			readerSettings.ValidationType = ValidationType.Schema;

			XmlReader reader = XmlReader.Create(configFilePath, readerSettings);

			XmlDocument configDocument = new XmlDocument();
			configDocument.Load(reader);

			return CreateFromXML(configDocument.DocumentElement);
		}

		#endregion

		#region Public methods

		public string GetDeviceName(string model)
		{
			if (model == null || mDeviceMap == null)
			{
				return model;
			}

			return mDeviceMap.ContainsKey(model) ? mDeviceMap[model] : model;
		}

		#endregion

		#region Internal methods

		internal bool IsProductModelCompatible(string productModel)
		{
			return mCompatibleProductModels == null || Array.IndexOf<string>(mCompatibleProductModels, productModel) != -1;
		}

		#endregion

		#region Properties

		public string[] StartActivities
		{
			get
			{
				return mStartActivities;
			}
		}

		public string[] ExitActivities
		{
			get
			{
				return mExitActivities;
			}
		}

		public Menu MainMenu
		{
			get
			{
				return mMainMenu;
			}
		}

		public string[] CompatibleProductModels
		{
			get
			{
				return mCompatibleProductModels;
			}
		}

		#endregion
	}
}
