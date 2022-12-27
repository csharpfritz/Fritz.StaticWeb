using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fritz.StaticBlog.adminweb.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
  public void OnGet()
  {
  }

  public void OnPost()
  {

    Console.WriteLine("Posting...");

    LocalWeb.WebsiteConfig = new()
    {
      BaseUrlPath = "/blog",
      OutputPath = """c:\dev\KlipTok.Blog\dist""",
      SiteConfig = new Fritz.StaticBlog.Data.Config()
      {
        Theme = "kliptok"
      },
      WorkingDirectory = """c:\dev\KlipTok.Blog""",
    };

    LocalWeb.Restart();

  }

}
