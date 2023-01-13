namespace Test.StaticBlog.GivenValidActionBuild;

public class IndexFixture : BaseFixture
{
	internal readonly IFileInfo _IndexFile;

	public IndexFixture()
	{

		_sut._Posts.Add(new PostData
		{
			Filename = "first_post.html",
			Frontmatter = new Frontmatter
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
