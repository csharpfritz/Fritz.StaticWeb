using System.Text.Json.Serialization;

namespace Fritz.StaticBlog
{
	public class Config {

		[JsonPropertyName("theme")]
		public string Theme { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("link")]
		public string Link { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonPropertyName("owner")]
		public string Owner { get; set; }

		[JsonPropertyName("editor_email")]
		public string EditorEmail { get; set; }

		[JsonPropertyName("webmaster_email")]
		public string WebmasterEmail { get; set; }

	}

}
