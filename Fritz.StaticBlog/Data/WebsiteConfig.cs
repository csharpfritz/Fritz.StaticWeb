using System.Collections;

namespace Fritz.StaticBlog.Data;

public class WebsiteConfig : IEnumerable<KeyValuePair<string,string>>
{

  public const string PARM_OUTPUTPATH = "OutputPath";
  public const string PARM_THEME = "Theme";
  public const string PARM_WORKINGDIRECTORY = "WorkingDirectory";

  private Dictionary<string, string> _Content = new()
  {
    {PARM_OUTPUTPATH, AppContext.BaseDirectory },
    {PARM_THEME, "" },
    {PARM_WORKINGDIRECTORY, AppContext.BaseDirectory },
  };

  public Config SiteConfig { 
    get { return new Config { Theme = _Content[PARM_THEME] }; }
    set { _Content[PARM_THEME] = value.Theme; }
  }

  /// <summary>
  /// Directory where the website markdown and theme are located
  /// </summary>
  public string WorkingDirectory
  {
    get { return _Content[PARM_WORKINGDIRECTORY]; }
    set { _Content[PARM_WORKINGDIRECTORY] = value; }
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
