using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Fritz.StaticBlog;
using Fritz.StaticBlog.Data;

namespace Test.StaticBlog.GivenLastBuildFile
{
    public class BaseFixture : TestSiteBaseFixture, IDisposable 
	{

		protected ActionBuild _sut { get; private set; }
		protected DateTime _LastBuildDate;

		protected MockFileSystem FileSystem { get; private set; }
		public override void Initialize()
		{
		
			base.Initialize();

			_LastBuildDate = DateTime.UtcNow.AddSeconds(-5);
			string lastBuildFilename = $".lastbuild.{Guid.NewGuid()}.json";
			
			FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ Path.Combine(WorkingDirectory.FullName, lastBuildFilename),
					$$"""
						{
							"Timestamp":  "{{_LastBuildDate.ToString("o")}}"
						}
					""" 
				}
			});
			FileSystem.AddFile(
				FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "layouts", "posts.html"), 
				PostLayout);
			FileSystem.AddFile(
				FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "layouts", "index.html"),
				IndexLayout);


			_sut = new ActionBuild(FileSystem)
			{
				Force = false,
				OutputPath = OutputFolder.FullName,
				ThisDirectory = WorkingDirectory.FullName,
				LastBuildFilename = lastBuildFilename,
				Config = new Config
				{
					Theme = "kliptok",
					Title = "The Unit Test Website",
					Link = "https://test.csharpfritz.com"
				}
			};

		}

		public virtual void Dispose()
		{

      File.Delete(Path.Combine(WorkingDirectory.FullName, _sut.LastBuildFilename));

    }

  }	


}