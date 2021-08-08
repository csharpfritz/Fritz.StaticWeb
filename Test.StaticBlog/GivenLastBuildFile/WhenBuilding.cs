using System;
using System.IO;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenLastBuildFile 
{

	public class WhenBuilding : TestSiteBaseFixture, IDisposable 
	{

		private readonly ActionBuild _sut;
		private readonly DateTime _LastBuildDate;

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

			_LastBuildDate = DateTime.Today.AddDays(-5).AddHours(10);

		}
 
		public void Dispose()
		{
			
			File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

		}




	}	

}