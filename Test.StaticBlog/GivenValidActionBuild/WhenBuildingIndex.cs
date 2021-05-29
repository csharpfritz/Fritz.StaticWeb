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
		public void ShouldCreateAnEntryForTheFirstPost() 
		{

			// Index wasnt created, don't process
			if (_IndexFile == null) return;

			var contents = File.OpenText(_IndexFile.FullName).ReadToEnd();

			Assert.Contains("First post!", contents);

		}


	}

}