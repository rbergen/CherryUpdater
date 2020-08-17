namespace CherryUpdater
{
	using System;
	using System.Xml;

	public class OutputIfContainsProcessor : OutputIfProcessor
	{
		#region Constructor

		private OutputIfContainsProcessor(OutputIfProcessorBaseProperties baseProperties) : base(baseProperties) { }

		#endregion

		#region Protected methods

		protected override bool IsMatch(string input)
		{
			return BaseProperies.IgnoreCase ? input.IndexOf(BaseProperies.SearchText, StringComparison.OrdinalIgnoreCase) != -1 : input.Contains(BaseProperies.SearchText);
		}

		#endregion

		#region Inner classes

		public class Parser : IProcessorParser
		{
			#region Constants

			private const string cElementName = "outputIfContains";

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

				return new OutputIfContainsProcessor(baseProperties);
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

