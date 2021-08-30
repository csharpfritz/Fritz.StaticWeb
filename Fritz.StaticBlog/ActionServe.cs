using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Fritz.StaticBlog
{

	public class ActionServe : ActionBase, ICommandLineAction
	{
		private IHost _Host;
 
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

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			
			return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
										webBuilder.UseStartup<Startup>();
								});
		}

		public class Startup 
		{

			// TODO: Inspired by Rick's live server at: https://github.com/RickStrahl/LiveReloadServer/blob/master/LiveReloadServer/Startup.cs

			public void ConfigureServices(IServiceCollection services)
			{
				
			}

			public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
			{
				
				app.UseDefaultFiles(new DefaultFilesOptions
				{
					// TODO: Change to the location captured from the command line
						FileProvider = new PhysicalFileProvider("./"),
						DefaultFileNames = new List<string>( new string[] { "index.html" } )
				});

			}

		}

	}

}