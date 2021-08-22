using System;
using System.IO;
using Fritz.StaticBlog;
using Xunit;

namespace Test.StaticBlog.GivenSiteWithoutLastBuild 
{
		
	public class WhenBuilding : TestSiteBaseFixture, IDisposable
	{
		private readonly ActionBuild _sut;

		public WhenBuilding()
		{
				
			base.Initialize();

			_sut = new ActionBuild
			{
				Force = false,
				OutputPath = TargetFolderName,
				ThisDirectory = WorkingDirectory.FullName,
				LastBuildFilename = $".lastbuild.{Guid.NewGuid()}.json",
				Config = new Config
				{
					Theme = "kliptok",
					Title = "The Unit Test Website"
				}
			};

		}
 
		public void Dispose()
		{
			
			File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

		}

		[Fact]
		public void ThenTheLastBuildFileShouldBeGenerated() {

			_sut.Execute();

			var lastBuildFile = Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename);
			Assert.True(System.IO.File.Exists(lastBuildFile));

		}

		[Fact]
		public void ThenTheLastBuildShouldBeMinDate() {

			_sut.Validate();

			Assert.Equal(DateTime.MinValue, _sut._LastBuild.Timestamp);

		}

	}

}