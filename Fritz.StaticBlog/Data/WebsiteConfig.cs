namespace Fritz.StaticBlog.Data;

public class WebsiteConfig
{

  public Config SiteConfig { get; set; }

  /// <summary>
  /// Directory where the website markdown and theme are located
  /// </summary>
  public string WorkingDirectory { get; set; } = AppContext.BaseDirectory;
  
  /// <summary>
  /// Output folder where the website will be written
  /// </summary>
  public string OutputPath { get; set; } = AppContext.BaseDirectory;

  /// <summary>
  /// Base segment of URLs that should be used when rendering the test site
  /// </summary>
  public string BaseUrlPath { get; set; } = "/";

}
