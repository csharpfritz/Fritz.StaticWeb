using System.Text.Json;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenLastBuildFile
{
	public class BaseFixture : TestSiteBaseFixture, IDisposable 
	{

		protected readonly ActionBuild _sut;
		protected readonly DateTime _LastBuildDate;

		public BaseFixture()
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
			var lastBuildFile = File.OpenWrite(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));
			JsonSerializer.Serialize<LastBuild>(lastBuildFile, new LastBuild { Timestamp = _LastBuildDate });
			lastBuildFile.Flush();
			lastBuildFile.Close();

		}
 
		public void Dispose()
		{
			
			File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

		}

	}	


}