using System.IO;
using System.Linq;
using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	public class WhenCreatingIndex : BaseFixture
	{

		public WhenCreatingIndex()
		{
			_sut.BuildIndex();
		}

		[Fact]
		public void ShouldCreateIndexHtml() {

			var indexFile = OutputFolder.GetFiles("index.html").FirstOrDefault();
			Assert.NotNull(indexFile);

		}

		[Fact]
		public void ShouldCreateAnEntryForTheFirstPost() 
		{

			

		}


	}

}