using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Threading;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenWwwrootContents
{
	public abstract class BaseFixture : TestSiteBaseFixture
	{

		internal ActionBuild _sut;

		protected MockFileSystem FileSystem { get; private set; }

		public BaseFixture()
		{

			base.Initialize();

			FileSystem = new MockFileSystem();
			FileSystem.Directory.CreateDirectory(WorkingDirectory.FullName);
			FileSystem.Directory.CreateDirectory(FileSystem.Path.Combine(WorkingDirectory.FullName, TargetFolderName));

			_sut = new ActionBuild(FileSystem)
			{
				Force = false,
				OutputPath = TargetFolderName,
				ThisDirectory = WorkingDirectory.FullName,
				Config = new Config
				{
					Theme = "kliptok",
					Title = "The Unit Test Website"
				}
			};

		}

	}

}