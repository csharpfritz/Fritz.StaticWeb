using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace Fritz.StaticBlog.adminweb.Pages
{
	public class PostsModel : PageModel
	{
		private readonly IConfiguration _Configuration;

		public PostsModel(IConfiguration configuration)
		{
			_Configuration = configuration;
		}

		public List<(Frontmatter fm, string fileName)> Posts { get; set; } = new();
		public string ErrorMessage { get; private set; }
		public string ErrorMessageHeight => string.IsNullOrEmpty(ErrorMessage) ? string.Empty : " 5em";

		public void OnGet()
		{

			var workingFolder = Path.Combine(_Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY], "posts");
			
			if (!Path.Exists(workingFolder))
			{
				ErrorMessage = $"The working folder {workingFolder} does not exist";
				return;
			}

			var files = Directory.GetFiles(workingFolder, "*.md");

			Posts.Clear();
			foreach (var item in files)
			{
				var txt = System.IO.File.ReadAllText(item, Encoding.UTF8);
				var doc = Markdig.Markdown.Parse(txt, ActionBuild.pipeline);
				var fm = txt.GetFrontMatter<Frontmatter>();

				Posts.Add((fm, Path.GetFileName(item)));

			}

		}

	}
}
