using YamlDotNet.Serialization;

namespace Fritz.StaticBlog.Data;

public class Frontmatter
{

	[YamlMember(Alias = "title")]
	public string Title { get; set; }

	[YamlMember(Alias = "draft")]
	public bool Draft { get; set; }

  [YamlMember(Alias = "author")]
  public string Author { get; set; } = string.Empty;

	[YamlMember(Alias = "publishdate")]
	public DateTime PublishDate { get; set; }

	[YamlMember(Alias = "description")]
	public string Description { get; set; } = string.Empty;

	[YamlMember(Alias = "preview")]
	public string Preview { get; set; }

	internal string Format(string sampleText)
	{

		var outText = sampleText.Clone().ToString();
		outText = outText.Replace("{{ PublishDate }}", PublishDate.ToString());
		outText = outText.Replace("{{ Title }}", Title);
    outText = outText.Replace("{{ Author }}", Author);

		return outText;

	}
}

