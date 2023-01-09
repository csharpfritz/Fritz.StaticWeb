using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace Fritz.StaticBlog.adminweb.Pages
{
	public class EditPostModel : PageModel
	{
		private readonly IConfiguration _Configuration;

		public EditPostModel(IConfiguration configuration)
    {
			_Configuration = configuration;
		}

    public string ErrorMessage { get; set; }

    public Frontmatter Frontmatter { get; set; }

		public string Markdown { get; set; }

    public void OnGet(string? articleTitle)
		{

			if (string.IsNullOrEmpty(articleTitle)) return;

      var workingFolder = Path.Combine(_Configuration[WebsiteConfig.PARM_WORKINGDIRECTORY], "posts");
			if (!Path.Exists(workingFolder))
			{
				ErrorMessage = $"The working folder {workingFolder} does not exist";
				return;
			}

			var fileName = Path.Combine(workingFolder, articleTitle);
			if (!System.IO.File.Exists(fileName))
			{
				ErrorMessage = $"The file {fileName} does not exist";
				return;
			}

			var txt = System.IO.File.ReadAllText(fileName, Encoding.UTF8);
			var doc = Markdig.Markdown.Parse(txt, ActionBuild.pipeline);
			Frontmatter = txt.GetFrontMatter<Frontmatter>();
			Markdown = txt.Split("---", 3)[2].Trim();

		}

	}
}
