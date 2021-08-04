using System.IO;
using System.Reflection;
using System.Threading;
using Fritz.StaticBlog;

namespace Test.StaticBlog
{

	public abstract class TestSiteBaseFixture
	{
		private static ReaderWriterLockSlim folderLock = new();

		public DirectoryInfo OutputPostsFolder { get; private set; }
		protected DirectoryInfo OutputFolder { get; private set; }

		protected DirectoryInfo WorkingDirectory { get; private set;}
		protected string TargetFolderName { get; private set; }

		protected void Initialize() 
		{

			var workingDirectory = Assembly.GetAssembly(GetType()).Location.Contains(@"\.vs\") ?
				@"..\..\..\..\..\..\..\..\..\TestSite" :
				"../../../../TestSite";
			this.WorkingDirectory = new DirectoryInfo(workingDirectory);

			TargetFolderName = GetType().Name.ToLowerInvariant();

			OutputFolder = new DirectoryInfo(Path.Combine(workingDirectory, TargetFolderName));
			if (!OutputFolder.Exists)
			{
				folderLock.EnterWriteLock();
				OutputFolder.Create();
				folderLock.ExitWriteLock();
			}
			OutputPostsFolder = new DirectoryInfo(Path.Combine(OutputFolder.FullName, "posts"));

		}


	}

}