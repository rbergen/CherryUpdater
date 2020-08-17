using System;
using System.Text.RegularExpressions;

namespace Managed.Adb
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class EnvironmentVariablesReceiver : MultiLineReceiver
	{
		public const string ENV_COMMAND = "printenv";
		private const string ENV_PATTERN = @"^([^=\s]+)\s*=\s*(.*)$";
		/// <summary>
		/// Initializes a new instance of the <see cref="EnvironmentVariablesReceiver"/> class.
		/// </summary>
		/// <param name="device">The device.</param>
		public EnvironmentVariablesReceiver(Device device)
		{
			Device = device;
		}

		/// <summary>
		/// Gets or sets the device.
		/// </summary>
		/// <value>The device.</value>
		public Device Device { get; private set; }

		/// <summary>
		/// Processes the new lines.
		/// </summary>
		/// <param name="lines">The lines.</param>
		protected override void ProcessNewLines(string[] lines)
		{
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
				{
					continue;
				}

				Match m = Regex.Match(line, ENV_PATTERN);
				if (m.Success)
				{
					string label = m.Groups[1].Value.Trim();
					string value = m.Groups[2].Value.Trim();

					if (label.Length > 0)
					{
						Device.EnvironmentVariables[label] = value;
					}
				}

			}
		}

		/// <summary>
		/// Finishes the receiver
		/// </summary>
		protected override void Done()
		{
			this.Device.OnBuildInfoChanged(EventArgs.Empty);
			base.Done();
		}
	}
}
