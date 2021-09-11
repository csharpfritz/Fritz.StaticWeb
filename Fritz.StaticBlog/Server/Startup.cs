using System.Collections.Generic;
using Fritz.StaticBlog.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Fritz.StaticBlog.Server
{

		internal class Startup 
		{

			public static string OutputPath { get; set; }

			public static string SourceFolder { get; set; }

			// TODO: Inspired by Rick's live server at: https://github.com/RickStrahl/LiveReloadServer/blob/master/LiveReloadServer/Startup.cs

			public BuildService BuildService { get; set;}

			public void ConfigureServices(IServiceCollection services)
			{
				
				this.BuildService = new BuildService(SourceFolder, OutputPath);
				this.BuildService.BuildComplete += BuildService_BuildComplete;
				services.AddHostedService(_ => this.BuildService);

			}

			private void BuildService_BuildComplete(object sender, FilesChangedArgs e)
			{
				// trigger live reload
			}

			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
				
				app.UseDefaultFiles(new DefaultFilesOptions
				{
					// TODO: Change to the location captured from the command line
						FileProvider = new PhysicalFileProvider(OutputPath),
						DefaultFileNames = new List<string>( new string[] { "index.html" } )
				});
				app.UseStaticFiles(OutputPath);

				// Need to attach a web socket listener to the app
				app.UseLiveReload();

			}

		}

}