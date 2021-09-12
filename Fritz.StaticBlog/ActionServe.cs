using CommandLine;
using Fritz.StaticBlog.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Fritz.StaticBlog
{

	[Verb("serve", HelpText = "Build the website, serve it and serve updates")]
	public partial class ActionServe : ActionBase, ICommandLineAction
	{
		private IHost _Host;

		[Option('o', "output", Required = true, HelpText = "Location to write out the rendered site")]
		public string OutputPath { get; set; }

		[Option('w', "workdir", Default = ".", Required = false, HelpText = "The directory to run the build against.  Default current directory")]
		public string ThisDirectory 
		{ 
			get { return this.WorkingDirectory; }
			set { this.WorkingDirectory = value; }	
		}

		public override int Execute()
		{

			base.Execute();

			_Host = CreateHostBuilder(new string[] {}).Build();
			_Host.Run();

			return 0;

		}

		public override bool Validate()
		{
			// do nothing
			return true;
		}

		private IHostBuilder CreateHostBuilder(string[] args)
		{
			
			Startup.OutputPath = OutputPath;
			Startup.SourceFolder = WorkingDirectory;

			return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
										webBuilder.UseStartup<Startup>();
										webBuilder.ConfigureKestrel(options => {
											options.ListenAnyIP(35729);
										});
								});
		}

	}

}