using System;
using System.Windows.Forms;
using System.Xml;

namespace CherryUpdater
{
	public class Task : MenuItem
	{
		#region Member variables

		private readonly string[] mActivities;
		private readonly bool mContinueOnError = false;
		private readonly DecoratedMessage mPostMessage;

		#endregion

		#region Constructor

		private Task(MenuItemBaseProperties baseProperties, string progressCaption, string[] activities, bool? continueOnError, DecoratedMessage postMessage)
				: base(baseProperties)
		{
			ProgressCaption = progressCaption ?? throw new ArgumentNullException("progressCaption");
			mActivities = activities ?? throw new ArgumentNullException("activities");

			if (continueOnError != null)
			{
				mContinueOnError = continueOnError.Value;
			}

			mPostMessage = postMessage;
		}

		#endregion

		#region Public static methods

		public static Task CreateFromXML(XmlElement taskElement, Menu parentMenu)
		{
			if (taskElement.Name != "task")
			{
				throw new ArgumentException("argument must be a task element", "taskElement");
			}

			MenuItemBaseProperties baseProperties = new MenuItemBaseProperties();
			baseProperties.FillFromXml(taskElement, parentMenu);

			DecoratedMessage postMessage = null;

			XmlElement postMessageElement = taskElement["postMessage"];
			if (postMessageElement != null)
			{
				MessageBoxIcon? messageTypeIcon = null;

				XmlAttribute messageTypeAttribute = postMessageElement.Attributes["messageType"];
				if (messageTypeAttribute != null)
				{
					switch (messageTypeAttribute.Value)
					{
						case "neutral":
							messageTypeIcon = MessageBoxIcon.None;
							break;

						case "information":
							messageTypeIcon = MessageBoxIcon.Information;
							break;

						case "warning":
							messageTypeIcon = MessageBoxIcon.Warning;
							break;
					}
				}

				postMessage = new DecoratedMessage(TextParser.ParseMultilineElementText(postMessageElement.InnerText.Trim()), postMessageElement.Attributes["caption"].Value,
						false, TextParser.ParseNullableXmlBoolean(postMessageElement.Attributes["showIfFailed"]), null, messageTypeIcon);
			}


			return new Task(baseProperties, taskElement.Attributes["progressCaption"].Value,
					TextParser.ParseActivities(taskElement["activities"].InnerText), TextParser.ParseNullableXmlBoolean(taskElement.Attributes["continueOnError"]), postMessage);
		}

		#endregion

		#region Properties

		public string ProgressCaption { get; }

		public string[] Activities
		{
			get
			{
				return mActivities;
			}
		}

		public bool ContinueOnError
		{
			get
			{
				return mContinueOnError;
			}
		}

		public DecoratedMessage PostMessage
		{
			get
			{
				return mPostMessage;
			}
		}

		#endregion
	}
}
