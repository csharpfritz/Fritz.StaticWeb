using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fritz.StaticBlog.adminweb.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{

  public readonly IConfiguration _Configuration;

  public IndexModel(IConfiguration configuration)
  {
    _Configuration = configuration;
  }

  public void OnGet()
  {
  }

  public void OnPost()
  {

    Console.WriteLine("Posting...");

    _Configuration["OutputPath"] = """c:\dev\KlipTok.Blog\dist""";
    _Configuration["Theme"] = "kliptok";
    _Configuration["WorkingDirectory"] = """c:\dev\KlipTok.Blog""";

  }

}
