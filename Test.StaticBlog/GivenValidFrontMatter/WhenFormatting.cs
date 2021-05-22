using System;
using Xunit;

namespace Test.StaticBlog.GivenValidFrontMatter
{
		
	public class WhenFormatting 
	{

		[Fact]
		public void ShouldReplacePublishDate() {

			var sut = new Fritz.StaticBlog.Frontmatter() {
				Draft = false,
				PublishDate = DateTime.Today.AddHours(8).AddMinutes(28),
				Title = "First Post!"
			};

			var sampleText = "<h5>{{ PublishDate }}</h5>";

			var outText = sut.Format(sampleText);

			Assert.DoesNotContain(outText, "PublishDate");
			Assert.Contains(sut.PublishDate.ToString(), outText);

		}

	}

}