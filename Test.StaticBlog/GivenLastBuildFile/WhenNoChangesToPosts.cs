using System;
using System.IO;
using Xunit;

namespace Test.StaticBlog.GivenLastBuildFile
{

	public class WhenNoChangesToPosts : BaseFixture
	{

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
			Assert.Empty(base.OutputPostsFolder.GetFiles());

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

			Directory.Delete(_sut.OutputPath, true);

			base.Dispose();
		}

	}

}