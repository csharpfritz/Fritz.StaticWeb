using Fritz.StaticBlog.Data;
using Fritz.StaticBlog.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using ILogger = Fritz.StaticBlog.Infrastructure.ILogger;

public static class LocalWeb
{
	public const string PARM_RUNASYNC = "runasync";
	static bool isRunning = false;
	static WebApplication app;

	public static async Task StartAdminWeb(params string[] args)
	{

		if (isRunning) return;

		System.Console.WriteLine("Starting Admin Web");

		var builder = WebApplication.CreateBuilder(new WebApplicationOptions
		{
			EnvironmentName = Environments.Production
		});

		if (_Config?.Any(c => c.Key == WebsiteConfig.PARM_WORKINGDIRECTORY) ?? false)
		{
			var configPath = Path.Combine(_Config[WebsiteConfig.PARM_WORKINGDIRECTORY], "config.json");
			if (System.IO.File.Exists(configPath))
			{
				var contents = System.IO.File.ReadAllText(configPath);
				try
				{
					var ThisConfig = JsonSerializer.Deserialize<Config>(contents);
					_Config.SiteConfig = ThisConfig;
				}
				catch
				{ }
			}
		}

		//builder.Services.AddSingleton<WebsiteConfig>(_Config ?? new WebsiteConfig());
		builder.Configuration.AddInMemoryCollection(_Config ?? new WebsiteConfig());
		builder.Services.AddSingleton<IFileProvider>(new EmbeddedFileProvider(typeof(LocalWeb).Assembly, "Fritz.StaticBlog.adminweb.Pages"));

		builder.Services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
		{
			options.FileProviders.Clear();
			options.FileProviders.Add(new EmbeddedFileProvider(typeof(LocalWeb).Assembly, "Fritz.StaticBlog.adminweb.Pages"));
		});

		builder.WebHost.UseUrls(new[] { "http://localhost:8028", "http://localhost:8029" });
		builder.Logging.ClearProviders();
		builder.Logging.AddConsole();
		builder.Logging.SetMinimumLevel(LogLevel.Trace);

		builder.Services.AddRazorPages(config =>
		{
			config.RootDirectory = "/";
		})
		.AddRazorRuntimeCompilation()
		.AddApplicationPart(typeof(LocalWeb).Assembly);

		builder.Services.AddCors();

		app = builder.Build();

		await StartWebServer(args);

	}

	public static async Task StartWebServer(params string[] args)
	{
		app.UseDeveloperExceptionPage();
		app.UseRouting();

		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new ManifestEmbeddedFileProvider(typeof(LocalWeb).Assembly, "adminweb")
		});

		app.UseCors(config =>
		{
			config.SetIsOriginAllowed(h => h.Contains("localhost"));
			config.AllowAnyMethod();
			config.AllowAnyHeader();
		});

		app.MapAdminSite(app.Services, NullLogger.Instance);
		app.MapRazorPages();

		System.Console.WriteLine("Admin Web Configured.  Navigate to http://localhost:8028 to get started");

		isRunning = true;

		if (args.Contains(PARM_RUNASYNC))
		{
			await app.StartAsync();
		}
		else
		{
			var ps = new ProcessStartInfo("http://localhost:8028")
			{
				UseShellExecute = true,
				Verb = "open"
			};
			Process.Start(ps);
			app.Run();
			isRunning = false;
		}
	}

	public static async Task Stop()
	{

		await app.StopAsync();
		isRunning = false;

	}

	public static IApplicationBuilder MapAdminSite(this WebApplication app, IServiceProvider services, ILogger logger)
	{

		app.MapWhen(ctx => ctx.Connection.LocalPort == 8029, config =>
		{

			var baseFolder = app.Configuration["WorkingDirectory"];

			config.UseStaticFiles(new StaticFileOptions
			{

				FileProvider = new ConfigurationFileProvider(app.Configuration, WebsiteConfig.PARM_OUTPUTPATH)

			});

			MapPosts(config, baseFolder, logger);

		});

		return app;

	}

	private static void MapPosts(IApplicationBuilder config, string baseFolder, ILogger logger)
	{

		config.Map("/posts", mapConfig =>
		{

			mapConfig.Run(async ctx =>
			{

				if (!Directory.Exists(Path.Combine(app.Configuration["WorkingDirectory"], "posts"))) throw new FileNotFoundException("Posts folder not found");
				var postLayout = File.ReadAllText(Path.Combine(app.Configuration["WorkingDirectory"], "themes", app.Configuration["Theme"], "layouts", "posts.html"));

				if (string.IsNullOrEmpty(ctx.Request.Path)) throw new FileNotFoundException("Post not found");
				if (ctx.Request.Path.Value.EndsWith(".html")) throw new FileNotFoundException("Post not found");

				var post = new FileInfo(Path.Combine(app.Configuration["WorkingDirectory"], "posts", ctx.Request.Path.Value.Substring(1)));
				if (!post.Exists) throw new FileNotFoundException($"Post not found {post.FullName}");

				var result = ActionBuild.BuildPost(post, postLayout, new Config { Theme = app.Configuration["Theme"] }, app.Configuration["WorkingDirectory"], logger);

				ctx.Response.ContentType = "text/html";
				await ctx.Response.WriteAsync(result.fullHTML);

			});

		});

		config.Map("/previewpost", mapConfig =>
		{

			mapConfig.Run(async ctx =>
			{

				if (!Directory.Exists(Path.Combine(app.Configuration["WorkingDirectory"], "posts"))) throw new FileNotFoundException("Posts folder not found");
				var postLayout = File.ReadAllText(Path.Combine(app.Configuration["WorkingDirectory"], "themes", app.Configuration["Theme"], "layouts", "posts.html"));

				var post = ctx.Request.Form["post"];

				var result = ActionBuild.BuildPost(post, postLayout, new Config { Theme = app.Configuration["Theme"] }, app.Configuration["WorkingDirectory"], logger);

				if (result.fullHTML.Contains("<base "))
				{
					result.fullHTML = result.fullHTML.Replace("""<base href="./../">""", """<base href="http://localhost:8029/">""");
				}
				else
				{
					result.fullHTML = result.fullHTML.Replace("""</head>""", """<base href="http://localhost:8029/"></head>""");
				}

				ctx.Response.ContentType = "text/html";
				await ctx.Response.WriteAsync(result.fullHTML);

			});

		});

		config.Map("/savepost", mapConfig =>
		{

			mapConfig.Run(async ctx =>
			{

				if (!Directory.Exists(Path.Combine(app.Configuration["WorkingDirectory"], "posts"))) throw new FileNotFoundException("Posts folder not found");
				var postLayout = File.ReadAllText(Path.Combine(app.Configuration["WorkingDirectory"], "themes", app.Configuration["Theme"], "layouts", "posts.html"));

				var post = ctx.Request.Form["post"];
				var result = ActionBuild.BuildPost(post, postLayout, new Config { Theme = app.Configuration["Theme"] }, app.Configuration["WorkingDirectory"], logger);

				var fileName = result.fm.Title.Replace(' ', '-') + ".md";
				File.WriteAllText(Path.Combine(app.Configuration["WorkingDirectory"], "posts", fileName), post);

				ctx.Response.StatusCode = (int)HttpStatusCode.OK;

			});

		});


	}

	private static WebsiteConfig _Config;
	public static WebsiteConfig WebsiteConfig
	{
		get { return _Config; }
		set { _Config = value; }
	}

	public static string CombineUriPaths(string uri1, string uri2)
	{
		uri1 = uri1.TrimEnd('/');
		uri2 = uri2.TrimStart('/');
		return string.Format("{0}/{1}", uri1, uri2);
	}

	public class PreviewPost
	{
		public string Post { get; set; }
	}

}
