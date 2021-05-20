using System.IO;
using System.Linq;
using Xunit;

namespace Test.StaticBlog.GivenValidActionBuild
{
		
	public class WhenBuildingPosts : BaseFixture {

		public WhenBuildingPosts() : base()
		{

			OutputFolder = new DirectoryInfo(Path.Combine(_sut.WorkingDirectory, "dist", "posts"));

		}

		public DirectoryInfo OutputFolder { get; private set; }

		[Fact]
		public void ShouldNotThrowError() {

			_sut.BuildPosts();

		}

		[Fact()]
		public void ShouldWriteTheFirstPost() {

			// arrange
			// Clear the folder to ensure this file is written
			if (OutputFolder.Exists) {
				var files = OutputFolder.GetFiles();
				foreach (var f in files)
				{
						f.Delete();
				}
			}

			// act
			_sut.BuildPosts();

			// assert
			Assert.NotEmpty(OutputFolder.GetFiles());

			var inspectFile = OutputFolder.GetFiles("*.html").First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.DoesNotContain("draft: true", html);

		}

		[Fact()]
		public void ShouldNotIncludeFrontmatter() {

			// act
			_sut.BuildPosts();

			// assert
			var inspectFile = OutputFolder.GetFiles("*.html").First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.DoesNotContain("draft: true", html);

		}

		[Fact]
		public void ShouldUseLayout() {

			// act
			_sut.BuildPosts();

			// assert
			var inspectFile = OutputFolder.GetFiles("*.html").First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.Contains("<html>", html);
			Assert.Contains("<body>", html);


		}

		[Fact]
		public void ShouldInsertTitle() {

			// act
			_sut.BuildPosts();

			// assert
			var inspectFile = OutputFolder.GetFiles("*.html").OrderBy(f => f.Name).First();
			var html = File.ReadAllText(inspectFile.FullName);
			Assert.Contains("<title>First post!</title>", html);


		}

	}

}