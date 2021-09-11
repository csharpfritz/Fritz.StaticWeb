using System;

namespace Fritz.StaticBlog
{
	public class FilesChangedArgs
	{

		/// <summary>
		/// Files that were changed in the last build.
		/// </summary>
		/// <returns></returns>
		public string[] Files { get; set; } = Array.Empty<string>();

	}

}