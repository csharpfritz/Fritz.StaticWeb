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

      config.Map("/blog/posts", mapConfig =>
      {

				var baseFolder = "/home/csharpfritz/dev/KlipTok.Blog";
				var postLayout = File.ReadAllText(Path.Combine(baseFolder, "themes", "kliptok", "layouts", "posts.html"));

        mapConfig.Run(async ctx =>
        {

					var post = new FileInfo(Path.Combine(baseFolder, "posts", "8-CategoriesAndTeams.md"));

					var result = ActionBuild.BuildPost(post, postLayout, new Config { Theme="kliptok" }, baseFolder);

          ctx.Response.ContentType = "text/html";
          await ctx.Response.WriteAsync(result.postHTML);

        });

      });


    });

    return app;

  }

}
