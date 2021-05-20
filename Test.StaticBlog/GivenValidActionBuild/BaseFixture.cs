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

		}


	}

}