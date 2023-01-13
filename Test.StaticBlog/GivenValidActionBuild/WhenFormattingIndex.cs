using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Test.StaticBlog.GivenValidActionBuild
{


	public class WhenFormattingIndex : IClassFixture<IndexFixture>
	{

		public WhenFormattingIndex(ITestOutputHelper output, IndexFixture fixture)
		{

			Output = output;

			this.Fixture = fixture;
			fixture._sut.Logger = new XUnitLogger(output);

		}

		public ITestOutputHelper Output { get; }
		public IndexFixture Fixture { get; }

		[Fact]
		public void ShouldSetTitleFromConfigFile()
		{

			if (Fixture._IndexFile == null) return;

			var contents = Fixture.FileSystem.File.OpenText(Fixture._IndexFile.FullName).ReadToEnd();

			Assert.Contains("<title>The Unit Test Website</title>", contents);

		}

		[Fact]
		public void ShouldUseTheLayout()
		{

			if (Fixture._IndexFile == null) return;

			var contents = Fixture.FileSystem.File.OpenText(Fixture._IndexFile.FullName).ReadToEnd();

			Assert.Contains("<!-- Test Layout -->", contents);

		}

		[Fact]
		public void ShouldCreateAnEntryForTheFirstPost()
		{

			// Index wasnt created, don't process
			if (Fixture._IndexFile == null) return;

			var contents = Fixture.FileSystem.File.OpenText(Fixture._IndexFile.FullName).ReadToEnd();

			// Check for the post title
			Assert.Contains("First post!", contents);

			// Should contains a link to the first post
			Assert.Contains($"<a href=\"{Fixture._sut._Posts.First().Filename}\">", contents);

			// Check for the content
			Assert.Contains("This is my first post", contents);

		}

		[Fact]
		public void ShouldEvaluateIncludes()
		{

			// Index wasnt created, don't process
			if (Fixture._IndexFile == null) return;

			var contents = Fixture.FileSystem.File.OpenText(Fixture._IndexFile.FullName).ReadToEnd();

			// Check for included material 
			Assert.Contains("This is an include", contents);

		}

		[Fact]
		public void ShouldEvaluateMacrosInsideIncludes()
		{

			// Index wasnt created, don't process
			if (Fixture._IndexFile == null) return;

			var contents = Fixture.FileSystem.File.OpenText(Fixture._IndexFile.FullName).ReadToEnd();

			// Check for included material   
			Assert.Contains("This is the current year: " + DateTime.UtcNow.Year, contents);

		}

	}
}
