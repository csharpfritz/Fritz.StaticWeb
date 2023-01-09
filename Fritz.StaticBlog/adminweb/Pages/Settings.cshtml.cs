using Fritz.StaticBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Fritz.StaticBlog.adminweb.Pages;

public class SettingsModel : PageModel
{

	public SettingsModel(IConfiguration configuration)
	{
		Configuration = configuration;

		// Load Config from config.json in WorkingDirectory
		var configPath = Path.Combine(Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY], "config.json");
		if (System.IO.File.Exists(configPath))
		{
			var contents = System.IO.File.ReadAllText(configPath);
			try
			{
				ThisConfig = JsonSerializer.Deserialize<Config>(contents);
			}
			catch (Exception ex)
			{
				ErrorMessage = $"Error while reading configuration file for website: {ex.Message}";
			}
		}

	}

	public IConfiguration Configuration { get; }
	public Config ThisConfig { get; set; }
	public string ErrorMessage { get; private set; }

	public string BuildLog { get; private set; } = string.Empty;

	public void OnGet()
	{
	}

	public void OnPost()
	{

		var logger = new StringLogger();
		var cmd = new ActionBuild()
		{
			Config = ThisConfig,
			Logger = logger,
			MinifyOutput = true,
			OutputPath = Configuration[WebsiteConfig.PARM_OUTPUTPATH],
			ThisDirectory = Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY]
		};
		cmd.Execute();

		BuildLog = logger.ToString();

	}

}
