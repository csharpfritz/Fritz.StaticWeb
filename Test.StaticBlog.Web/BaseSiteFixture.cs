using Fritz.StaticBlog.Data;

namespace Test.StaticBlog.Web;

public abstract class BaseSiteFixture : PageTest
{

  protected WebsiteConfig WebsiteConfig = new();

  [OneTimeSetUp]
  public async virtual Task SetupFixture()
  {

    LocalWeb.WebsiteConfig = WebsiteConfig;

    await LocalWeb.StartAdminWeb(LocalWeb.PARM_RUNASYNC);

  }
  [OneTimeTearDown]
  public async virtual Task TeardownFixture()
  {

    await LocalWeb.Stop();

  }



}
