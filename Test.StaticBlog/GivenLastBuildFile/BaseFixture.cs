using System;
using System.IO;
using System.Text.Json;
using Fritz.StaticBlog;
using Fritz.StaticBlog.Data;

namespace Test.StaticBlog.GivenLastBuildFile
{
    public class BaseFixture : TestSiteBaseFixture, IDisposable 
	{

		protected ActionBuild _sut { get; private set; }
		protected DateTime _LastBuildDate;

		public override void Initialize()
		{
				
			base.Initialize();

			_sut = new ActionBuild
			{
				Force = false,
				OutputPath = OutputFolder.FullName,
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
			JsonSerializer.SerializeAsync<LastBuild>(lastBuildFile, new LastBuild { Timestamp = _LastBuildDate }).GetAwaiter().GetResult();
			lastBuildFile.Flush();
			lastBuildFile.Close();

		}
 
		public virtual void Dispose()
		{
			
			File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

		}

	}	


}