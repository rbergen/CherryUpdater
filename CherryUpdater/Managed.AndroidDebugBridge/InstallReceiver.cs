using System.Text.RegularExpressions;

namespace Managed.Adb
{
	public class InstallReceiver : MultiLineReceiver
	{
		/// <summary>
		/// 
		/// </summary>
		private const string SUCCESS_OUTPUT = "Success";
		/// <summary>
		/// 
		/// </summary>
		private const string FAILURE_PATTERN = @"Failure(?:\s+\[(.*)\])?";


		private const string UNKNOWN_ERROR = "An unknown error occured.";
		/// <summary>
		/// Processes the new lines.
		/// </summary>
		/// <param name="lines">The lines.</param>
		protected override void ProcessNewLines(string[] lines)
		{
			foreach (string line in lines)
			{
				if (line.Length > 0)
				{
					if (line.StartsWith(SUCCESS_OUTPUT))
					{
						ErrorMessage = null;
					}
					else
					{
						Match m = Regex.Match(line, FAILURE_PATTERN);
						if (m.Success)
						{
							string msg = m.Groups[1].Value;
							ErrorMessage = string.IsNullOrEmpty(msg) ? UNKNOWN_ERROR : msg;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the error message if the install was unsuccessful.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage { get; private set; }
	}
}
