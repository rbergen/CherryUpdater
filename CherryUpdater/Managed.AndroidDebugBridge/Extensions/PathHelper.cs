namespace System.IO
{
	/// <summary>
	/// 
	/// </summary>
	public static class PathHelper
	{

		/// <summary>
		/// Combines the specified paths.
		/// </summary>
		/// <remarks>This wraps the normal System.IO.Path.Combine to allow for an unlimited list of paths to combine</remarks>
		/// <param name="paths">The paths.</param>
		/// <returns></returns>
		public static string Combine(params string[] paths)
		{
			string lastPath = string.Empty;

			foreach (var item in paths)
			{
				lastPath = Path.Combine(lastPath, item);
			}

			return lastPath;
		}

	}
}
