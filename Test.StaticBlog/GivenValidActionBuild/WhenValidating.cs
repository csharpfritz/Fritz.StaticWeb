using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{

	public class WhenValidating : BaseFixture {

		[Fact]
		public void ShouldReturnValid() {

			var outValue = _sut.Validate();			

			Assert.True(outValue);

		}


	}

}