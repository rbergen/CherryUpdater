using System;
using System.Collections.Generic;
using System.Xml;

namespace CherryUpdater
{
	public class Query : MenuItem
	{
		#region Static variables

		private static readonly IDictionary<string, IProcessorParser> mProcessorParsers;

		#endregion

		#region Member variables

		private readonly string mCommand;
		private readonly IProcessor[] mProcessors;
		private readonly bool mOfferClipboardCopy = false;

		#endregion

		#region Static initializer

		static Query()
		{
			mProcessorParsers = new SortedDictionary<string, IProcessorParser>();
			AddProcessorParser(new CombineProcessor.Parser());
			AddProcessorParser(new FindLineProcessor.Parser());
			AddProcessorParser(new OutputIfContainsProcessor.Parser());
			AddProcessorParser(new OutputIfEqualProcessor.Parser());
			AddProcessorParser(new SubstringAfterProcessor.Parser());
			AddProcessorParser(new TranslateProcessor.Parser());
		}

		#endregion

		#region Constructor

		private Query(MenuItemBaseProperties baseProperties, string command, IProcessor[] processors, bool? offerClipboardCopy)
				: base(baseProperties)
		{
			mCommand = command;
			mProcessors = processors;
			if (offerClipboardCopy.HasValue)
			{
				mOfferClipboardCopy = offerClipboardCopy.Value;
			}
		}

		#endregion

		#region Public static methods

		public static Query CreateFromXML(XmlElement queryElement, Menu parentMenu)
		{
			if (queryElement.Name != "query")
			{
				throw new ArgumentException("argument must be a query element", "queryElement");
			}

			MenuItemBaseProperties baseProperties = new MenuItemBaseProperties();
			baseProperties.FillFromXml(queryElement, parentMenu);

			List<IProcessor> processorList = new List<IProcessor>();
			XmlElement processorsElement = queryElement["processors"];
			if (processorsElement != null)
			{
				string processorElementName;
				foreach (XmlNode node in processorsElement)
				{
					if (node is XmlElement element)
					{
						processorElementName = element.Name;
						if (mProcessorParsers.ContainsKey(processorElementName))
						{
							processorList.Add(mProcessorParsers[processorElementName].CreateFromXML(element));
						}
					}
				}
			}

			return new Query(baseProperties, queryElement.Attributes["command"].Value, processorList.ToArray(), TextParser.ParseNullableXmlBoolean(queryElement.Attributes["offerClipboardCopy"]));
		}

		#endregion

		#region Private static methods

		private static void AddProcessorParser(IProcessorParser parser)
		{
			mProcessorParsers[parser.ElementName] = parser;
		}

		#endregion

		#region Public methods

		public string Process(string text)
		{

			foreach (IProcessor processor in mProcessors)
			{
				text = processor.Process(text, out bool stopProcessing);

				if (stopProcessing)
				{
					break;
				}
			}

			return text;
		}

		#endregion

		#region Properties

		public string Command
		{
			get
			{
				return mCommand;
			}
		}

		public bool OfferClipboardCopy
		{
			get
			{
				return mOfferClipboardCopy;
			}
		}

		#endregion
	}
}
