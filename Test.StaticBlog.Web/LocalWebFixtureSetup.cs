using NUnit.Framework;

namespace Test.StaticBlog.Web;

[SetUpFixture]
public class LocalWebFixtureSetup
{

  [OneTimeSetUp]
  public async Task AllTestsSetup()
  {

    LocalWeb.WebsiteConfig = new()
    {
      OutputPath = """c:\dev\KlipTok.Blog\dist""",
      SiteConfig = new Fritz.StaticBlog.Data.Config() {
        Theme = "kliptok"
      },
      WorkingDirectory = """c:\dev\KlipTok.Blog""",
    };

    await LocalWeb.StartAdminWeb(LocalWeb.PARM_RUNASYNC);

    //await LocalWeb.Stop();
    //await LocalWeb.StartWebServer(LocalWeb.PARM_RUNASYNC);

  }

  [OneTimeTearDown]
  public async Task AllTestsTeardown()
  {

    await LocalWeb.Stop();

  }

}
