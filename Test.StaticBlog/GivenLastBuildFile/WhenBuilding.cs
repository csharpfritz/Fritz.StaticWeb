using System;
using System.IO;
using Xunit;

namespace Test.StaticBlog.GivenLastBuildFile
{

	public class WhenBuilding : BaseFixture
	{

		[Fact]
		public void ShouldReadLastBuildDate()
		{

			// act
			_sut.Validate();

			// assert
			Assert.Equal(_LastBuildDate, _sut._LastBuild?.Timestamp);

		}

	}


}