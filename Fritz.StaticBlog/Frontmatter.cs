using YamlDotNet.Serialization;

namespace Fritz.StaticBlog
{
		
	public class Frontmatter {

		[YamlMember(Alias = "title")]
    public string Title { get; set; }

		[YamlMember(Alias = "draft")]
    public bool Draft { get; set; }

	}

}