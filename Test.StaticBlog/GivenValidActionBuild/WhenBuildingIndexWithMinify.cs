namespace Test.StaticBlog.GivenValidActionBuild;

[Collection("Index construction")]
public class WhenBuildingIndexWithMinify : BaseFixture
{

	public WhenBuildingIndexWithMinify(ITestOutputHelper output)
	{

		_sut.Logger = new XUnitLogger(output);

		// Reset by deleting the index file
		//File.Delete(Path.Combine(OutputFolder.FullName, "index.html"));

		_sut._Posts.Add(new PostData {
			Filename = "first_post.html",
			Frontmatter = new Frontmatter {
				Draft = false,
				PublishDate = DateTime.Today.AddDays(-1),
				Title = "First post!"
			},
			Abstract = "This is my first post",
			LastUpdate = DateTime.Today.AddDays(-1)
		});

		_sut.MinifyOutput = true;
		_sut.Validate();
		_sut.BuildIndex();

		Output = output;
		Output.WriteLine(OutputFolder.FullName);
		Output.WriteLine($"Index file location: {Path.Combine(OutputFolder?.FullName, "index.html")}");
		_IndexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
	}

	private IFileInfo _IndexFile;

	public ITestOutputHelper Output { get; }

	[Fact]
	public void ShouldContainHtml() {

		var contents = FileSystem.File.ReadAllText(_IndexFile.FullName);

		Assert.Contains("</html>", contents);

	}

	[Fact]
	public void ShouldCreateIndexHtml() { 

		Assert.NotNull(_IndexFile);

	}

}

