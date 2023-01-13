using System;
using System.IO;
using Xunit;

namespace Test.StaticBlog.GivenLastBuildFile
{
	public class WhenNoChangesToPostsAndForceIsEnabled : BaseFixture
	{

		public override void Initialize()
		{

			base.Initialize();

			var postsFolder = FileSystem.Path.Combine(WorkingDirectory.FullName, "posts");

			var oldFile = new System.IO.Abstractions.TestingHelpers.MockFileData(
				"""
				---
				draft: false
				---
				# Old file content
				""");
			oldFile.LastWriteTime = DateTime.UtcNow;
			base.FileSystem.AddFile(
				FileSystem.Path.Combine(postsFolder, "oldPost.md"), oldFile
			);

		}

		[Fact]
		public void ShouldRebuildPosts()
		{

			// arrange
			TargetFolderName = Guid.NewGuid().ToString();
			Initialize();
			_sut.Force = true;

			// act
			_sut.Validate();
			_sut.BuildPosts();

			// assert
			var outputFolder = FileSystem.DirectoryInfo.New(FileSystem.Path.Combine(OutputFolder.FullName, "posts"));
			Assert.NotEmpty(outputFolder.GetFiles());

		}

		[Fact]
		public void ShouldRebuildIndex()
		{

			// arrange
			TargetFolderName = Guid.NewGuid().ToString();
			Initialize();
			_sut.Force = true;

			// act
			_sut.Validate();
			_sut.BuildPosts();
			_sut.BuildIndex();

			// assert
			var outputFolder = FileSystem.DirectoryInfo.New(OutputFolder.FullName);
			Assert.NotEmpty(outputFolder.GetFiles("index.html"));

		}

		public override void Dispose()
		{

			Directory.Delete(_sut.OutputPath, true);

			base.Dispose();
		}

	}


}