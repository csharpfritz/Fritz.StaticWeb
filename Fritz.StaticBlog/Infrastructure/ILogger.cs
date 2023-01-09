using System.Text;

namespace Fritz.StaticBlog.Infrastructure;

public interface ILogger
{

	void Log(string message);

}

public class ConsoleLogger : ILogger
{

	public void Log(string message)
	{
		Console.WriteLine(message);
	}

}

public class NullLogger : ILogger
{

	public static ILogger Instance = new NullLogger();

	public void Log(string message)
	{
		// Do nothing
	}

}

public class StringLogger : ILogger 
{

	private StringBuilder _Builder = new StringBuilder();

	public void Log(string message)
	{
		_Builder.AppendLine(message);
	}

	public override string ToString()
	{
		return _Builder.ToString();
	}

}
