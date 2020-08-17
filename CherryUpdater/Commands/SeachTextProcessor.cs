using System;
using System.Xml;

namespace CherryUpdater
{
	public class SearchTextProcessorBaseProperties
	{
		#region Constructor

		public SearchTextProcessorBaseProperties()
		{
			IgnoreCase = false;
		}

		#endregion

		#region Public methods

		public virtual void FillFromXml(XmlElement element)
		{
			XmlAttribute ignoreCaseAttribute = element.Attributes["ignoreCase"];
			if (ignoreCaseAttribute != null)
			{
				IgnoreCase = TextParser.ParseXmlBoolean(ignoreCaseAttribute);
			}

			SearchText = element.Attributes["searchText"].Value;
		}

		#endregion

		#region Properties

		public string SearchText
		{
			get;
			protected set;
		}

		public bool IgnoreCase
		{
			get;
			protected set;
		}

		#endregion
	}

	public abstract class SearchTextProcessor : IProcessor
	{
		#region Member variables

		private readonly SearchTextProcessorBaseProperties mBaseProperties;

		#endregion

		#region Constructor

		protected SearchTextProcessor(SearchTextProcessorBaseProperties baseProperties)
		{
			mBaseProperties = baseProperties ?? throw new ArgumentNullException("baseProperties");
		}

		#endregion

		#region Public abstract methods

		public abstract string Process(string input, out bool stopProcessing);

		#endregion

		#region Properties

		protected SearchTextProcessorBaseProperties BaseProperies
		{
			get
			{
				return mBaseProperties;
			}
		}

		protected string LogTag
		{
			get
			{
				return GetType().Name;
			}
		}

		#endregion
	}
}
