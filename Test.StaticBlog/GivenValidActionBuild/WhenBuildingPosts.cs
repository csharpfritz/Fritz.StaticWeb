namespace Test.StaticBlog.GivenValidActionBuild;

public class WhenBuildingPosts : BaseFixture {

  public WhenBuildingPosts(ITestOutputHelper helper)
  {
		_sut.Logger = new XUnitLogger(helper);

		FileSystem.AddFile(FileSystem.Path.Combine(PostsFolder.FullName, "1-FirstPost.md"), new MockFileData("""
		---
		draft: true
		title: First post!
		author: Testy McTestFace
		publishdate: 2021-05-22 14:22
		---

		This is my first post
		"""));

  }

  [Fact]
	public void ShouldNotThrowError() {

		_sut.BuildPosts();

	}

	[Fact()]
	public void ShouldWriteTheFirstPost() {

		// arrange
		// Clear the folder to ensure this file is written
		//if (OutputPostsFolder.Exists) {
		//	var files = OutputPostsFolder.GetFiles();
		//	foreach (var f in files)
		//	{
		//			f.Delete();
		//	}
		//}

		// act
		_sut.BuildPosts();

		// assert
		OutputPostsFolder.Refresh();
		Assert.NotEmpty(OutputPostsFolder.GetFiles());

		var inspectFile = OutputPostsFolder.GetFiles("*.html").First();
		var html = FileSystem.File.ReadAllText(inspectFile.FullName);
		Assert.DoesNotContain("draft: true", html);

	}

	[Fact()]
	public void ShouldNotIncludeFrontmatter() {

		// act
		_sut.BuildPosts();

		// assert
		var inspectFile = OutputPostsFolder.GetFiles("*.html").First();
		var html = FileSystem.File.ReadAllText(inspectFile.FullName);
		Assert.DoesNotContain("draft: true", html);

	}

	[Fact]
	public void ShouldUseLayout() {

		// act
		_sut.BuildPosts();

		// assert
		var inspectFile = OutputPostsFolder.GetFiles("*.html").First();
		var html = FileSystem.File.ReadAllText(inspectFile.FullName);
		Assert.Contains("<html>", html);
		Assert.Contains("<body>", html);


	}

  [Fact]
  public void ShouldInsertTitle()
  {

    // act
    _sut.BuildPosts();

    // assert
    var inspectFile = OutputPostsFolder.GetFiles("*.html").OrderBy(f => f.Name).First();
    var html = FileSystem.File.ReadAllText(inspectFile.FullName);
    Assert.Contains("<title>First post!</title>", html);


  }

  [Fact]
  public void ShouldInsertAuthor()
  {

    // act
    _sut.BuildPosts();

    // assert
    var inspectFile = OutputPostsFolder.GetFiles("*.html").OrderBy(f => f.Name).First();
    var html = FileSystem.File.ReadAllText(inspectFile.FullName);
    Assert.Contains("<h3>Author: Testy McTestFace</h3>", html);

  }

  [Fact]
	public void ShouldAddToPostsForIndex() {

		// act
		_sut.BuildPosts();

		// assert
		Assert.NotEmpty(_sut._Posts);

	}

	[Fact]
	public void ShouldAddToPostsForIndexWithPostsPathInFilename()
	{

		// act
		_sut.BuildPosts();

		// Assert
		var firstPost = _sut._Posts.First();
		Assert.StartsWith("posts/", firstPost.Filename);

	}

	[Fact]
	public void ShouldApplyMacros()
	{

		// act
		_sut.BuildPosts();

		// assert
		var inspectFile = OutputPostsFolder.GetFiles("*.html").OrderBy(f => f.Name).First();
		var html = FileSystem.File.ReadAllText(inspectFile.FullName);
		Assert.DoesNotContain("{{ CurrentYear }}", html);
		Assert.Contains($"<span>Year: {DateTime.Now.Year}</span>", html);

	}

}