using System;
using System.Xml;

namespace CherryUpdater
{
	public class OutputIfEqualProcessor : OutputIfProcessor
	{
		#region Constructor

		private OutputIfEqualProcessor(OutputIfProcessorBaseProperties baseProperties) : base(baseProperties) { }

		#endregion

		#region Protected methods

		protected override bool IsMatch(string input)
		{
			return BaseProperies.IgnoreCase ? input.Equals(BaseProperies.SearchText, StringComparison.OrdinalIgnoreCase) : input == BaseProperies.SearchText;
		}

		#endregion

		#region Inner classes

		public class Parser : IProcessorParser
		{
			#region Constants

			private const string cElementName = "outputIfEqual";

			#endregion

			#region Public methods

			public IProcessor CreateFromXML(XmlElement element)
			{
				if (element.Name != cElementName)
				{
					throw new ArgumentException(string.Format("argument must be a {0} element", cElementName), "element");
				}

				OutputIfProcessorBaseProperties baseProperties = new OutputIfProcessorBaseProperties();

				baseProperties.FillFromXml(element);

				return new OutputIfEqualProcessor(baseProperties);
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
