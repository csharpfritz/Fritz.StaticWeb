using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Fritz.StaticBlog.Infrastructure;

public static class InfrastructureExtensions
{

  public static IFileInfo ToIFileInfo(this FileInfo file)
  {

    return new PhysicalFileInfo(file);

  }


}