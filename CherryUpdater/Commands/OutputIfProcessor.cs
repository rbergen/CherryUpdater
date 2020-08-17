using Managed.Adb;
using System.Xml;

namespace CherryUpdater
{
	public class OutputIfProcessorBaseProperties : SearchTextProcessorBaseProperties
	{
		#region Public methods

		public override void FillFromXml(XmlElement element)
		{
			base.FillFromXml(element);

			Message = TextParser.ParseMultilineElementText(element.InnerText);
		}

		#endregion

		#region Properties

		public string Message
		{
			get;
			protected set;
		}

		#endregion
	}

	public abstract class OutputIfProcessor : SearchTextProcessor
	{
		#region Constructor

		protected OutputIfProcessor(OutputIfProcessorBaseProperties baseProperties) : base(baseProperties) { }

		#endregion

		#region Public methods

		public override string Process(string input, out bool stopProcessing)
		{
			Log.V(LogTag, "input: {0}", input);

			if (IsMatch(input))
			{
				Log.V(LogTag, "match found for search text \"{0}\"{1}", BaseProperies.SearchText, BaseProperies.IgnoreCase ? ", ignoring case" : "");
				stopProcessing = true;

				string output = ((OutputIfProcessorBaseProperties)BaseProperies).Message.Replace("$$", input);

				Log.V(LogTag, "output: {0}", output);
				Log.V(LogTag, "stopping processing");
				return output;
			}

			stopProcessing = false;
			Log.V(LogTag, "no match found for search text \"{0}\"{1}", BaseProperies.SearchText, BaseProperies.IgnoreCase ? ", ignoring case" : "");
			Log.V(LogTag, "output: {0}", input);

			return input;
		}

		#endregion

		#region Protected abstract methods

		protected abstract bool IsMatch(string input);

		#endregion
	}
}
