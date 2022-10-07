namespace Test.StaticBlog.GivenValidActionBuild;

[Collection("Archive construction")]
public class WhenBuildingArchive : BaseFixture
{
  private const string ArchiveFileName = "archive.html";

  public WhenBuildingArchive(ITestOutputHelper output) : base()
	{ 

		// Reset by deleting the index file
		File.Delete(Path.Combine(OutputFolder.FullName, ArchiveFileName));

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
		_sut.BuildArchive();

		Output = output;
		Output.WriteLine(OutputFolder.FullName);
		Output.WriteLine($"Archive file location: {Path.Combine(OutputFolder?.FullName, ArchiveFileName)}");
		_ArchiveFile = OutputFolder.GetFiles(ArchiveFileName).FirstOrDefault();
    
	}

  private FileInfo _ArchiveFile;

  public ITestOutputHelper Output { get; }

  [Fact]
  public void ShouldCreateArchiveHtml()
  {

    Assert.NotNull(_ArchiveFile);

  }

  [Fact]
  public void ShouldApplyMacros()
  {

    var html = File.ReadAllText(_ArchiveFile.FullName);
    Assert.DoesNotContain("{{ CurrentYear }}", html);
    Assert.Contains($"<span>Year: {DateTime.Now.Year}</span>", html);

  }


}