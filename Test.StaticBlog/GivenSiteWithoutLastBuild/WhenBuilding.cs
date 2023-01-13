using Fritz.StaticBlog;
using System.IO.Abstractions.TestingHelpers;

namespace Test.StaticBlog.GivenSiteWithoutLastBuild;

public class WhenBuilding : TestSiteBaseFixture
{
	private readonly ActionBuild _sut;

	protected MockFileSystem FileSystem { get; private set; }

	public WhenBuilding(ITestOutputHelper helper)
	{
			
		base.Initialize();

		FileSystem = new MockFileSystem();
		FileSystem.AddFile(
			FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "layouts", "posts.html"),
			PostLayout);
		FileSystem.AddFile(
			FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "layouts", "index.html"),
			IndexLayout);

		var postsFolder = FileSystem.Path.Combine(WorkingDirectory.FullName, "posts");
		var oldFile = new MockFileData("""
				---
				draft: false
				---
				# Old file content
				""");
		oldFile.LastWriteTime = DateTime.UtcNow.AddMinutes(-5);
		FileSystem.AddFile(
			FileSystem.Path.Combine(postsFolder, "oldPost.md"), oldFile
		);
		FileSystem.AddFile(
			FileSystem.Path.Combine(WorkingDirectory.FullName, "config.json"),
			new MockFileData("""{ "theme": "test" }"""));
		FileSystem.Directory.CreateDirectory(OutputFolder.FullName);
		FileSystem.Directory.CreateDirectory(FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "test"));
		FileSystem.AddFile(FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "test", "layouts", "index.html"), IndexLayout);
		FileSystem.AddFile(FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "test", "layouts", "posts.html"), PostLayout);


		_sut = new ActionBuild(FileSystem)
		{
			Force = false,
			OutputPath = TargetFolderName,
			ThisDirectory = WorkingDirectory.FullName,
			LastBuildFilename = $".lastbuild.{Guid.NewGuid()}.json",
			Config = new Config
			{
				Theme = "kliptok",
				Title = "The Unit Test Website"
			}
		};
		_sut.Logger = new XUnitLogger(helper);
		

	}
 
	//public void Dispose()
	//{
		
	//	FileSystem.File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

	//}

	[Fact]
	public void ThenTheLastBuildFileShouldBeGenerated() {

		var outValue = _sut.Execute();

		Assert.Equal(0, outValue);

		var lastBuildFile = FileSystem.Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename);
		Assert.True(FileSystem.File.Exists(lastBuildFile));

	}

	[Fact]
	public void ThenTheLastBuildShouldBeMinDate() {

		_sut.Validate();

		Assert.Equal(DateTime.MinValue, _sut._LastBuild.Timestamp);

	}

}