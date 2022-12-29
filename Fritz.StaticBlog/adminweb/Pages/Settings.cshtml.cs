using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fritz.StaticBlog.adminweb.Pages;

public class SettingsModel : PageModel
{

  public SettingsModel(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  public void OnGet()
  {
  }
}
