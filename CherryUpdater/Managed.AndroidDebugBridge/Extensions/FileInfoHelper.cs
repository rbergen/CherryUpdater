namespace System.IO
{
	/// <summary>
	/// 
	/// </summary>
	public static class FileInfoHelper
	{

		/// <summary>
		/// Determines whether the specified path is directory.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		///   <c>true</c> if the specified path is directory; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsDirectory(string path)
		{
			return File.Exists(path) && (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
		}

		/// <summary>
		/// Determines whether the specified fsi is directory.
		/// </summary>
		/// <param name="fsi">The fsi.</param>
		/// <returns>
		///   <c>true</c> if the specified fsi is directory; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsDirectory(FileSystemInfo fsi)
		{
			return (fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}

		/// <summary>
		/// Determines whether the specified path is file.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		///   <c>true</c> if the specified path is file; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsFile(string path)
		{
			return File.Exists(path) && !IsDirectory(path);
		}

		/// <summary>
		/// Determines whether the specified fsi is file.
		/// </summary>
		/// <param name="fsi">The fsi.</param>
		/// <returns>
		///   <c>true</c> if the specified fsi is file; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsFile(FileSystemInfo fsi)
		{
			return fsi is FileInfo && !IsDirectory(fsi.FullName);
		}

		/// <summary>
		/// Gets the file system info.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public static FileSystemInfo GetFileSystemInfo(string path)
		{
			if (IsDirectory(path))
			{
				return new DirectoryInfo(path);
			}
			else
			{
				return new FileInfo(path);
			}
		}

	}
}
