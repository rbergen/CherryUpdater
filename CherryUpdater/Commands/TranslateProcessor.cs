using Managed.Adb;
using System;
using System.Collections.Generic;
using System.Xml;

namespace CherryUpdater
{
	public class TranslateProcessor : IProcessor
	{
		#region Constants

		private const string cLogTag = "TranslateProcessor";

		#endregion

		#region Member variables

		private readonly IDictionary<string, string> mItems;
		private readonly string mDefaultText;

		#endregion

		#region Constructor

		private TranslateProcessor(string defaultText) : this(null, defaultText) { }

		private TranslateProcessor(IDictionary<string, string> items, string defaultText)
		{
			if (items == null)
			{
				mItems = new SortedDictionary<string, string>();
			}
			else
			{
				mItems = items;
			}

			mDefaultText = defaultText;
		}

		#endregion

		#region Public methods

		public string Process(string input, out bool stopProcessing)
		{
			stopProcessing = false;

			Log.V(cLogTag, "input: {0}", input);

			string output;

			if (mItems.ContainsKey(input))
			{
				Log.V(cLogTag, "match found");
				output = mItems[input];
			}
			else if (mDefaultText != null)
			{
				Log.V(cLogTag, "match not found, using default text");
				output = mDefaultText.Replace("$$", input);
			}
			else
			{
				Log.V(cLogTag, "match not found and no default text available");
				output = input;
			}

			Log.V(cLogTag, "output: {0}", output);
			return output;
		}

		#endregion

		#region Properties

		private IDictionary<string, string> Items
		{
			get
			{
				return mItems;
			}
		}

		#endregion

		#region Inner classes

		public class Parser : IProcessorParser
		{
			#region Constants

			private const string cElementName = "translate";

			#endregion

			#region Public methods

			public IProcessor CreateFromXML(XmlElement element)
			{
				if (element.Name != cElementName)
				{
					throw new ArgumentException(string.Format("argument must be a {0} element", cElementName), "element");
				}

				XmlElement defaultTextElement = element["defaultText"];

				TranslateProcessor processor = new TranslateProcessor(defaultTextElement != null ? TextParser.ParseMultilineElementText(defaultTextElement.InnerText) ?? "" : null);

				foreach (XmlNode node in element)
				{
					if (node is XmlElement childElement && childElement.Name == "item")
					{
						processor.Items[childElement.Attributes["from"].Value] = TextParser.ParseMultilineElementText(childElement.InnerText);
					}
				}

				return processor;
			}

			#endregion

			#region Properties

			public string ElementName
			{
				get
				{
					return cElementName;
				}
			}

			#endregion
		}

		#endregion

	}
}
