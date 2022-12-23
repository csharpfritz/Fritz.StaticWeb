using Microsoft.Extensions.FileProviders;
using NUglify.JavaScript.Syntax;

public static class LocalWeb
{

  public static void StartAdminWeb(string[] args)
  {

		System.Console.WriteLine("Starting Admin Web");

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
      EnvironmentName = Environments.Production
    });

    builder.WebHost.UseUrls(new[] { "http://localhost:8028", "http://localhost:8029" });
    builder.Logging.ClearProviders();

		builder.Services.AddRazorPages();

    var app = builder.Build();

    app.MapAdminSite();

		app.MapRazorPages();

		System.Console.WriteLine("Admin Web Configured");

    app.Run();

  }

  public static IApplicationBuilder MapAdminSite(this WebApplication app)
  {

    app.MapWhen(ctx => ctx.Connection.LocalPort == 8028, config =>
    {

      config.MapWhen(ctx => ctx.Request.Path == "/", c => c.Run(x =>
      {
        x.Response.Redirect("/index.html");
        return Task.CompletedTask;
      }));

      config.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new ManifestEmbeddedFileProvider(typeof(LocalWeb).Assembly, "adminweb")
      });

    });

    return app;

  }

}
