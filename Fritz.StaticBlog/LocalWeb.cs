using Fritz.StaticBlog.Data;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;

public static class LocalWeb
{

  public static void StartAdminWeb(string[] args)
  {

    System.Console.WriteLine("Starting Admin Web");

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
      EnvironmentName = Environments.Production
    });

    builder.Services.AddSingleton<WebsiteConfig>();
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

    var app = builder.Build();

    app.UseDeveloperExceptionPage();
    app.UseRouting();

    app.UseStaticFiles(new StaticFileOptions
    {
      FileProvider = new ManifestEmbeddedFileProvider(typeof(LocalWeb).Assembly, "adminweb")
    });

    app.MapAdminSite();
    app.MapRazorPages();

    System.Console.WriteLine("Admin Web Configured.  Navigate to http://localhost:8028 to get started");

    app.Run();

  }

  public static IApplicationBuilder MapAdminSite(this WebApplication app)
  {

    app.MapWhen(ctx => ctx.Connection.LocalPort == 8029, config =>
    {

			var baseFolder = "/home/csharpfritz/dev/KlipTok.Blog";

			config.UseStaticFiles(new StaticFileOptions {
				RequestPath = "/blog",
				FileProvider = new PhysicalFileProvider(Path.Combine(baseFolder, "dist"))

			});

      config.Map("/blog/posts", mapConfig =>
      {

				mapConfig.UseStaticFiles(new StaticFileOptions
				{
					RequestPath = "/blog/posts",
					FileProvider = new PhysicalFileProvider(Path.Combine(baseFolder, "posts"))
				});

				var postLayout = File.ReadAllText(Path.Combine(baseFolder, "themes", "kliptok", "layouts", "posts.html"));

        mapConfig.Run(async ctx =>
        {

					System.Console.WriteLine($"Request Path: {ctx.Request.Path}");

					if (string.IsNullOrEmpty(ctx.Request.Path)) throw new FileNotFoundException("Post not found");
					if (ctx.Request.Path.Value.EndsWith(".html")) throw new FileNotFoundException("Post not found");

					var post = new FileInfo(Path.Combine(baseFolder, "posts", ctx.Request.Path.Value.Substring(1)));
					if (!post.Exists) throw new FileNotFoundException($"Post not found {post.FullName}");

					var result = ActionBuild.BuildPost(post, postLayout, new Config { Theme="kliptok" }, baseFolder);

          ctx.Response.ContentType = "text/html";
          await ctx.Response.WriteAsync(result.fullHTML);

        });

      });


    });

    return app;

  }

}
