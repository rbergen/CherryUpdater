namespace Managed.Adb
{
	public class ConsoleOutputReceiver : MultiLineReceiver
	{
		private static ConsoleOutputReceiver _instance = null;
		public static ConsoleOutputReceiver Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ConsoleOutputReceiver();
				}
				return _instance;
			}
		}
		protected override void ProcessNewLines(string[] lines)
		{
			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
				{
					continue;
				}
			}
		}
	}
}
