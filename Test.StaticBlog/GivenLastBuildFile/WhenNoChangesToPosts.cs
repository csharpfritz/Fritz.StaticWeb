using Xunit;

namespace Test.StaticBlog.GivenLastBuildFile
{
	public class WhenNoChangesToPosts : BaseFixture
	{

		[Fact]
		public void ShouldNotRebuildPosts()
		{

			Assert.False(true);

		}

		[Fact]
		public void ShouldNotRebuildIndex()
		{

			Assert.False(true);

		}


	}


}