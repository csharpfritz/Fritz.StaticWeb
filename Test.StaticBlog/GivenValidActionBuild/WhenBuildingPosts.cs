using System.IO;
using System.Linq;
using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	public class WhenBuildingPosts : BaseFixture {

		[Fact]
		public void ShouldNotThrowError() {

			_sut.BuildPosts();

		}

		[Fact()]
		public void ShouldWriteTheFirstPost() {

			// arrange
			// Clear the folder to ensure this file is written
			if (OutputPostsFolder.Exists) {
				var files = OutputPostsFolder.GetFiles();
				foreach (var f in files)
				{
						f.Delete();
				}
			}

			// act
			_sut.BuildPosts();

			// assert
			Assert.NotEmpty(OutputPostsFolder.GetFiles());

			var inspectFile = OutputPostsFolder.GetFiles("*.html").First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.DoesNotContain("draft: true", html);

		}

		[Fact()]
		public void ShouldNotIncludeFrontmatter() {

			// act
			_sut.BuildPosts();

			// assert
			var inspectFile = OutputPostsFolder.GetFiles("*.html").First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.DoesNotContain("draft: true", html);

		}

		[Fact]
		public void ShouldUseLayout() {

			// act
			_sut.BuildPosts();

			// assert
			var inspectFile = OutputPostsFolder.GetFiles("*.html").First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.Contains("<html>", html);
			Assert.Contains("<body>", html);


		}

		[Fact]
		public void ShouldInsertTitle() {

			// act
			_sut.BuildPosts();

			// assert
			var inspectFile = OutputPostsFolder.GetFiles("*.html").OrderBy(f => f.Name).First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.Contains("<title>First post!</title>", html);


		}

	}

}