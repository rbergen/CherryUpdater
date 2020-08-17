using System.Xml;

namespace CherryUpdater
{
	public interface IProcessor
	{
		string Process(string input, out bool stopProcessing);
	}

	public interface IProcessorParser
	{
		IProcessor CreateFromXML(XmlElement element);

		string ElementName { get; }
	}
}
