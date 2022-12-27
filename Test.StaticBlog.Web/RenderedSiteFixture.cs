using System.Text.RegularExpressions;

namespace Test.StaticBlog.Web;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class RenderedSiteFixture : PageTest
{


    [Test]
    public async Task NavigateToABlogPostPage()
    {
        await Page.GotoAsync("http://localhost:8029/blog/posts/8-CategoriesAndTeams.md");

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