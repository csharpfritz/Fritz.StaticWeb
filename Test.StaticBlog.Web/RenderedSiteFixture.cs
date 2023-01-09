using Microsoft.VisualBasic;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Test.StaticBlog.Web;


//[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RenderedSiteFixture : BaseSiteFixture
{

  public override Task SetupFixture()
  {

    WebsiteConfig = new()
    {
      OutputPath = """c:\dev\KlipTok.Blog\dist""",
      SiteConfig = new Fritz.StaticBlog.Data.Config()
      {
        Theme = "kliptok"
      },
      WorkingDirectory = """c:\dev\KlipTok.Blog""",
    };
    
    return base.SetupFixture();
    
  }

	[Test(), Ignore("Feature not built yet")]
	public async Task GoToIndex()
	{

		var response = await Page.GotoAsync("http://localhost:8029");

		Assert.That(response?.Status ?? 404, Is.EqualTo(200));

	}

	[Test]
  public async Task NavigateToABlogPostPage()
  {
    await Page.GotoAsync("http://localhost:8029/posts/8-CategoriesAndTeams.md");

    await Page.ScreenshotAsync(
      new() { Path = "screenshot.png", FullPage = true }
    );

    // Expect a title "to contain" a substring.
    await Expect(Page).ToHaveTitleAsync(new Regex("KlipTok"));

    //// create a locator
    //var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });

    //// Expect an attribute "to be strictly equal" to the value.
    //await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

    //// Click the get started link.
    //await getStarted.ClickAsync();

    //// Expects the URL to contain intro.
    //await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
  }
}
