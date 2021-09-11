using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Fritz.StaticBlog
{

	[Verb("serve", HelpText = "Build the website, serve it and serve updates")]
	public class ActionServe : ActionBase, ICommandLineAction
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
								});
		}

		internal class Startup 
		{

			public static string OutputPath { get; set; }

			public static string SourceFolder { get; set; }

			// TODO: Inspired by Rick's live server at: https://github.com/RickStrahl/LiveReloadServer/blob/master/LiveReloadServer/Startup.cs

			public void ConfigureServices(IServiceCollection services)
			{
				
				services.AddHostedService(_ => new BuildService(SourceFolder, OutputPath));

			}

			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
				
				app.UseDefaultFiles(new DefaultFilesOptions
				{
					// TODO: Change to the location captured from the command line
						FileProvider = new PhysicalFileProvider(OutputPath),
						DefaultFileNames = new List<string>( new string[] { "index.html" } )
				});

			}

		}

		internal class BuildService : IHostedService
		{
			private string _SourceFolder;
			private string _OutputPath;
			private FileSystemWatcher _Watcher;

			public BuildService(string sourceFolder, string outputPath)
			{
				this._SourceFolder = sourceFolder;
				this._OutputPath = outputPath;
			}

			public Task StartAsync(CancellationToken cancellationToken)
			{

				_Watcher = new FileSystemWatcher(_SourceFolder) {
					EnableRaisingEvents = true,
					IncludeSubdirectories = true
				};

				_Watcher.Changed += (sender, e) => {
					// Rebuild website and raise event to update browser
					// TODO: Need backoff / throttle logic

					var build = new ActionBuild {

					};

				};

				return Task.CompletedTask;

			}

			public Task StopAsync(CancellationToken cancellationToken)
			{
				_Watcher.Dispose();
				return Task.CompletedTask;
			}
		}

	}

}