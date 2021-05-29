using System.IO;
using System.Linq;
using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	public class WhenBuildingIndex : BaseFixture
	{

		public WhenBuildingIndex()
		{
			_sut.BuildIndex();
		}

		[Fact]
		public void ShouldCreateIndexHtml() {

			System.Console.WriteLine($"Index file location: {Path.Combine(OutputFolder.FullName, "index.html")}");

			var indexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
			Assert.NotNull(indexFile);

		}

		[Fact]
		public void ShouldCreateAnEntryForTheFirstPost() 
		{

			

		}


	}

}