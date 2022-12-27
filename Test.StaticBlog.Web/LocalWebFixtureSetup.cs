using NUnit.Framework;

namespace Test.StaticBlog.Web;

[SetUpFixture]
public class LocalWebFixtureSetup
{

  [OneTimeSetUp]
  public async Task AllTestsSetup()
  {

    await LocalWeb.StartAdminWeb(LocalWeb.PARM_RUNASYNC);

  }

  [OneTimeTearDown]
  public async Task AllTestsTeardown() {

    await LocalWeb.Stop();

  }

}
