using System.Reflection;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenValidActionBuild;

public abstract class BaseFixture
{

	internal ActionBuild _sut;
	private static ReaderWriterLockSlim folderLock = new();


	public BaseFixture()
	{

		var workingDirectory = Assembly.GetAssembly(GetType()).Location.Contains(@"\.vs\") ?
				@"..\..\..\..\..\..\..\..\..\TestSite" :
				"../../../../TestSite";


		string targetFolderName = GetType().Name.ToLowerInvariant(); 
		_sut = new ActionBuild
		{
			Force = true,
			OutputPath = targetFolderName,
			ThisDirectory = workingDirectory,
			Config = new Config
			{
				Theme = "kliptok",
				Title = "The Unit Test Website"
			}
		};

		OutputFolder = new DirectoryInfo(Path.Combine(_sut.ThisDirectory, targetFolderName));
		if (!OutputFolder.Exists)
		{
			folderLock.EnterWriteLock();
			OutputFolder.Create();
			folderLock.ExitWriteLock();
		}
		OutputPostsFolder = new DirectoryInfo(Path.Combine(OutputFolder.FullName, "posts"));
		OutputRssFile = new FileInfo(Path.Combine(OutputFolder.FullName, "rss.xml"));

	}

	public DirectoryInfo OutputFolder { get; private set; }
	public DirectoryInfo OutputPostsFolder { get; private set; }
	public FileInfo OutputRssFile { get; private set; }

}