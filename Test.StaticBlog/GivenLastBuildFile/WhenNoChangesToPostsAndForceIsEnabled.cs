using System;
using System.IO;
using Xunit;

namespace Test.StaticBlog.GivenLastBuildFile
{
	public class WhenNoChangesToPostsAndForceIsEnabled : BaseFixture
	{

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
			Assert.NotEmpty(base.OutputPostsFolder.GetFiles());

		}

		[Fact]
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
			Assert.NotEmpty(base.OutputFolder.GetFiles("index.html"));

		}

		public override void Dispose()
		{

			Directory.Delete(_sut.OutputPath, true);

			base.Dispose();
		}

	}


}