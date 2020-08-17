
using System.Text;

namespace Managed.Adb
{
	internal class CommandErrorReceiver : MultiLineReceiver
	{
		public CommandErrorReceiver()
		{
			ErrorMessage = null;
		}

		protected override void ProcessNewLines(string[] lines)
		{
			StringBuilder message = new StringBuilder();
			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("$"))
				{
					continue;
				}

				message.AppendLine(line);
			}

			if (message.Length > 0)
			{
				ErrorMessage = message.ToString();
			}
		}

		public string ErrorMessage { get; set; }
	}
}
