using System;
using System.IO;
using System.Linq;

namespace Test.StaticBlog.GivenValidActionBuild
{
	public class IndexFixture : BaseFixture
	{
		internal readonly FileInfo _IndexFile;

		public IndexFixture()
		{

			_sut._Posts.Add(new Fritz.StaticBlog.PostData
			{
				Filename = "first_post.html",
				Frontmatter = new Fritz.StaticBlog.Frontmatter
				{
					Draft = false,
					PublishDate = DateTime.Today.AddDays(-1),
					Title = "First post!"
				},
				Abstract = "This is my first post"
			});

			_sut.BuildIndex();

			_IndexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
		}

	}
}
