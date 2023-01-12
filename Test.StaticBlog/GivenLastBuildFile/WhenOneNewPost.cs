namespace Test.StaticBlog.GivenLastBuildFile
{
	public class WhenOneNewPost : BaseFixture
	{

		public override void Initialize()
		{

			base.Initialize();

			var postsFolder = FileSystem.Path.Combine(WorkingDirectory.FullName, "posts");

			var oldFile = new System.IO.Abstractions.TestingHelpers.MockFileData("# Old file content");
			oldFile.LastWriteTime = DateTime.UtcNow.AddMinutes(-5);
			base.FileSystem.AddFile(
				FileSystem.Path.Combine(postsFolder, "oldPost.md"), oldFile
			);

			var newFile = new System.IO.Abstractions.TestingHelpers.MockFileData("# New file content");
			newFile.LastWriteTime = DateTime.UtcNow;
			base.FileSystem.AddFile(
				FileSystem.Path.Combine(postsFolder, "newPost.md"), newFile
			);


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
			Assert.Empty(base.OutputPostsFolder.GetFiles());

		}

		[Fact(Skip = "Test not finished")]
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
			Assert.Empty(base.OutputFolder.GetFiles("index.html"));

		}

		[Fact(Skip = "Test not finished")]
		public void ShouldRebuildRss()
		{

		}

		public override void Dispose()
		{

			Directory.Delete(_sut.OutputPath, true);

			base.Dispose();
		}

	}


}