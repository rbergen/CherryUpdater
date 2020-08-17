using Managed.Adb;
using System;
using System.Xml;

namespace CherryUpdater
{
	public class CombineProcessor : IProcessor
	{
		#region Constants

		private const string cLogTag = "CombineProcessor";

		#endregion

		#region Member variables

		private readonly string mMessage;

		#endregion

		#region Constructor

		private CombineProcessor(string message)
		{
			mMessage = message;
		}

		#endregion

		#region Public methods

		public string Process(string input, out bool stopProcessing)
		{
			stopProcessing = false;

			Log.V(cLogTag, "input: {0}", input);

			string output;

			if (mMessage.Contains("$$"))
			{
				Log.V(cLogTag, "placeholder found, replacing with input");
				output = mMessage.Replace("$$", input);
			}
			else
			{
				Log.V(cLogTag, "placeholder not found, appending input");
				output = mMessage + input;
			}

			Log.V(cLogTag, "output: {0}", output);
			return output;

		}

		#endregion

		#region Inner classes

		public class Parser : IProcessorParser
		{
			#region Constants

			private const string cElementName = "combine";

			#endregion

			#region Public methods

			public IProcessor CreateFromXML(XmlElement element)
			{
				if (element.Name != cElementName)
				{
					throw new ArgumentException(string.Format("argument must be a {0} element", cElementName), "element");
				}

				return new CombineProcessor(TextParser.ParseMultilineElementText(element.InnerText));
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
