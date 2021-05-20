using System;
using System.IO;
using System.Text.Json;
using CommandLine;
using Markdig.Syntax;

namespace Fritz.StaticBlog
{

	[Verb("build", HelpText="Build the website")]
	public class ActionBuild : ICommandLineAction
	{

		[Option('f', "force", Default = (bool)false)]
		public bool Force { get; set; }

		[Option('o', "output", Required=true, HelpText="Location to write out the rendered site")]
		public string OutputPath { get; set; }

		[Option('d', "directory", Required=false, HelpText="The directory to run the build against.  Default current directory")]
		public string WorkingDirectory { get; set; } = ".";

		internal Config Config { get; private set; }

		public int Execute()
		{

			if (!Validate()) return 1;

			System.Console.WriteLine($"Building in folder {WorkingDirectory} and distributing to {Path.Combine(WorkingDirectory, OutputPath)}");

			BuildPosts();

			BuildPages();

			BuildIndex();

			return 0;

		}

		public bool Validate()
		{

			var outValue = true;
			
			var outputDir = new DirectoryInfo(Path.Combine(WorkingDirectory, OutputPath));
			outValue = outputDir.Exists;
			if (!outValue) System.Console.WriteLine($"Output folder '{outputDir.FullName}' does not exist");

			if (outValue) {
				outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "themes")).Exists;
				if (!outValue) System.Console.WriteLine("themes folder is missing");
			}

			if (outValue) {
				outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "posts")).Exists;
				if (!outValue) System.Console.WriteLine("posts folder is missing");
			} 

			if (outValue) {
				outValue = new DirectoryInfo(Path.Combine(WorkingDirectory, "pages")).Exists;
				if (!outValue) System.Console.WriteLine("pages folder is missing");
			}

			if (outValue) {
				outValue = new FileInfo(Path.Combine(WorkingDirectory, "config.json")).Exists;
				if (!outValue) System.Console.WriteLine($"config.json file is missing");
			}

			if (outValue)	outValue = ValidateConfig();

			return outValue;

		}

		private bool ValidateConfig()
		{

			try {
				
				var rdr = File.OpenRead(Path.Combine(WorkingDirectory, "config.json"));
				var json = new StreamReader(rdr).ReadToEnd();
				this.Config = JsonSerializer.Deserialize<Config>(json);
			} catch (Exception ex) {
				System.Console.WriteLine($"Error while reading config: {ex.Message}");
				return false;
			}

			if (!Directory.Exists(Path.Combine(WorkingDirectory, "themes", Config.Theme))) {
				System.Console.WriteLine($"Theme folder '{Config.Theme}' does not exist");
				return false;
			}

			return true;

		}

		internal void BuildIndex()
		{
			throw new NotImplementedException();
		}

		internal void BuildPages()
		{
			throw new NotImplementedException();
		}

		internal void BuildPosts()
		{
			
			var postsFolder = new DirectoryInfo(Path.Combine(WorkingDirectory, "posts"));

			foreach (var post in postsFolder.GetFiles("*.md"))
			{
				
				var txt = File.OpenText(post.FullName);
				var doc = Markdig.Markdown.Parse(txt.ReadToEnd());

				// File.OpenWrite(Path.Combine()) Markdig.Markdown.ToHtml(doc);

			}


		}



	}

}
