using Managed.Adb;
using System;
using System.Xml;

namespace CherryUpdater
{
	public class SubstringAfterProcessor : IProcessor
	{
		#region Constants

		private const string cLogTag = "SubstringAfterProcessor";

		#endregion

		#region Member variables

		private readonly string mMarker;

		#endregion

		#region Constructor

		private SubstringAfterProcessor(string marker)
		{
			mMarker = marker;
		}

		#endregion

		#region Public methods

		public string Process(string input, out bool stopProcessing)
		{
			stopProcessing = false;

			Log.V(cLogTag, "input: {0}", input);

			int flagIndex = input.IndexOf(mMarker);
			if (flagIndex == -1)
			{
				Log.V(cLogTag, "marker \"{0}\" not found", mMarker);
				Log.V(cLogTag, "output: {0}", input);
				return input;
			}

			string output = input.Substring(flagIndex + mMarker.Length);
			Log.V(cLogTag, "marker \"{0}\" found", mMarker);
			Log.V(cLogTag, "output: {0}", output);
			return output;
		}

		#endregion

		#region Inner classes

		public class Parser : IProcessorParser
		{
			#region Constants

			private const string cElementName = "substringAfter";

			#endregion

			#region Public methods

			public IProcessor CreateFromXML(XmlElement element)
			{
				if (element.Name != cElementName)
				{
					throw new ArgumentException(string.Format("argument must be a {0} element", cElementName), "element");
				}

				return new SubstringAfterProcessor(element.Attributes["marker"].Value);
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
