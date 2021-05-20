using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	public class WhenBuildingPosts : BaseFixture {

		[Fact]
		public void DoesNotThrowError() {

			_sut.BuildPosts();

		}

		[Fact]
		public void WritesTheFirstPost() {

			// Clear the folder to ensure this file is written

			_sut.BuildPosts();



		}

	}

}