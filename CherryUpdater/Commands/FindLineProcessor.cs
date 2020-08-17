using Managed.Adb;
using System;
using System.Xml;

namespace CherryUpdater
{
	public class FindLineProcessor : SearchTextProcessor
	{
		#region Constructor

		private FindLineProcessor(SearchTextProcessorBaseProperties baseProperties) : base(baseProperties) { }

		#endregion

		#region Public methods

		public override string Process(string input, out bool stopProcessing)
		{
			stopProcessing = false;

			Log.V(LogTag, "input: {0}", input);

			string[] lines = input.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			string trimmedLine;

			foreach (string line in lines)
			{
				trimmedLine = line.Trim();
				if (BaseProperies.IgnoreCase ? trimmedLine.IndexOf(BaseProperies.SearchText, StringComparison.OrdinalIgnoreCase) != -1 : trimmedLine.Contains(BaseProperies.SearchText))
				{
					Log.V(LogTag, "searchtext \"{0}\" found{1}", BaseProperies.SearchText, BaseProperies.IgnoreCase ? ", ignoring case" : "");
					Log.V(LogTag, "output: {0}", trimmedLine);
					return trimmedLine;
				}
			}

			Log.V(LogTag, "searchtext \"{0}\" not found{1}", BaseProperies.SearchText, BaseProperies.IgnoreCase ? ", ignoring case" : "");
			Log.V(LogTag, "output:");
			return "";
		}

		#endregion

		#region Inner classes

		public class Parser : IProcessorParser
		{
			#region Constants

			private const string cElementName = "findLine";

			#endregion

			#region Public methods

			public IProcessor CreateFromXML(XmlElement element)
			{
				if (element.Name != cElementName)
				{
					throw new ArgumentException(string.Format("argument must be a {0} element", cElementName), "element");
				}

				SearchTextProcessorBaseProperties baseProperties = new SearchTextProcessorBaseProperties();
				baseProperties.FillFromXml(element);

				return new FindLineProcessor(baseProperties);
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
