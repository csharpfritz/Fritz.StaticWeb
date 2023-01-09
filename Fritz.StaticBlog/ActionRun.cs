using CommandLine;

namespace Fritz.StaticBlog;

[Verb("run", HelpText = "Start a helpful web application to manage the website")]
public class ActionRun : ICommandLineAction
{

	[Option('w', "workdir", Default = ".", Required = false, HelpText = "The directory containing the website to manage")]
	public string WorkingDirectory { get; set; }

	[Option('o', "output", Required = false, HelpText = "Location to write out the rendered site")]
	public string OutputPath { get; set; }

	public int Execute()
  {
    
    LocalWeb.WebsiteConfig = new WebsiteConfig {
				OutputPath = OutputPath,
        WorkingDirectory = WorkingDirectory
    };

    LocalWeb.StartAdminWeb();

    return 0;


  }

}