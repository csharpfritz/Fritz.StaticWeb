using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	public class WhenBuildingIndex : BaseFixture
	{

		public WhenBuildingIndex()
		{

			// Reset by deleting the index file
			File.Delete(Path.Combine(OutputFolder.FullName, "index.html"));

			_sut._Posts.Add(new Fritz.StaticBlog.PostData {
				Filename = "first_post.html",
				Frontmatter = new Fritz.StaticBlog.Frontmatter {
					Draft = false,
					PublishDate = DateTime.Today.AddDays(-1),
					Title = "First post!"
				},
				Abstract = "This is my first post"
			});

			_sut.BuildIndex();

			System.Console.WriteLine($"Index file location: {Path.Combine(OutputFolder.FullName, "index.html")}");
			_IndexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();

		}

		private FileInfo _IndexFile;

		[Fact]
		public void ShouldCreateIndexHtml() {

			Assert.NotNull(_IndexFile);

		}

		[Fact]
		public void ShouldSetTitleFromConfigFile() {

			if (_IndexFile == null) return;

			var contents = File.OpenText(_IndexFile.FullName).ReadToEnd();			

			Assert.Contains("<title>The Unit Test Website</title>", contents);

		}

		[Fact]
		public void ShouldUseTheLayout() 
		{

			if (_IndexFile == null) return;

			var contents = File.OpenText(_IndexFile.FullName).ReadToEnd();

			Assert.Contains("<!-- Test Layout -->", contents);

		}

		[Fact]
		public void ShouldCreateAnEntryForTheFirstPost() 
		{

			// Index wasnt created, don't process
			if (_IndexFile == null) return;

			var contents = File.OpenText(_IndexFile.FullName).ReadToEnd();

			// Check for the post title
			Assert.Contains("First post!", contents);

			// Check for the content
			Assert.Contains("This is my first post", contents);

		}


	}

}