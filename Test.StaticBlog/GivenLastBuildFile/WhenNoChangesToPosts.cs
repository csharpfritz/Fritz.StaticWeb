using System;
using System.IO;
using Xunit;

namespace Test.StaticBlog.GivenLastBuildFile
{

	public class WhenNoChangesToPosts : BaseFixture
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

		}

		[Fact]
		public void ShouldNotRebuildPosts()
		{

			// arrange
			TargetFolderName = Guid.NewGuid().ToString();
			Initialize();

			// act
			_sut.Validate();
			_sut.BuildPosts();

			// assert
			var outputFolder = FileSystem.DirectoryInfo.New(FileSystem.Path.Combine(OutputFolder.FullName, "posts"));
			Assert.Empty(outputFolder.GetFiles());

		}

		[Fact]
		public void ShouldNotRebuildIndex()
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

		public override void Dispose()
		{

			//Directory.Delete(_sut.OutputPath, true);

			base.Dispose();
		}

	}

}