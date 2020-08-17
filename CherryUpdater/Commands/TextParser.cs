using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CherryUpdater
{
	public static class TextParser
	{
		#region Public static methods

		public static string[] ParseActivities(string activitiesText)
		{
			string[] rawActivities = activitiesText.Trim().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			List<string> activities = new List<string>();

			foreach (string activity in rawActivities)
			{
				string trimmedActivity = activity.Trim();

				if (trimmedActivity != string.Empty && trimmedActivity[0] != '#')
				{
					activities.Add(trimmedActivity);
				}
			}

			return activities.ToArray();
		}

		public static bool? ParseNullableXmlBoolean(XmlAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}

			return ParseXmlBoolean(attribute.Value);
		}

		public static bool ParseXmlBoolean(XmlAttribute attribute)
		{
			return ParseXmlBoolean(attribute.Value);
		}

		public static bool ParseXmlBoolean(string text)
		{
			return text == "true" || text == "1";
		}

		public static string ParseMultilineElementText(string text)
		{
			string[] textLines = text.Trim().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			if (textLines.Length == 0)
			{
				return null;
			}

			StringBuilder parsedTextBuilder = new StringBuilder(textLines[0].Trim());

			for (int index = 1; index < textLines.Length; index++)
			{
				parsedTextBuilder.Append("\r\n");
				parsedTextBuilder.Append(textLines[index].Trim());
			}

			return parsedTextBuilder.ToString();
		}

		public static string[] ParseProductModels(string productModelsText)
		{
			string[] productModels = productModelsText.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

			return productModels == null || productModels.Length == 0 ? null : productModels;
		}

		#endregion
	}
}
