using Fritz.StaticBlog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Test.StaticBlog.GivenValidActionCreate
{

	public class WhenCreatingPost
	{

		internal ActionCreate sut;

		MockFileSystem FileSystem { get; }

		public WhenCreatingPost(ITestOutputHelper output)
		{

			var workingDirectory = Assembly.GetAssembly(GetType()).Location.Contains(@"\.vs\") ?
					@"..\..\..\..\..\..\..\..\..\TestSite" :
					"../../../../TestSite";


			string targetFolderName = GetType().Name.ToLowerInvariant();

			FileSystem = new MockFileSystem();

			sut = new ActionCreate(FileSystem)
			{
				Config = new Config
				{
					Theme = "kliptok",
					Title = "The Unit Test Website"
				},
				ContentType = "post",
				OutputPath = targetFolderName,
				ThisDirectory = workingDirectory
			};

			var destFolder = FileSystem.Path.Combine(sut.ThisDirectory, sut.OutputPath, "posts");
			FileSystem.AddDirectory(destFolder);
			if (!FileSystem.Directory.Exists(destFolder)) FileSystem.Directory.CreateDirectory(destFolder);

			Output = output;
		}

		public ITestOutputHelper Output { get; }

		[Fact]
		public void ShouldWriteNewFile()
		{

			sut.Filename = "test";
			sut.Execute();

			var outFile = FileSystem.FileInfo.New(FileSystem.Path.Combine(sut.ThisDirectory, sut.OutputPath, "posts", sut.Filename + ".md"));
			Output.WriteLine($"File should be written to: {outFile.FullName}");
			Assert.True(outFile.Exists);

		}

		[Fact]
		public void ShouldWriteBasicFrontMatter()
		{

			sut.Filename = "testFrontMatter";
			sut.Execute();

			var outFile = FileSystem.File.OpenText(Path.Combine(sut.ThisDirectory, sut.OutputPath, "posts", sut.Filename + ".md"));

			var contents = outFile.ReadToEnd();
			Assert.StartsWith("---", contents);

		}

	}

}
