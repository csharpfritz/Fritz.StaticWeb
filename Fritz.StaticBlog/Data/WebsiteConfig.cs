using System.Collections;

namespace Fritz.StaticBlog.Data;

public class WebsiteConfig : IEnumerable<KeyValuePair<string,string>>
{

  private Dictionary<string, string> _Content = new()
  {
    {"OutputPath", AppContext.BaseDirectory },
    {"Theme", "" },
    {"WorkingDirectory", AppContext.BaseDirectory },
  };

  public Config SiteConfig { 
    get { return new Config { Theme = _Content["Theme"] }; }
    set { _Content["Theme"] = value.Theme; }
  }

  /// <summary>
  /// Directory where the website markdown and theme are located
  /// </summary>
  public string WorkingDirectory
  {
    get { return _Content[nameof(WorkingDirectory)]; }
    set { _Content[nameof(WorkingDirectory)] = value; }
  }
  
  /// <summary>
  /// Output folder where the website will be written
  /// </summary>
  public string OutputPath {
    get { return _Content[nameof(OutputPath)]; }
    set { _Content[nameof(OutputPath)] = value; }
  }

  public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
  {
    return ((IEnumerable<KeyValuePair<string, string>>)_Content).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_Content).GetEnumerator();
  }

}
