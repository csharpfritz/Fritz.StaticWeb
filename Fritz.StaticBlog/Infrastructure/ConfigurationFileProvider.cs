using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System.IO;

namespace Fritz.StaticBlog.Infrastructure;

public class ConfigurationFileProvider : IFileProvider
{
  private readonly IConfiguration _Configuration;
  private readonly string _Field;

  public ConfigurationFileProvider(IConfiguration configuration, string configurationField)
  {
    _Configuration = configuration;
    _Field = configurationField;
  }

  public IDirectoryContents GetDirectoryContents(string subpath)
  {

    if (string.IsNullOrEmpty(_Configuration?[_Field])) throw new ArgumentException($"Configuration field '{_Field} is not set");
    if (!Directory.Exists(_Configuration[_Field])) return new NotFoundDirectoryContents();

    var relativePath = subpath.StartsWith(@"/") || subpath.StartsWith(@"\") ? subpath.Substring(1) : subpath;
    string path = Path.Combine(_Configuration[_Field], relativePath);
    return new PhysicalDirectoryContents(path);

  }

  public IFileInfo GetFileInfo(string subpath)
  {

    if (string.IsNullOrEmpty(_Configuration?[_Field])) throw new ArgumentException($"Configuration field '{_Field} is not set");

    var relativePath = subpath.StartsWith(@"/") || subpath.StartsWith(@"\") ? subpath.Substring(1) : subpath;
    string path = Path.Combine(_Configuration[_Field], relativePath);
    if (!Directory.Exists(_Configuration[_Field])) return new NotFoundFileInfo(path);
    if (!File.Exists(path)) return new NotFoundFileInfo(path);

    return new FileInfo(path).ToIFileInfo();

  }

  public IChangeToken Watch(string filter)
  {
    if (string.IsNullOrEmpty(_Configuration?[_Field])) throw new ArgumentException($"Configuration field '{_Field} is not set");
    if (!Directory.Exists(_Configuration[_Field])) return NullChangeToken.Singleton;

    var relativeFilter = filter.StartsWith(@"/") || filter.StartsWith(@"\") ? filter.Substring(1) : filter;
    if (relativeFilter.Contains("*")) {
      return new PollingWildCardChangeToken(_Configuration[_Field], relativeFilter);
    }

    return new PollingFileChangeToken(new FileInfo(Path.Combine(_Configuration[_Field], relativeFilter)));

  }
}
