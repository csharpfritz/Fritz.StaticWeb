using Fritz.StaticBlog;
using System;
using System.Collections.Generic;
using System.IO;
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

		public WhenCreatingPost(ITestOutputHelper output)
		{

			var workingDirectory = Assembly.GetAssembly(GetType()).Location.Contains(@"\.vs\") ?
					@"..\..\..\..\..\..\..\..\..\TestSite" :
					"../../../../TestSite";


			string targetFolderName = GetType().Name.ToLowerInvariant();

			sut = new ActionCreate
			{
				Config = new Config
				{
					Theme = "kliptok",
					Title = "The Unit Test Website"
				},
				ContentType = "post",
				OutputPath = targetFolderName,
				WorkingDirectory = workingDirectory
			};

			var destFolder = Path.Combine(sut.WorkingDirectory, sut.OutputPath, "posts");
			if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

			Output = output;
		}

		public ITestOutputHelper Output { get; }

		[Fact]
		public void ShouldWriteNewFile()
		{

			sut.Filename = "test";
			sut.Execute();

			var outFile = new FileInfo(Path.Combine(sut.WorkingDirectory, sut.OutputPath, "posts", sut.Filename + ".md"));
			Output.WriteLine($"File should be written to: {outFile.FullName}");
			Assert.True(outFile.Exists);

		}

		[Fact]
		public void ShouldWriteBasicFrontMatter()
		{

			sut.Filename = "testFrontMatter";
			sut.Execute();

			var outFile = File.OpenText(Path.Combine(sut.WorkingDirectory, sut.OutputPath, "posts", sut.Filename + ".md"));

			var contents = outFile.ReadToEnd();
			Assert.StartsWith("---", contents);

		}

	}

}
