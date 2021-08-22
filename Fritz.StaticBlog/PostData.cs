using System;

namespace Fritz.StaticBlog
{

	public class PostData 
	{

		public Frontmatter Frontmatter { get; set; }

		public DateTime LastUpdate { get; set; }
		
		public string Filename { get; set; }

		public string Abstract { get; set; }

	}
		
}