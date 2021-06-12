using System.IO;
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

					try {
						Directory.Delete(Path.Join(OutputFolder.FullName, "css"), true);
					} catch {}

					_sut.DeployWwwRoot();

					Assert.True(File.Exists(Path.Join(OutputFolder.FullName, "css", "site.css")));

        }

				[Fact]
				public void ShouldDeployThemeWwwrootContentToOutputFolderRoot() {

					try {
						File.Delete(Path.Join(OutputFolder.FullName, "theme.css"));
					}
					catch {}

					_sut.DeployWwwRoot();

					Assert.True(File.Exists(Path.Join(OutputFolder.FullName, "theme.css")));

				}

    }

}
