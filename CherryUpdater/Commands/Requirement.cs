using System;
using System.IO;
using System.Xml;

namespace CherryUpdater
{
	public enum PathType
	{
		File,
		Directory
	}

	public class Requirement
	{
		#region Member variables

		private readonly string mPath;
		private readonly PathType mType;

		#endregion

		#region Constructor

		public Requirement(string path, PathType type)
		{
			mPath = path;
			mType = type;
		}

		#endregion

		#region Public static methods

		public static Requirement CreateFromXML(XmlElement requireElement)
		{
			if (requireElement.Name != "require")
			{
				throw new ArgumentException("argument must be a require element", "requireElement");
			}

			return new Requirement(requireElement.Attributes["path"].Value, requireElement.Attributes["type"].Value == "file" ? PathType.File : PathType.Directory);
		}

		#endregion

		#region Public methods

		public bool Validate()
		{
			try
			{
				return mType == PathType.Directory ? Directory.Exists(mPath) : File.Exists(mPath);
			}
			catch { }

			return false;
		}

		#endregion
	}
}
