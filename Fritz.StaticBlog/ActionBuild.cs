using System.Diagnostics;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommandLine;
using Fritz.StaticBlog.Infrastructure;
using Markdig;
using Markdig.Prism;
using ILogger = Fritz.StaticBlog.Infrastructure.ILogger;

namespace Fritz.StaticBlog;

[Verb("build", HelpText = "Build the website")]
public class ActionBuild : ActionBase, ICommandLineAction
{
	private const string ArchiveFileName = "archive.html";
	internal List<PostData> _Posts = new();
	internal LastBuild _LastBuild;

	public static MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
		.UseAdvancedExtensions()
		.UseYamlFrontMatter()
		.UsePrism()
		.Build();

	public ActionBuild(IFileSystem fileSystem) : base(fileSystem) { }

	public ActionBuild() : this(new FileSystem()) { }

  [Option('f', "force", Default = (bool)false)]
	public bool Force { get; set; }

	[Option('o', "output", Required = true, HelpText = "Location to write out the rendered site")]
	public string OutputPath { get; set; }

	[Option('m', "minify", Default = (bool)false, HelpText = "Minify the output HTML")]
	public bool MinifyOutput { get; set; } = false;

	[Option('w', "workdir", Default = ".", Required = false, HelpText = "The directory to run the build against.  Default current directory")]
	public string ThisDirectory
	{
		get { return base.WorkingDirectory; }
		set { base.WorkingDirectory = value; }
	}

	[Option('l', "lastbuild", Default = ".lastbuild.json", Required = false, HelpText = "The file to store the last build configuration")]
	public string LastBuildFilename { get; set; } = ".lastbuild.json";

	public ILogger Logger { get; set; } = new ConsoleLogger();

	public override int Execute()
	{

		Logger.Log($"Outputting to: {OutputPath}");

		var sw = Stopwatch.StartNew();

		var outValue = base.Execute();
		if (outValue > 0) return outValue;

		Logger.Log($"Building in folder {WorkingDirectory} and distributing to {Path.Combine(WorkingDirectory, OutputPath)}");

		BuildPosts();

		BuildPages();

		BuildRss();

		BuildIndex();

		BuildArchive();

		DeployWwwRoot();

		SaveLastBuild();

		Logger.Log($"Built website successfully in {sw.Elapsed}");

		return 0;

	}

	public override bool Validate()
	{

		var outputDir = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, OutputPath));
		var outValue = outputDir.Exists;

		if (!outValue) Logger.Log($"Output folder '{outputDir.FullName}' does not exist");
		if (outValue)
		{
			outValue = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "themes")).Exists;
			if (!outValue) Logger.Log("themes folder is missing");
		}

		if (outValue)
		{
			outValue = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "posts")).Exists;
			if (!outValue) Logger.Log("posts folder is missing");
		}

		/*  -- Making pages folder optional --
if (outValue)
{
outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "pages")).Exists;
if (!outValue) Logger.Log("pages folder is missing");
}
**/

		if (outValue)
		{
			outValue = _FileSystem.FileInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "config.json")).Exists;
			if (!outValue) Logger.Log($"config.json file is missing");
		}

		// Validate LastBuild configuration
		if (outValue)
		{
			var exists = _FileSystem.FileInfo.New(Path.Combine(WorkingDirectory, LastBuildFilename)).Exists;
			if (!exists)
			{
				Logger.Log($"LastBuild file is missing - Complete build requested");
				_LastBuild = new LastBuild { Timestamp = DateTime.MinValue };
			}
			else
			{
				var file = _FileSystem.File.OpenRead(Path.Combine(WorkingDirectory, LastBuildFilename));
				_LastBuild = JsonSerializer.DeserializeAsync<LastBuild>(file).GetAwaiter().GetResult();
				Logger.Log($"Website last built at: {_LastBuild.Timestamp} UTC");
				file.Dispose();
			}
		}

		return outValue;

	}

	internal void BuildIndex()
	{

		if (!Force && _Posts.All(p => p.LastUpdate <= _LastBuild?.Timestamp))
		{
			Logger.Log("No new posts found.  Skipping build of index");
			return;
		}

		using var indexFile = _FileSystem.File.CreateText(_FileSystem.Path.Combine(WorkingDirectory, OutputPath, "index.html"));
		using var indexLayout = _FileSystem.File.OpenText(_FileSystem.Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", "index.html"));

		var outContent = indexLayout.ReadToEnd();

		// If Config.Link is set, add a reference to the RSS file
		if (Config.Link != null)
		{
			outContent = outContent.Replace("</head>", $"""<link rel="alternate" type="application/rss+xml" title="{Config.Title}" href="rss.xml" /></head>""");
		}

		outContent = ApplyMacros(outContent, _FileSystem.DirectoryInfo.New(WorkingDirectory), Config, Logger);

		// Set the title from config
		outContent = outContent.Replace("{{ Title }}", Config.Title);

		// Load the first 10 articles on the index page
		Logger.Log($"Found {_Posts.Count()} posts to format");
		var orderedPosts = _Posts.Where(p => !p.Frontmatter.Draft).OrderByDescending(p => p.Frontmatter.PublishDate);
		var sb = new StringBuilder();
		for (var i = 0; i < Math.Min(10, orderedPosts.Count()); i++)
		{

			var thisPost = orderedPosts.Skip(i).First();
			sb.AppendLine($"""<h2 class="postTitle"><a href="{thisPost.Filename}">{thisPost.Frontmatter.Title}</a></h2>""");
			sb.AppendLine($"<h4>Written By: {thisPost.Frontmatter.Author}</h4>");
			sb.AppendLine($"<h5>Published: {thisPost.Frontmatter.PublishDate}</h5>");

			sb.AppendLine(thisPost.Abstract);

		}

		outContent = outContent.Replace("{{ Body }}", sb.ToString());
		outContent = MinifyOutput ? Minify(outContent) : outContent;

		indexFile.Write(outContent);
		indexFile.Close();

	}

	internal void BuildPages()
	{

		var outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "pages")).Exists;
		if (!outValue)
		{
			Logger.Log("Pages folder does not exist... skipping");
			return;
		}

		// TODO: Build the static pages

	}

	internal void BuildPosts()
	{

		var postsFolder = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "posts"));
		var outputFolder = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, OutputPath, "posts"));
		if (!outputFolder.Exists) outputFolder.Create();

		// Load layout for post
		var layoutText = _FileSystem.File.ReadAllText(_FileSystem.Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", "posts.html"));

		foreach (var post in postsFolder.GetFiles("*.md"))
		{

			// Skip if the post has not been updated since the last build
			if (!Force && post.LastWriteTimeUtc < (_LastBuild?.Timestamp ?? DateTime.MinValue)) continue;

			(string html, string mdHTML, Frontmatter fm) = BuildPost(post, layoutText, Config, _FileSystem.DirectoryInfo.New(WorkingDirectory), Logger);

			string outputHTML = MinifyOutput ? Minify(html) : html;

			// Identify the output file name
			var baseName = _FileSystem.Path.Combine(post.Name[0..^3] + ".html");
			var fileName = _FileSystem.Path.Combine(outputFolder.FullName, baseName);

			_FileSystem.File.WriteAllText(fileName, outputHTML);

			_Posts.Add(new PostData
			{
				Abstract = mdHTML,
				Filename = $"posts/{baseName}",
				Frontmatter = fm,
				LastUpdate = post.LastWriteTimeUtc
			});

		}


	}

	internal static (string fullHTML, string postHTML, Frontmatter fm)
		BuildPost(
			IFileInfo postFile,
			string layoutText,
			Config config,
			IDirectoryInfo workingDirectory,
			ILogger logger)
	{

		var txt = postFile.FileSystem.File.ReadAllText(postFile.FullName, Encoding.UTF8);
		return BuildPost(txt, layoutText, config, workingDirectory, logger);

	}

	internal static (string fullHTML, string postHTML, Frontmatter fm)
		BuildPost(
			string mdContents,
			string layoutText,
			Config config,
			IDirectoryInfo workingDirectory,
			ILogger logger)
	{

		var doc = Markdig.Markdown.Parse(mdContents, pipeline);
		var fm = mdContents.GetFrontMatter<Frontmatter>();

		var thisLayout = InsertHeadContent(fm, layoutText, config);

		var mdHTML = Markdig.Markdown.ToHtml(doc, pipeline);

		string outputHTML = thisLayout.Replace("{{ Body }}", mdHTML);
		outputHTML = ApplyMacros(outputHTML, workingDirectory, config, logger);
		outputHTML = fm?.Format(outputHTML) ?? outputHTML;

		return (outputHTML, mdHTML, fm);

	}

	internal static string InsertHeadContent(Frontmatter fm, string layout, Config config)
	{

		var workingText = layout;

		if (fm == null) return workingText;

		var ogHeaders = $"""
        <meta property="og:title" content="{fm.Title}">
        <meta property="og:description" content="{fm.Description}">
        <meta property="og:image" content="{fm.Preview}">
        <meta property="og:type" content="website">
        <meta property="fb:app_id" content="{config.FacebookId}">
      </head>
      """;

		//$"< meta property = "og:url" content = "https://kliptok.com/@Model.Channel.DisplayName.ToLowerInvariant()" >
		//< meta property = "og:site_name" content = "KlipTok - Social Media Fun for Twitch Clips" >

		workingText = workingText.Replace("</head>", ogHeaders, StringComparison.InvariantCultureIgnoreCase);

		var twitterHeaders = $"""
        <meta name="twitter:card" content="summary_large_image">
        <meta name="twitter:title" content="{fm.Title}">
        <meta name="twitter:description" content="{fm.Description}">
        <meta name="twitter:image" content="{fm.Preview}">
      </head>
    """;


		workingText = workingText.Replace("</head>", twitterHeaders, StringComparison.InvariantCultureIgnoreCase);

		return workingText;

	}

	public void BuildRss()
	{

		if (Config.Link == null)
		{
			Logger.Log("RSS link is not configured... skipping");
			return;
		}

		var rssHeader = $"""
      <rss version="2.0">
        <channel>
          <title>{Config.Title}</title>
          <link>{Config.Link}</link>
          <description>{Config.Description}</description>
          <language>en-us</language>
          <lastBuildDate>{DateTime.UtcNow.ToString("r")}</lastBuildDate>
          <ttl>60</ttl>
          <copyright>Copyright {DateTime.UtcNow.Year} {Config.Owner}</copyright>
      """;
		var rssHeader2 = $"""
      <managingEditor>{Config.EditorEmail}</managingEditor>
      <webMaster>{Config.WebmasterEmail}</webMaster>
      <pubDate>{DateTime.UtcNow.ToString("r")}</pubDate>
      <docs>http://blogs.law.harvard.edu/tech/rss</docs>
      <generator>Fritz.StaticWeb</generator>
    """;

		var rssItems = new List<string>();
		foreach (var post in _Posts.Where(p => !p.Frontmatter.Draft).OrderByDescending(p => p.Frontmatter.PublishDate))
		{

			var posH2 = post.Abstract.IndexOf("<h2");
			posH2 = posH2 < 0 ? post.Abstract.Length : posH2;
			var postAbstract = post.Abstract.Substring(0, posH2).Trim();
			var rssItem = $"""
        <item>
          <title>{post.Frontmatter.Title}</title>
          <link>{Config.Link}/posts/{post.Filename}</link>
          <description>{postAbstract}</description>
          <pubDate>{post.Frontmatter.PublishDate.ToString("r")}</pubDate>
          <guid>{Config.Link}/posts/{post.Filename}</guid>
        </item>
      """;
			rssItems.Add(rssItem);

		}

		var rssFooter = "\n</channel>\n</rss>";

		var rssFilename = _FileSystem.Path.Combine(WorkingDirectory, OutputPath, "rss.xml");

		using var rssFile = _FileSystem.File.Create(rssFilename);
		Logger.Log($"Writing to RSS file at: {rssFile.Name}");
		rssFile.Write(Encoding.UTF8.GetBytes(rssHeader));
		rssFile.Write(Encoding.UTF8.GetBytes(rssHeader2));

		foreach (var item in rssItems)
		{
			rssFile.Write(Encoding.UTF8.GetBytes(item));
		}

		rssFile.Write(Encoding.UTF8.GetBytes(rssFooter));
		rssFile.Flush();
		rssFile.Close();

	}

	/// <summary>
	/// Build the archive.html page that contains a list of all posts on the site
	/// </summary>
	internal void BuildArchive()
	{


		var layoutInfo = _FileSystem.FileInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", ArchiveFileName));
		if (!layoutInfo.Exists)
		{
			Logger.Log("Layout for archive page missing - skipping");
			return;
		}

		using var archiveFile = _FileSystem.File.CreateText(_FileSystem.Path.Combine(WorkingDirectory, OutputPath, ArchiveFileName));
		using var archiveLayout = _FileSystem.File.OpenText(_FileSystem.Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", ArchiveFileName));

		var outContent = archiveLayout.ReadToEnd();

		// Set the title from config
		outContent = ApplyMacros(outContent, _FileSystem.DirectoryInfo.New(WorkingDirectory), Config, Logger);
		outContent = outContent.Replace("{{ Title }}", Config.Title);

		var orderedPosts = _Posts.Where(p => !p.Frontmatter.Draft).OrderByDescending(p => p.Frontmatter.PublishDate);
		var sb = new StringBuilder();
		var years = orderedPosts.Select(o => o.Frontmatter.PublishDate.Year).Distinct().OrderByDescending(o => o);

		foreach (var thisYear in years)
		{

			sb.AppendLine($"<h2>{thisYear}</h2>");
			sb.AppendLine("<ul>");
			foreach (var thisPost in orderedPosts.Where(o => o.Frontmatter.PublishDate.Year == thisYear))
			{

				sb.AppendLine($"<li>{thisPost.Frontmatter.PublishDate.ToString("yyyy-MM-dd")} - <a href=\"{thisPost.Filename}\">{thisPost.Frontmatter.Title}</a></li>");

			}
			sb.AppendLine("</ul>");

		}

		outContent = outContent.Replace("{{ Body }}", sb.ToString());
		outContent = MinifyOutput ? Minify(outContent) : outContent;

		archiveFile.Write(outContent);
		archiveFile.Close();


	}

	private static string Minify(string html)
	{

		var settings = new NUglify.Html.HtmlSettings();
		settings.KeepTags.UnionWith(new string[] { "html", "head", "body" });

		var result = NUglify.Uglify.Html(html, settings);
		if (result.HasErrors)
		{
			throw new Exception("NUglify has errors: " + result.Errors[0].ToString());
		}
		html = result.Code;

		return html;

	}

	internal void DeployWwwRoot()
	{

		var themeFolder = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "themes", Config.Theme, "wwwroot"));
		if (!_FileSystem.Directory.Exists(_FileSystem.Path.Combine(WorkingDirectory, "wwwroot")) && !themeFolder.Exists) return;

		var wwwFolder = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, "wwwroot"));
		var targetFolder = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, OutputPath));

		if (themeFolder.Exists)
		{

			foreach (var item in themeFolder.GetDirectories())
			{
				CopyFolder(targetFolder, item);
			}

			foreach (var item in themeFolder.GetFiles())
			{
				_FileSystem.File.Copy(item.FullName, _FileSystem.Path.Combine(targetFolder.FullName, item.Name), true);
			}
		}

		if (wwwFolder.Exists)
		{

			foreach (var item in wwwFolder.GetDirectories())
			{
				CopyFolder(targetFolder, item);
			}

			foreach (var item in wwwFolder.GetFiles())
			{
				_FileSystem.File.Copy(item.FullName, _FileSystem.Path.Combine(targetFolder.FullName, item.Name), true);
			}

		}

	}

	/// <summary>
	/// Recursive function to copy folder contents
	/// </summary>
	/// <param name="target"></param>
	/// <param name="source"></param>
	private void CopyFolder(IDirectoryInfo target, IDirectoryInfo source)
	{

		if (!_FileSystem.Directory.Exists(_FileSystem.Path.Combine(target.FullName, source.Name)))
		{
			_FileSystem.Directory.CreateDirectory(_FileSystem.Path.Combine(target.FullName, source.Name));
		}

		var targetFolder = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(target.FullName, source.Name));
		foreach (var item in source.GetDirectories())
		{
			CopyFolder(targetFolder, item);
		}

		foreach (var item in source.GetFiles())
		{
			_FileSystem.File.Copy(item.FullName, _FileSystem.Path.Combine(targetFolder.FullName, item.Name), true);
		}

	}

	private static string ApplyMacros(string initialHTML, IDirectoryInfo workingDirectory, Config config, ILogger logger)
	{

		var outHTML = initialHTML;
		outHTML = HandleIncludes(outHTML, workingDirectory, config, logger);
		outHTML = outHTML.Replace("{{ CurrentYear }}", DateTime.Now.Year.ToString());
		outHTML = outHTML.Replace("{{ ArchiveURL }}", ArchiveFileName);


		return outHTML;

	}

	private static string HandleIncludes(string outHTML, IDirectoryInfo workingDirectory, Config config, ILogger logger)
	{

		var includeRegex = new Regex(@"\{\{ Include:([a-zA-Z0-9\-_\.]+) \}\}");
		var workingHTML = outHTML;
		var matches = includeRegex.Matches(workingHTML);
		foreach (Match match in matches)
		{

			var includeFile = match.Groups[1].Value;
			var includePath = workingDirectory.FileSystem.Path.Combine(workingDirectory.FullName, "themes", config.Theme, "includes", $"{includeFile}.html");
			if (!workingDirectory.FileSystem.File.Exists(includePath))
			{
				logger.Log($"Include file {includePath} not found - skipping");
				workingHTML = workingHTML.Replace(match.Value, string.Empty);
				continue;
			}

			var includeContent = workingDirectory.FileSystem.File.ReadAllText(includePath);
			workingHTML = workingHTML.Replace(match.Value, includeContent);

		}

		return workingHTML;

	}

	private void SaveLastBuild()
	{
		_LastBuild.Timestamp = DateTime.UtcNow;
		var outText = JsonSerializer.Serialize(_LastBuild);
		_FileSystem.File.WriteAllText(_FileSystem.Path.Combine(WorkingDirectory, LastBuildFilename), outText);
	}

}

