using Fritz.StaticBlog.Data;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;

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

    builder.Services.AddSingleton<WebsiteConfig>(_Config ?? new WebsiteConfig());
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

    app.MapAdminSite(app.Services);
    app.MapRazorPages();

    System.Console.WriteLine("Admin Web Configured.  Navigate to http://localhost:8028 to get started");

    isRunning = true;

    if (args.Contains(PARM_RUNASYNC))
    {
      await app.StartAsync();
    }
    else
    {
      app.Run();
      isRunning = false;
    }
  }

  public static async Task Stop()
  {

    await app.StopAsync();
    isRunning = false;

  }

  public static IApplicationBuilder MapAdminSite(this WebApplication app, IServiceProvider services)
  {

    app.MapWhen(ctx => ctx.Connection.LocalPort == 8029, config =>
    {

      var siteConfig = services.GetRequiredService<WebsiteConfig>();

      var baseFolder = siteConfig.WorkingDirectory;

      config.UseStaticFiles(new StaticFileOptions
      {

        RequestPath = "/blog",
        FileProvider = new PhysicalFileProvider(siteConfig.OutputPath)

      });

      MapPosts(config, baseFolder, siteConfig.BaseUrlPath);

    });

    return app;

  }

  private static void MapPosts(IApplicationBuilder config, string baseFolder, string basePath)
  {

    if (!Directory.Exists(Path.Combine(baseFolder, "posts"))) return;

    config.Map("/blog/posts", mapConfig =>
    {

      mapConfig.UseStaticFiles(new StaticFileOptions
      {
        RequestPath = CombineUriPaths(basePath, "posts"),
        FileProvider = new PhysicalFileProvider(Path.Combine(baseFolder, "posts"))
      });

      var postLayout = File.ReadAllText(Path.Combine(baseFolder, "themes", WebsiteConfig.SiteConfig.Theme, "layouts", "posts.html"));

      mapConfig.Run(async ctx =>
      {

        System.Console.WriteLine($"Request Path: {ctx.Request.Path}");

        if (string.IsNullOrEmpty(ctx.Request.Path)) throw new FileNotFoundException("Post not found");
        if (ctx.Request.Path.Value.EndsWith(".html")) throw new FileNotFoundException("Post not found");

        var post = new FileInfo(Path.Combine(baseFolder, "posts", ctx.Request.Path.Value.Substring(1)));
        if (!post.Exists) throw new FileNotFoundException($"Post not found {post.FullName}");

        var result = ActionBuild.BuildPost(post, postLayout, WebsiteConfig.SiteConfig, baseFolder);

        ctx.Response.ContentType = "text/html";
        await ctx.Response.WriteAsync(result.fullHTML);

      });

    });
  }

  private static WebsiteConfig _Config;
  public static WebsiteConfig WebsiteConfig
  {
    get { return app.Services.GetRequiredService<WebsiteConfig>(); }
    set { _Config = value; }
  }

  public static string CombineUriPaths(string uri1, string uri2)
  {
    uri1 = uri1.TrimEnd('/');
    uri2 = uri2.TrimStart('/');
    return string.Format("{0}/{1}", uri1, uri2);
  }

  private static Task _RestartTask;
  internal static void Restart()
  {

    _RestartTask = Task.Run(async () =>
    {

      await Console.Out.WriteLineAsync("Stopping webserver");
      await app.StopAsync();

      await Console.Out.WriteLineAsync("Stopped webserver");

      await Console.Out.WriteLineAsync("Starting webserver");
      await StartAdminWeb();
      await Console.Out.WriteLineAsync("Started webserver");

    });
    //_RestartTask.Start();

  }
}
