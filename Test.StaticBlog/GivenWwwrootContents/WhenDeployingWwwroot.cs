using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.StaticBlog.GivenWwwrootContents
{

	public class WhenDeployingWwwroot : BaseFixture
	{

		public WhenDeployingWwwroot(ITestOutputHelper output)
		{
			Output = output;
		}

		public ITestOutputHelper Output { get; }

		[Fact]
		public void ShouldDeployContentToOutputFolderRoot()
		{

			var wwwFolder = FileSystem.Path.Combine(WorkingDirectory.FullName, "wwwroot", "css");
			FileSystem.AddFile(FileSystem.Path.Combine(wwwFolder, "site.css"), new MockFileData("body { margin: 2px; }"));

			_sut.DeployWwwRoot();

			Assert.True(FileSystem.File.Exists(FileSystem.Path.Join(OutputFolder.FullName, "css", "site.css")));

		}

		[Fact]
		public void ShouldDeployThemeWwwrootContentToOutputFolderRoot()
		{

			FileSystem.AddFile(FileSystem.Path.Combine(WorkingDirectory.FullName, "themes", "kliptok", "wwwroot", "theme.css"), new MockFileData("header { background-color: purple }"));

			_sut.DeployWwwRoot();

			Assert.True(FileSystem.File.Exists(FileSystem.Path.Join(OutputFolder.FullName, "theme.css")));

		}

	}

}
