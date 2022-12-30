namespace Test.StaticBlog.Web;

//[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class FirstTimeStartupAdminSiteFixture : BaseSiteFixture
{

  [Test]
  public async Task NavigateToTheAdminSite()
  {
    await Page.GotoAsync("http://localhost:8028");

    await Page.ScreenshotAsync(
      new() { Path = "screenshot_admin.png", FullPage = true }
    );

    var warningElement = Page.GetByText("The output path is not set");
    Assert.NotNull(warningElement, "Could not locate the warning that the site is not configured");

  }


}