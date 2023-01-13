using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild;


public class WhenValidating : BaseFixture
{
	private readonly XUnitLogger _Logger;

	public WhenValidating(ITestOutputHelper output)
	{
		_Logger = new XUnitLogger(output);
	}

	[Fact]
	public void ShouldReturnValid()
	{

		_sut.Logger = _Logger;
		var outValue = _sut.Validate();

		Assert.True(outValue);

	}


}
