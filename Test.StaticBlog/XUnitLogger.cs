using Fritz.StaticBlog.Infrastructure;

namespace Test.StaticBlog;

public class XUnitLogger : ILogger
{
  private readonly ITestOutputHelper _Output;

  public XUnitLogger(ITestOutputHelper output)
  {
    _Output = output;
  }

  public void Log(string message)
  {
    _Output.WriteLine(message);
  }
}