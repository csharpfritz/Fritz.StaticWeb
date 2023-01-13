namespace Test.StaticBlog.GivenLastBuildFile;

public class WhenBuilding : BaseFixture
{
	private readonly XUnitLogger _Logger;

	public WhenBuilding(ITestOutputHelper outputHelper)
  {
		_Logger = new XUnitLogger(outputHelper);
  }

  [Fact]
	public void ShouldReadLastBuildDate()
	{

		// arrange
		Initialize();
		_sut.Logger = _Logger;

		// act
		_sut.Validate();

		// assert
		Assert.Equal(_LastBuildDate, _sut._LastBuild?.Timestamp);

	}

}