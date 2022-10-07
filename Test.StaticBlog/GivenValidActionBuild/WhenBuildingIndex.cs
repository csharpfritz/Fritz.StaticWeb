namespace Test.StaticBlog.GivenValidActionBuild;

[Collection("Index construction")]
public class WhenBuildingIndex : BaseFixture
{

	public WhenBuildingIndex(ITestOutputHelper output) : base()
	{ 

		// Reset by deleting the index file
		File.Delete(Path.Combine(OutputFolder.FullName, "index.html"));

		_sut._Posts.Add(new PostData {
			Filename = "first_post.html",
			Frontmatter = new Frontmatter {
				Draft = false,
				PublishDate = DateTime.Today.AddDays(-1),
				Title = "First post!",
			},
			LastUpdate = DateTime.Today.AddDays(-1),
			Abstract = "This is my first post"
		});

		_sut.Validate();
		_sut.BuildIndex();

		Output = output;
		Output.WriteLine(OutputFolder.FullName);
		Output.WriteLine($"Index file location: {Path.Combine(OutputFolder?.FullName, "index.html")}");
		_IndexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
	}

  private FileInfo _IndexFile;

  public ITestOutputHelper Output { get; }

  [Fact]
  public void ShouldCreateIndexHtml()
  {

    Assert.NotNull(_IndexFile);

  }

  [Fact]
	public void ShouldApplyMacros()
	{

		var html = File.ReadAllText(_IndexFile.FullName);
		Assert.DoesNotContain("{{ CurrentYear }}", html);
		Assert.Contains($"<span>Year: {DateTime.Now.Year}</span>", html);

    // Archive URL
    Assert.DoesNotContain("{{ ArchiveURL }}", html);
    Assert.Contains($"<a href=\"archive.html\">All posts</a>", html);

  }

}