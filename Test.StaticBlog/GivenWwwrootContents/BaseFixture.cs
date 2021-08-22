using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenWwwrootContents
{
	public abstract class BaseFixture : TestSiteBaseFixture
	{

		internal ActionBuild _sut;

		public BaseFixture()
		{

			base.Initialize();


			_sut = new ActionBuild
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