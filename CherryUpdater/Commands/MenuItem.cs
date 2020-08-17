using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace CherryUpdater
{
	public class MenuItemBaseProperties
	{
		#region Public methods

		public virtual void FillFromXml(XmlElement element, Menu parentMenu)
		{
			ParentMenu = parentMenu;

			XmlAttribute compatibleProductModelsAttribute = element.Attributes["compatibleProductModels"];
			string[] compatibleProductModels = null;
			if (compatibleProductModelsAttribute != null)
			{
				compatibleProductModels = TextParser.ParseProductModels(compatibleProductModelsAttribute.Value);
			}

			CompatibleProductModels = compatibleProductModels;

			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
			namespaceManager.AddNamespace("cf", "http://schema.vanbergen.local/Tools/CherryUpdater/CommandFile");

			List<Requirement> requirementList = new List<Requirement>();
			XmlNodeList requireNodes = element.SelectNodes("child::cf:require", namespaceManager);
			foreach (XmlElement requireElement in requireNodes)
			{
				requirementList.Add(Requirement.CreateFromXML(requireElement));
			}

			Requirements = requirementList.Count > 0 ? requirementList.ToArray() : null;

			XmlElement helpTextElement = element["helpText"];
			string helpText = null;
			if (helpTextElement != null)
			{
				helpText = TextParser.ParseMultilineElementText(helpTextElement.InnerText.Trim());
			}

			HelpText = helpText;

			XmlElement preMessageElement = element["preMessage"];
			if (preMessageElement != null)
			{
				MessageBoxButtons? showButtons = null;

				XmlAttribute showButtonsAttribute = preMessageElement.Attributes["showButtons"];
				if (showButtonsAttribute != null)
				{
					switch (showButtonsAttribute.Value)
					{
						case "ok":
							showButtons = MessageBoxButtons.OK;
							break;

						case "okCancel":
							showButtons = MessageBoxButtons.OKCancel;
							break;

						case "yesNo":
							showButtons = MessageBoxButtons.YesNo;
							break;
					}
				}

				MessageBoxIcon? messageTypeIcon = null;

				XmlAttribute messageTypeAttribute = preMessageElement.Attributes["messageType"];
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

						case "question":
							messageTypeIcon = MessageBoxIcon.Question;
							break;

						case "warning":
							messageTypeIcon = MessageBoxIcon.Warning;
							break;
					}
				}

				PreMessage = new DecoratedMessage(TextParser.ParseMultilineElementText(preMessageElement.InnerText.Trim()), preMessageElement.Attributes["caption"].Value,
						TextParser.ParseNullableXmlBoolean(preMessageElement.Attributes["showOnce"]), false, showButtons, messageTypeIcon);
			}

			Title = element.Attributes["title"].Value;

		}

		#endregion

		#region Properties

		public string Title
		{
			get;
			protected set;
		}

		public Menu ParentMenu
		{
			get;
			protected set;
		}

		public string[] CompatibleProductModels
		{
			get;
			protected set;
		}

		public string HelpText
		{
			get;
			protected set;
		}

		public Requirement[] Requirements
		{
			get;
			protected set;
		}

		public DecoratedMessage PreMessage
		{
			get;
			protected set;
		}

		#endregion

	}

	public abstract class MenuItem
	{
		#region Member variables

		private readonly MenuItemBaseProperties mBaseProperties;
		private bool mIsValidated;
		private bool mRequirementsValidated;

		#endregion

		#region Constructor

		protected MenuItem(MenuItemBaseProperties baseProperties)
		{
			mBaseProperties = baseProperties ?? throw new ArgumentNullException("baseProperties");
		}

		#endregion

		#region Public methods

		public virtual void ValidateRequirements()
		{
			bool requirementsValidated = true;

			if (mBaseProperties.Requirements != null)
			{
				foreach (Requirement requirement in mBaseProperties.Requirements)
				{
					if (!requirement.Validate())
					{
						requirementsValidated = false;
						break;
					}
				}
			}

			mRequirementsValidated = requirementsValidated;
			mIsValidated = true;
		}

		#endregion

		#region Internal methods

		internal bool IsProductModelCompatible(string productModel, bool isKnownDevice)
		{
			return !isKnownDevice || mBaseProperties.CompatibleProductModels == null || Array.IndexOf<string>(mBaseProperties.CompatibleProductModels, productModel) != -1;
		}

		#endregion

		#region Properties

		public string Title
		{
			get
			{
				return mBaseProperties.Title;
			}
		}

		public Menu ParentMenu
		{
			get
			{
				return mBaseProperties.ParentMenu;
			}
		}

		public string[] CompatibleProductModels
		{
			get
			{
				return mBaseProperties.CompatibleProductModels;
			}
		}

		public virtual bool RequirementsValidated
		{
			get
			{
				if (!mIsValidated)
				{
					ValidateRequirements();
				}

				return mRequirementsValidated;
			}
		}

		public string HelpText
		{
			get
			{
				return mBaseProperties.HelpText;
			}
		}

		public DecoratedMessage PreMessage
		{
			get
			{
				return mBaseProperties.PreMessage;
			}
		}

		#endregion
	}
}
