using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Fritz.StaticBlog.adminweb.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{

  public readonly IConfiguration _Configuration;

  public string ErrorMessage { get; private set; }

  public IndexModel(IConfiguration configuration)
  {
    _Configuration = configuration;
  }

  [BindProperty]
  public string WorkingFolder { get; set; }

  public void OnGet()
  {
  }

  public IActionResult OnPost()
  {

    _Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY] = WorkingFolder; //"""c:\dev\KlipTok.Blog""";

    if (System.IO.File.Exists(Path.Combine(_Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY], "config.json"))) {

      var contents = System.IO.File.ReadAllText(Path.Combine(_Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY], "config.json"));

      try
      {
        var thisConfig = JsonSerializer.Deserialize<Config>(contents);
        _Configuration[WebsiteConfig.PARM_THEME] = thisConfig.Theme;
      } catch (Exception ex) {
        ErrorMessage = $"Error while reading configuration file for website: {ex.Message}";
      }

    }

    _Configuration[WebsiteConfig.PARM_OUTPUTPATH] = """c:\dev\KlipTok.Blog\dist""";

    return RedirectToPage("/Index");

  }

}
