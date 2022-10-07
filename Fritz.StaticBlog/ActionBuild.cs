using System.Text;
using System.Text.Json;
using CommandLine;
using Markdig;
using Markdig.Prism;

namespace Fritz.StaticBlog;

[Verb("build", HelpText = "Build the website")]
public class ActionBuild : ActionBase, ICommandLineAction
{

	internal List<PostData> _Posts = new();
	internal LastBuild _LastBuild;

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

	public override int Execute()
	{

		Console.WriteLine($"Outputting to: {OutputPath}");

		var outValue = base.Execute();
		if (outValue > 0) return outValue;

		System.Console.WriteLine($"Building in folder {WorkingDirectory} and distributing to {Path.Combine(WorkingDirectory, OutputPath)}");

		BuildPosts();

		BuildPages();

		BuildRss();

		BuildIndex();

		DeployWwwRoot();

		SaveLastBuild();

		return 0;

	}

	public override bool Validate()
	{

		var outputDir = new DirectoryInfo(Path.Combine(WorkingDirectory, OutputPath));
		var outValue = outputDir.Exists;

		if (!outValue) System.Console.WriteLine($"Output folder '{outputDir.FullName}' does not exist");
		if (outValue)
		{
			outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "themes")).Exists;
			if (!outValue) System.Console.WriteLine("themes folder is missing");
		}

		if (outValue)
		{
			outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "posts")).Exists;
			if (!outValue) System.Console.WriteLine("posts folder is missing");
		}

		/*  -- Making pages folder optional --
if (outValue)
{
outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "pages")).Exists;
if (!outValue) System.Console.WriteLine("pages folder is missing");
}
**/

		if (outValue)
		{
			outValue = new FileInfo(Path.Combine(WorkingDirectory, "config.json")).Exists;
			if (!outValue) System.Console.WriteLine($"config.json file is missing");
		}

		// Validate LastBuild configuration
		if (outValue)
		{
			var exists = new FileInfo(Path.Combine(WorkingDirectory, LastBuildFilename)).Exists;
			if (!exists)
			{
				System.Console.WriteLine($"LastBuild file is missing - Complete build requested");
				_LastBuild = new LastBuild { Timestamp = DateTime.MinValue };
			}
			else
			{
				var file = File.OpenRead(Path.Combine(WorkingDirectory, LastBuildFilename));
				_LastBuild = JsonSerializer.DeserializeAsync<LastBuild>(file).GetAwaiter().GetResult();
				file.Dispose();
			}
		}

		return outValue;

	}

	internal void BuildIndex()
	{

		if (!Force && !_Posts.Any(p => p.LastUpdate > _LastBuild?.Timestamp))
		{
			Console.WriteLine("No new posts found.  Skipping build of index");
			return;
		}

		using var indexFile = File.CreateText(Path.Combine(WorkingDirectory, OutputPath, "index.html"));
		using var indexLayout = File.OpenText(Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", "index.html"));

		var outContent = indexLayout.ReadToEnd();

		// Set the title from config
		outContent = outContent.Replace("{{ Title }}", Config.Title);

		// If Config.Link is set, add a reference to the RSS file
		if (Config.Link != null)
		{
			outContent = outContent.Replace("</head>", $"<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{Config.Title}\" href=\"rss.xml\" /></head>");
		}

		outContent = ApplyMacros(outContent);

		// Load the first 10 articles on the index page
		Console.WriteLine($"Found {_Posts.Count()} posts to format");
		var orderedPosts = _Posts.Where(p => !p.Frontmatter.Draft).OrderByDescending(p => p.Frontmatter.PublishDate);
		var sb = new StringBuilder();
		for (var i = 0; i < Math.Min(10, orderedPosts.Count()); i++)
		{

			var thisPost = orderedPosts.Skip(i).First();
			sb.AppendLine($"<h2><a href=\"{thisPost.Filename}\">{thisPost.Frontmatter.Title}</a></h2>");

			sb.AppendLine(thisPost.Abstract);

		}

		outContent = outContent.Replace("{{ Body }}", sb.ToString());
		outContent = Minify(outContent);

		indexFile.Write(outContent);
		indexFile.Close();

	}

	internal void BuildPages()
	{

		var outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "pages")).Exists;
		if (!outValue)
		{
			Console.WriteLine("Pages folder does not exist... skipping");
			return;
		}

		// TODO: Build the static pages

	}

	internal void BuildPosts()
	{

		var postsFolder = new DirectoryInfo(Path.Combine(WorkingDirectory, "posts"));
		var outputFolder = new DirectoryInfo(Path.Combine(WorkingDirectory, OutputPath, "posts"));
		if (!outputFolder.Exists) outputFolder.Create();

		var pipeline = new MarkdownPipelineBuilder()
						.UseAdvancedExtensions()
						.UseYamlFrontMatter()
						.UsePrism()
						.Build();

		// Load layout for post
		var layoutText = File.ReadAllText(Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", "posts.html"));

		foreach (var post in postsFolder.GetFiles("*.md"))
		{

			var txt = File.ReadAllText(post.FullName, Encoding.UTF8);

			var baseName = Path.Combine(post.Name[0..^3] + ".html");
			var fileName = Path.Combine(outputFolder.FullName, baseName);

			var doc = Markdig.Markdown.Parse(txt, pipeline);
			var fm = txt.GetFrontMatter<Frontmatter>();

			var thisLayout = InsertHeadContent(fm, layoutText);

			var mdHTML = Markdig.Markdown.ToHtml(doc, pipeline);

			if (Force || post.LastWriteTimeUtc > (_LastBuild?.Timestamp ?? DateTime.MinValue))
			{

				string outputHTML = thisLayout.Replace("{{ Body }}", mdHTML);
				outputHTML = fm.Format(outputHTML);
				outputHTML = ApplyMacros(outputHTML);
				outputHTML = Minify(outputHTML);

				File.WriteAllText(fileName, outputHTML);

			}

			_Posts.Add(new PostData
			{
				Abstract = mdHTML,
				Filename = $"posts/{baseName}",
				Frontmatter = fm,
				LastUpdate = post.LastWriteTimeUtc
			});

		}


	}

	internal string InsertHeadContent(Frontmatter fm, string layout) 
	{

		var workingText = layout;

		var ogHeaders = $"<meta property=\"og:title\" content=\"{fm.Title}\">\n" +
			$"<meta property=\"og:description\" content=\"{fm.Description}\" >\n" +
			$"<meta property=\"og:image\" content=\"{fm.Preview}\">" +
			"<meta property=\"og:type\" content=\"website\">" +
			$"<meta property=\"fb:app_id\" content=\"{Config.FacebookId}\">" +
			"</head>";
		//$"< meta property = "og:url" content = "https://kliptok.com/@Model.Channel.DisplayName.ToLowerInvariant()" >
		//< meta property = "og:site_name" content = "KlipTok - Social Media Fun for Twitch Clips" >

		workingText = workingText.Replace("</head>", ogHeaders, StringComparison.InvariantCultureIgnoreCase);

		var twitterHeaders = $"\t<meta name=\"twitter:card\" content=\"summary_large_image\">\r\n\t" +
			// $"<meta name=\"twitter:site\" content=\"@@thekliptok\">\r\n\t" +
			$"<meta name=\"twitter:title\" content=\"{fm.Title}\">\r\n\t" +
			$"<meta name=\"twitter:description\" content=\"{fm.Description}\">\r\n\t" +
			$"<meta name=\"twitter:image\" content=\"{fm.Preview}\">\r\n" +
			"</head>";


		workingText = workingText.Replace("</head>", twitterHeaders, StringComparison.InvariantCultureIgnoreCase);

		return workingText;

	}

	public void BuildRss()
	{

		if (Config.Link == null)
		{
			Console.WriteLine("RSS link is not configured... skipping");
			return;
		}

		var rssHeader = $"<rss version=\"2.0\">\n<channel>\n<title>{Config.Title}</title>\n<link>{Config.Link}</link>\n<description>{Config.Description}</description>\n<language>en-us</language>\n<lastBuildDate>{DateTime.UtcNow.ToString("r")}</lastBuildDate>\n<ttl>60</ttl>\n<copyright>Copyright {DateTime.UtcNow.Year} {Config.Owner}</copyright>\n";
		var rssHeader2 = $"<managingEditor>{Config.EditorEmail}</managingEditor>\n<webMaster>{Config.WebmasterEmail}</webMaster>\n<pubDate>{DateTime.UtcNow.ToString("r")}</pubDate>\n<docs>http://blogs.law.harvard.edu/tech/rss</docs>\n<generator>Fritz.StaticWeb</generator>\n";

		var rssItems = new List<string>();
		foreach (var post in _Posts.Where(p => !p.Frontmatter.Draft).OrderByDescending(p => p.Frontmatter.PublishDate))
		{

			var posH2 = post.Abstract.IndexOf("<h2");
			posH2 = posH2 < 0 ? post.Abstract.Length : posH2;
			var postAbstract = post.Abstract.Substring(0, posH2).Trim();
			var rssItem = $"<item>\n<title>{post.Frontmatter.Title}</title>\n<link>{Config.Link}/posts/{post.Filename}</link>\n<description>{postAbstract}</description>\n<pubDate>{post.Frontmatter.PublishDate.ToString("r")}</pubDate>\n<guid>{Config.Link}/posts/{post.Filename}</guid>\n</item>\n";
			rssItems.Add(rssItem);

		}

		var rssFooter = "\n</channel>\n</rss>";

		var rssFilename = Path.Combine(WorkingDirectory, OutputPath, "rss.xml");
		File.Delete(rssFilename);

		using var rssFile = File.OpenWrite(rssFilename);
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

	private string Minify(string html)
	{

		if (!MinifyOutput) return html;

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

		var themeFolder = new DirectoryInfo(Path.Combine(WorkingDirectory, "themes", Config.Theme, "wwwroot"));
		if (!Directory.Exists(Path.Combine(WorkingDirectory, "wwwroot")) && !themeFolder.Exists) return;

		var wwwFolder = new DirectoryInfo(Path.Combine(WorkingDirectory, "wwwroot"));
		var targetFolder = new DirectoryInfo(Path.Combine(WorkingDirectory, OutputPath));

		if (themeFolder.Exists)
		{

			foreach (var item in themeFolder.GetDirectories())
			{
				CopyFolder(targetFolder, item);
			}

			foreach (var item in themeFolder.GetFiles())
			{
				File.Copy(item.FullName, Path.Combine(targetFolder.FullName, item.Name), true);
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
				File.Copy(item.FullName, Path.Combine(targetFolder.FullName, item.Name), true);
			}

		}

	}

	/// <summary>
	/// Recursive function to copy folder contents
	/// </summary>
	/// <param name="target"></param>
	/// <param name="source"></param>
	private void CopyFolder(DirectoryInfo target, DirectoryInfo source)
	{

		if (!Directory.Exists(Path.Combine(target.FullName, source.Name)))
		{
			Directory.CreateDirectory(Path.Combine(target.FullName, source.Name));
		}

		var targetFolder = new DirectoryInfo(Path.Combine(target.FullName, source.Name));
		foreach (var item in source.GetDirectories())
		{
			CopyFolder(targetFolder, item);
		}

		foreach (var item in source.GetFiles())
		{
			File.Copy(item.FullName, Path.Combine(targetFolder.FullName, item.Name), true);
		}

	}

	private string ApplyMacros(string initialHTML)
	{

		var outHTML = initialHTML;
		outHTML = outHTML.Replace("{{ CurrentYear }}", DateTime.Now.Year.ToString());

		return outHTML;

	}

	private void SaveLastBuild()
	{
		var outText = JsonSerializer.Serialize(_LastBuild);
		File.WriteAllText(Path.Combine(WorkingDirectory, LastBuildFilename), outText);
	}


}

