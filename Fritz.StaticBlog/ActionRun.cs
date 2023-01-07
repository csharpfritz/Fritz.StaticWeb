using CommandLine;

namespace Fritz.StaticBlog;

[Verb("run", HelpText = "Start a helpful web application to manage the website")]
public class ActionRun : ICommandLineAction
{

	[Option('w', "workdir", Default = ".", Required = false, HelpText = "The directory containing the website to manage")]
	public string ThisDirectory { get; set; }


  public int Execute()
  {
    
    LocalWeb.WebsiteConfig = new WebsiteConfig {
        WorkingDirectory = ThisDirectory
    };

    LocalWeb.StartAdminWeb();

    return 0;


  }

}