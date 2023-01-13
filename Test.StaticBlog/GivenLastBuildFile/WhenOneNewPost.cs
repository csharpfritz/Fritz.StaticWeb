using System.IO.Abstractions;

namespace Test.StaticBlog.GivenLastBuildFile
{
	public class WhenOneNewPost : BaseFixture
	{

		new IDirectoryInfo OutputFolder { get; set; }
		new IDirectoryInfo OutputPostsFolder { get; set; }

		public override void Initialize()
		{

			base.Initialize();

			var postsFolder = FileSystem.Path.Combine(WorkingDirectory.FullName, "posts");

			var oldFile = new System.IO.Abstractions.TestingHelpers.MockFileData("""
				---
				draft: false
				---
				# Old file content
				""");
			oldFile.LastWriteTime = DateTime.UtcNow.AddMinutes(-5);
			base.FileSystem.AddFile(
				FileSystem.Path.Combine(postsFolder, "oldPost.md"), oldFile
			);

			var newFile = new System.IO.Abstractions.TestingHelpers.MockFileData("""
			---
			draft: false
			---
			# New file content
			""");
			newFile.LastWriteTime = DateTime.UtcNow;
			base.FileSystem.AddFile(
				FileSystem.Path.Combine(postsFolder, "newPost.md"), newFile
			);
			FileSystem.AddFile(FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "includes", "sample.html"), new MockFileData("This is an include"));
			FileSystem.AddFile(FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "includes", "sampleWithMacro.html"), new MockFileData("This is the current year: {{ CurrentYear }}"));

			OutputPostsFolder = FileSystem.DirectoryInfo.New(FileSystem.Path.Combine(base.OutputFolder.FullName, "posts"));
			OutputFolder = FileSystem.DirectoryInfo.New(base.OutputFolder.FullName);

		}

		[Fact()]
		public void ShouldBuildOnlyThatPost()
		{

			// arrange
			TargetFolderName = Guid.NewGuid().ToString();
			Initialize();

			// act
			_sut.Validate();
			_sut.BuildPosts();

			// assert
			Assert.Single(OutputPostsFolder.GetFiles());

		}

		[Fact()]
		public void ShouldRebuildIndex()
		{

			// arrange
			TargetFolderName = Guid.NewGuid().ToString();
			Initialize();

			// act
			_sut.Validate();
			_sut.BuildPosts();
			_sut.BuildIndex();

			// assert
			Assert.NotEmpty(OutputFolder.GetFiles("index.html"));

		}

		[Fact()]
		public void ShouldRebuildRss()
		{

			// arrange
			TargetFolderName = Guid.NewGuid().ToString();
			Initialize();

			// act
			_sut.Validate();
			_sut.BuildPosts();
			_sut.BuildIndex();
			_sut.BuildRss();

			// assert
			Assert.NotEmpty(OutputFolder.GetFiles("rss.xml"));

		}

		public override void Dispose()
		{

			Directory.Delete(_sut.OutputPath, true);

			base.Dispose();
		}

	}


}