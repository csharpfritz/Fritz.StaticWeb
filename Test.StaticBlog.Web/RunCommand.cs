using Fritz.StaticBlog.Data;

namespace Test.StaticBlog.Web;

[TestFixture]
public class RunCommand : PageTest
{

	[OneTimeTearDown]
	public async virtual Task TeardownFixture()
	{

		await LocalWeb.Stop();

	}


	[Test]
	public async Task StartWithOnlyWorkingDirectoryShouldLoadSettingsFromDisk()
	{

		LocalWeb.WebsiteConfig = new WebsiteConfig
		{
			WorkingDirectory = """c:\dev\KlipTok.Blog"""
		};

		await LocalWeb.StartAdminWeb(LocalWeb.PARM_RUNASYNC);

		Assert.AreEqual("kliptok", LocalWeb.WebsiteConfig.SiteConfig.Theme);

	}



}