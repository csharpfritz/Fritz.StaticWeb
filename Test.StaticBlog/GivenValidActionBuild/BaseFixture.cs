using System.IO;
using Fritz.StaticBlog;

namespace Test.StaticBlog.GivenValidActionBuild
{
	public abstract class BaseFixture {

		protected ActionBuild _sut;

		public BaseFixture()
		{
				
			_sut = new ActionBuild {
				Force = false,
				OutputPath = "dist",
				WorkingDirectory = "../../../../TestSite",
				Config = new Config {
					Theme = "kliptok"
				}
			};

			OutputFolder = new DirectoryInfo(Path.Combine(_sut.WorkingDirectory, "dist", "posts"));

		}

		public DirectoryInfo OutputFolder { get; private set; }


	}

}