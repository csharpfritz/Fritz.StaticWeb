using Fritz.StaticBlog;
using System.IO.Abstractions.TestingHelpers;

namespace Test.StaticBlog.GivenSiteWithoutLastBuild;

public class WhenBuilding : TestSiteBaseFixture, IDisposable
{
	private readonly ActionBuild _sut;

	protected MockFileSystem FileSystem { get; private set; }

	public WhenBuilding()
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

	}
 
	public void Dispose()
	{
		
		FileSystem.File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

	}

	[Fact]
	public void ThenTheLastBuildFileShouldBeGenerated() {

		_sut.Execute();

		var lastBuildFile = FileSystem.Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename);
		Assert.True(FileSystem.File.Exists(lastBuildFile));

	}

	[Fact]
	public void ThenTheLastBuildShouldBeMinDate() {

		_sut.Validate();

		Assert.Equal(DateTime.MinValue, _sut._LastBuild.Timestamp);

	}

}