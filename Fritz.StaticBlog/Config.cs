using System.Text.Json.Serialization;

namespace Fritz.StaticBlog
{
	public class Config {

		[JsonPropertyName("theme")]
		public string Theme { get; set; }

	}

}
