﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Managed.Adb
{
	public class PackageManagerReceiver : MultiLineReceiver
	{
		/// <summary>
		/// Pattern to parse the output of the 'pm -lf' command.
		/// The output format looks like:
		/// /data/app/myapp.apk=com.mypackage.myapp
		/// </summary>
		public const string PM_PACKAGE_PATTERN = "^package:(.+?)=(.+)$";

		public PackageManagerReceiver(Device device, PackageManager pm)
		{
			Device = device;
			PackageManager = pm;
		}

		public Device Device { get; set; }
		public PackageManager PackageManager { get; set; }

		protected override void ProcessNewLines(string[] lines)
		{
			PackageManager.Packages.Clear();
			foreach (string line in lines)
			{
				if (line.Length > 0)
				{
					// get the filepath and package from the line
					Match m = Regex.Match(line, PackageManagerReceiver.PM_PACKAGE_PATTERN, RegexOptions.Compiled);
					if (m.Success)
					{
						// get the children with that path
						FileEntry entry;
						if (PackageManager.Packages.ContainsKey(m.Groups[2].Value))
						{
							entry = PackageManager.Packages[m.Groups[1].Value];
							if (entry != null)
							{
								entry.Info = m.Groups[2].Value;
							}
						}
						else
						{
							entry = FileEntry.Find(Device, m.Groups[1].Value);
							entry.Info = m.Groups[2].Value;
							PackageManager.Packages.Add(m.Groups[2].Value, entry);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PackageManagerListingReceiver : MultiLineReceiver
	{


		/// <summary>
		/// Initializes a new instance of the <see cref="PackageManagerReceiver"/> class.
		/// </summary>
		/// <param name="entryMap">The entry map.</param>
		/// <param name="receiver">The receiver.</param>
		public PackageManagerListingReceiver(Dictionary<string, FileEntry> entryMap, IListingReceiver receiver)
		{
			this.Map = entryMap;
			this.Receiver = receiver;
		}

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public Dictionary<string, FileEntry> Map { get; set; }
		/// <summary>
		/// Gets or sets the receiver.
		/// </summary>
		/// <value>The receiver.</value>
		public IListingReceiver Receiver { get; set; }

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
					// get the filepath and package from the line
					Match m = Regex.Match(line, PackageManagerReceiver.PM_PACKAGE_PATTERN, RegexOptions.Compiled);
					if (m.Success)
					{
						// get the children with that path
						FileEntry entry = Map[m.Groups[1].Value];
						if (entry != null)
						{
							entry.Info = m.Groups[2].Value;
							Receiver.RefreshEntry(entry);
						}
					}
				}
			}

		}
	}
}
