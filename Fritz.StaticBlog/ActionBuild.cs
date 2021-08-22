using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CommandLine;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Prism;
using Markdig.Syntax;

namespace Fritz.StaticBlog
{

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
				if (!exists) {
					System.Console.WriteLine($"LastBuild file is missing - Complete build requested");
					_LastBuild = new LastBuild { Timestamp=DateTime.MinValue };
				} else {
					var file = File.OpenRead(Path.Combine(WorkingDirectory, LastBuildFilename));
					_LastBuild = JsonSerializer.Deserialize<LastBuild>(file);
					file.Dispose(); 
				}
			}

			return outValue;

		}

		internal void BuildIndex()
		{

			using var indexFile = File.CreateText(Path.Combine(WorkingDirectory, OutputPath, "index.html"));
			using var indexLayout = File.OpenText(Path.Combine(WorkingDirectory, "themes", Config.Theme, "layouts", "index.html"));

			var outContent = indexLayout.ReadToEnd();

			// Set the title from config
			outContent = outContent.Replace("{{ Title }}", Config.Title);

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

			foreach (var post in postsFolder.GetFiles("*.md").Where(f => f.LastWriteTimeUtc > (_LastBuild?.Timestamp ?? DateTime.MinValue)).ToArray())
			{

				var txt = File.ReadAllText(post.FullName, Encoding.UTF8);

				var baseName = Path.Combine(post.Name[0..^3] + ".html");
				var fileName = Path.Combine(outputFolder.FullName, baseName);

				var doc = Markdig.Markdown.Parse(txt, pipeline);
				var fm = txt.GetFrontMatter<Frontmatter>();
				var mdHTML = Markdig.Markdown.ToHtml(doc, pipeline);

				string outputHTML = layoutText.Replace("{{ Body }}", mdHTML);
				outputHTML = fm.Format(outputHTML);
				outputHTML = Minify(outputHTML);

				File.WriteAllText(fileName, outputHTML);

				_Posts.Add(new PostData
				{
					Abstract = mdHTML,
					Filename = $"posts/{baseName}",
					Frontmatter = fm
				});

			}


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

		private void SaveLastBuild()
		{
			var outText = JsonSerializer.Serialize(_LastBuild);
			File.WriteAllText(Path.Combine(WorkingDirectory, LastBuildFilename), outText);
		}


	}

}
