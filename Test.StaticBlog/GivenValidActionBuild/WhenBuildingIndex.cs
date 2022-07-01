using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	[Collection("Index construction")]
	public class WhenBuildingIndex : BaseFixture
	{

		public WhenBuildingIndex(ITestOutputHelper output)
		{ 

			// Reset by deleting the index file
			File.Delete(Path.Combine(OutputFolder.FullName, "index.html"));

			_sut._Posts.Add(new Fritz.StaticBlog.PostData {
				Filename = "first_post.html",
				Frontmatter = new Fritz.StaticBlog.Frontmatter {
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
		public void ShouldCreateIndexHtml() { 

			Assert.NotNull(_IndexFile);

		}

		[Fact]
		public void ShouldApplyMacros()
		{

			// Reset by deleting the index file
			File.Delete(Path.Combine(OutputFolder.FullName, "index.html"));

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

			_sut.Validate();
			_sut.BuildIndex();

			_IndexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
			var html = File.ReadAllText(_IndexFile.FullName);
			Assert.DoesNotContain("{{ CurrentYear }}", html);
			Assert.Contains($"<span>Year: {DateTime.Now.Year}</span>", html);

		}



	}

}