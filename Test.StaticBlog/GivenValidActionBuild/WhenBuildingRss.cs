
using System;
using System.IO;
using System.Linq;
using System.Xml;
using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild;

public class WhenBuildingRss : BaseFixture
{

	public WhenBuildingRss()
	{
		_sut.Config.Link = "https://example.com";
		_sut.Config.Description = "This is a test website";
		// _sut.Config.
	}

	[Fact]
	public void ShouldNotThrowError()
	{

		_sut.BuildRss();

	}

	[Fact()]
	public void ShouldSkipRssFileIfLinkNotSet()
	{

		// arrange
		// Clear the folder to ensure this file is written
		if (OutputRssFile.Exists) OutputRssFile.Delete();
		_sut.Config.Link = null;

		// act
		_sut.BuildRss();

		// assert
		Assert.False(OutputRssFile.Exists);

	}

	[Fact()]
	public void ShouldWriteTheSiteHeader()
	{

		// arrange
		// Clear the folder to ensure this file is written
		if (OutputRssFile.Exists) OutputRssFile.Delete();

		// act
		_sut.BuildRss();

		// assert
		Assert.True(OutputRssFile.Exists);
		var contents = File.ReadAllText(OutputRssFile.FullName);
		Assert.Contains("<rss", contents);
		Assert.Contains("<channel", contents);
		Assert.Contains("<title>", contents);
		Assert.Contains("<link>", contents);
		Assert.Contains("<description>", contents);
		Assert.Contains("<language>", contents);
		Assert.Contains("<copyright>", contents);

		var xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(contents);

	}

	[Fact()]
	public void ShouldWriteItemsForThePosts()
	{

		// arrange
		// Clear the folder to ensure this file is written
		if (OutputRssFile.Exists) OutputRssFile.Delete();
		_sut._Posts.Add(new Fritz.StaticBlog.PostData
		{
			Filename = "first_post.html",
			Frontmatter = new Fritz.StaticBlog.Frontmatter
			{
				Draft = false,
				PublishDate = DateTime.Today.AddDays(-1),
				Title = "First post!",
			},
			LastUpdate = DateTime.Today.AddDays(-1),
			Abstract = "This is my first post"
		});

		// act
		_sut.BuildPosts();
		_sut.BuildRss();

		// assert
		var contents = File.ReadAllText(OutputRssFile.FullName);
		Assert.Contains("<item>", contents);

	}

	[Fact()]
	public void ShouldCutPostAtH2()
	{

		// arrange
		// Clear the folder to ensure this file is written
		if (OutputRssFile.Exists) OutputRssFile.Delete();
		_sut._Posts.Add(new Fritz.StaticBlog.PostData
		{
			Filename = "first_post.html",
			Frontmatter = new Fritz.StaticBlog.Frontmatter
			{
				Draft = false,
				PublishDate = DateTime.Today.AddDays(-1),
				Title = "First post!",
			},
			LastUpdate = DateTime.Today.AddDays(-1),
			Abstract = "This is my first post.  I write a lot of content <h2>This is the first child header</h2>"
		});

		// act
		_sut.BuildPosts();
		_sut.BuildRss();

		// assert
		var contents = File.ReadAllText(OutputRssFile.FullName);
		Assert.DoesNotContain("<h2>", contents);

	}

	[Fact]
	public void IndexShouldIncludeRssLink()
	{

		// arrange
		// Clear the folder to ensure this file is written
		if (OutputRssFile.Exists) OutputRssFile.Delete();
		_sut._Posts.Add(new Fritz.StaticBlog.PostData
		{
			Filename = "first_post.html",
			Frontmatter = new Fritz.StaticBlog.Frontmatter
			{
				Draft = false,
				PublishDate = DateTime.Today.AddDays(-1),
				Title = "First post!",
			},
			LastUpdate = DateTime.Today.AddDays(-1),
			Abstract = "This is my first post"
		});

		// act
		_sut.Validate();
		_sut.BuildPosts();
		_sut.BuildRss();
		_sut.BuildIndex();

		var indexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
		Assert.Contains($"<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{_sut.Config.Title}\" href=\"rss.xml\" />", File.ReadAllText(indexFile.FullName));

	}


}