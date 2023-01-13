using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenValidActionBuild;

public abstract class BaseFixture
{

	internal ActionBuild _sut;
	private static ReaderWriterLockSlim folderLock = new();

	internal MockFileSystem FileSystem { get; }

	public IDirectoryInfo PostsFolder { get; }

	public BaseFixture()
	{

		var workingDirectory = Assembly.GetAssembly(GetType()).Location.Contains(@"\.vs\") ?
				@"..\..\..\..\..\..\..\..\..\TestSite" :
				"../../../../TestSite";


		FileSystem = new MockFileSystem();
		workingDirectory = FileSystem.DirectoryInfo.New(workingDirectory).FullName;
		FileSystem.AddDirectory(workingDirectory);
		FileSystem.Directory.CreateDirectory(FileSystem.Path.Combine(workingDirectory, "themes"));
		PostsFolder = FileSystem.Directory.CreateDirectory(FileSystem.Path.Combine(workingDirectory, "posts"));
		FileSystem.AddFile(FileSystem.Path.Combine(workingDirectory, "config.json"), new MockFileData(ConfigJsonContent));

		// Layouts
		FileSystem.AddFile(FileSystem.Path.Combine(workingDirectory, "themes", "kliptok", "layouts", "archive.html"), new MockFileData(ArchiveLayoutContent));
		FileSystem.AddFile(FileSystem.Path.Combine(workingDirectory, "themes", "kliptok", "layouts", "index.html"), new MockFileData(IndexLayoutContent));
		FileSystem.AddFile(FileSystem.Path.Combine(workingDirectory, "themes", "kliptok", "layouts", "posts.html"), new MockFileData(PostLayoutContent));
		FileSystem.AddFile(FileSystem.Path.Combine(workingDirectory, "themes", "kliptok", "includes", "sample.html"), new MockFileData("This is an include"));
		FileSystem.AddFile(FileSystem.Path.Combine(workingDirectory, "themes", "kliptok", "includes", "sampleWithMacro.html"), new MockFileData("This is the current year: {{ CurrentYear }}"));

		string targetFolderName = GetType().Name.ToLowerInvariant();

		_sut = new ActionBuild(FileSystem)
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

		OutputFolder = FileSystem.DirectoryInfo.New(FileSystem.Path.Combine(workingDirectory, targetFolderName));
		OutputFolder.Create();
		OutputPostsFolder = FileSystem.DirectoryInfo.New(FileSystem.Path.Combine(OutputFolder.FullName, "posts"));
		OutputRssFile = FileSystem.FileInfo.New(FileSystem.Path.Combine(OutputFolder.FullName, "rss.xml"));

	}

	public IDirectoryInfo OutputFolder { get; private set; }
	public IDirectoryInfo OutputPostsFolder { get; private set; }
	public IFileInfo OutputRssFile { get; private set; }

	protected virtual string ConfigJsonContent => """{}""";

	protected string ArchiveLayoutContent => """
 	<html>
 	<head>
 		<title>{{ Title }} - Post Archive</title>
 		<!-- Test Layout -->
 	</head>
 	<body>
 		{{ Body }}

 		<span>Year: {{ CurrentYear }}</span>
 	</body>
 </html>
 """;

	protected string IndexLayoutContent => """
	<html>
		<head>
			<title>{{ Title }}</title>
			<!-- Test Layout -->
		</head>
		<body>
			{{ Body }}

			<span>Year: {{ CurrentYear }}</span>
			<a href="{{ ArchiveURL }}">All posts</a>

			<footer>{{ Include:sample }}</footer>

			{{ Include:sampleWithMacro }}

		</body>
	</html>
	""";

	public string PostLayoutContent => """
	<html>
		<head>
			<title>{{ Title }}</title>
		</head>
		<body>

			<h1>{{ Title }}</h1>
			<h3>Author: {{ Author }}</h3>
			<h5>Published: {{ PublishDate }}</h5>

			{{ Body }}

			<span>Year: {{ CurrentYear }}</span>

		</body>
	</html>
	""";

}

