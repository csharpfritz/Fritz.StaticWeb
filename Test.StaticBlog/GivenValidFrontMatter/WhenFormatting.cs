using Fritz.StaticBlog;
using System;
using Xunit;

namespace Test.StaticBlog.GivenValidFrontMatter
{
		
	public class WhenFormatting 
	{

		[Fact]
		public void ShouldReplacePublishDate()
		{

			var sut = new Fritz.StaticBlog.Frontmatter()
			{
				Draft = false,
				PublishDate = DateTime.Today.AddHours(8).AddMinutes(28),
				Title = "First Post!"
			};

			var sampleText = "<h5>{{ PublishDate }}</h5>";

			var outText = sut.Format(sampleText);

			Assert.DoesNotContain(outText, "PublishDate");
			Assert.Contains(sut.PublishDate.ToString(), outText);

		}

		[Fact]
		public void ShouldAddImageAndDescriptionInHeader()
		{

			var fm = new Fritz.StaticBlog.Frontmatter()
			{
				Draft = false,
				PublishDate = DateTime.Today.AddHours(8).AddMinutes(28),
				Title = "First Post!",
				Description = "This is my first post!",
				Preview = "/foo.png"
			};

			var sampleLayout = "<html><head><title>This is my blog</title></head><body>{{ body }}</body></html>";
			var sut = new ActionBuild
			{
				Force = false,
				OutputPath = "foo",
				ThisDirectory = "someOtherFoo",
				Config = new Config
				{
					Theme = "kliptok",
					Title = "The Unit Test Website"
				}
			};

			var outText = sut.InsertHeadContent(fm, sampleLayout);

			Assert.Contains(fm.Preview, outText);

		}

	}

}