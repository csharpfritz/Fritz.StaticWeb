global using System.IO.Abstractions;
global using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace Test.StaticBlog
{

	public abstract class TestSiteBaseFixture
	{
		private static ReaderWriterLockSlim folderLock = new();

		public DirectoryInfo OutputPostsFolder { get; private set; }
		protected DirectoryInfo OutputFolder { get; private set; }

		protected DirectoryInfo WorkingDirectory { get; private set;}
		protected string TargetFolderName { get; set; }

		public MockFileData PostLayout { 
			get
			{
				return new MockFileData(
				"""
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
				""");
			}
		}

		public MockFileData IndexLayout
		{
			get
			{
				return new MockFileData(
				"""
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
				""");
			}
		}

		public TestSiteBaseFixture()
		{
			TargetFolderName = GetType().Name.ToLowerInvariant();
		}

		public virtual void Initialize()
		{

			var workingDirectory = Assembly.GetAssembly(GetType()).Location.Contains(@"\.vs\") ?
				@"..\..\..\..\..\..\..\..\..\TestSite" :
				"../../../../TestSite";
			this.WorkingDirectory = new DirectoryInfo(workingDirectory);

			InitializeOutputFolder(Path.Combine(workingDirectory, TargetFolderName));

		}

		private void InitializeOutputFolder(string workingDirectory)
		{
			OutputFolder = new DirectoryInfo(workingDirectory);
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