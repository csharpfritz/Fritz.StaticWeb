using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fritz.StaticBlog
{

	[Verb("create", HelpText = "Create a page or post for the site")]
	public class ActionCreate : ActionBase, ICommandLineAction
	{
		private readonly IFileSystem _FileSystem;

		[Value(0, Default = "post", HelpText = "The type of content to create.  Valid values: post, page")]
		public string ContentType { get; set; }

		[Value(1, HelpText = "The name of the file to create")]
		public string Filename { get; set; }

		[Option('o', "output", Required = false, HelpText = "Location to write out the rendered site")]
		public string OutputPath { get; set; } = ".";

		[Option('w', "directory", Required = false, HelpText = "The directory to run the build against.  Default current directory")]
		public string ThisDirectory { 
			get { return base.WorkingDirectory; }
			set { base.WorkingDirectory = value; }
		}

    public ActionCreate(IFileSystem fileSystem)
    {
			_FileSystem = fileSystem;
		}

		public ActionCreate() : this(new FileSystem()) { }

    public override int Execute()
		{

			var outValue = base.Execute();
			if (outValue > 0) return outValue;

			var contentFolder = ContentType == "post" ? "posts" : "pages";
			var targetFile = _FileSystem.Path.Combine(WorkingDirectory, OutputPath, contentFolder, Filename + ".md");

			var sb = new StringBuilder();

			var fm = new Frontmatter
			{
				Draft = true,
				PublishDate = DateTime.Now,
				Title = Filename,
        Author = "Author Name"
			};
			sb.Append(fm.Serialize());

			_FileSystem.File.WriteAllText(targetFile, sb.ToString());

			return 0;

		}

		public override bool Validate()
		{

			var outValue = true;

			if (outValue)
			{
				var validContentType = new string[] { "post", "page" };
				if (!validContentType.Contains(ContentType))
				{
					System.Console.WriteLine("ContentType must be either 'post' or 'page'");
					outValue = false;
				}
			}

			if (outValue)
			{
				var outputDir = _FileSystem.DirectoryInfo.New(_FileSystem.Path.Combine(WorkingDirectory, OutputPath));
				outValue = outputDir.Exists;
				if (!outValue) System.Console.WriteLine($"Output folder '{outputDir.FullName}' does not exist");
			}



			return outValue;

		}

	}
}
