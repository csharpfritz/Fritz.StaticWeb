using Microsoft.Extensions.FileProviders;
using NUglify.JavaScript.Syntax;

public static class LocalWeb
{

  public static void StartAdminWeb(string[] args)
  {

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
      EnvironmentName = Environments.Production
    });

    builder.WebHost.UseUrls(new[] { "http://localhost:8028", "http://localhost:8029" });
    builder.Logging.ClearProviders();

    var app = builder.Build();

    app.MapAdminSite();

    app.Run();

  }

  public static IApplicationBuilder MapAdminSite(this IApplicationBuilder app)
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
