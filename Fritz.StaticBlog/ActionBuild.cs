using System;
using System.IO;
using System.Text.Json;
using CommandLine;

namespace Fritz.StaticBlog
{

	[Verb("build", HelpText="Build the website")]
	public class ActionBuild : ICommandLineAction
	{

		[Option('f', "force", Default = (bool)false)]
		public bool Force { get; set; }

		[Option('o', "output", Required=true, HelpText="Location to write out the rendered site")]
		public string OutputPath { get; set; }

		internal Config Config { get; private set; }

		public int Execute()
		{

			if (!Validate()) return 1;

			System.Console.WriteLine("Building...");
			return 0;

		}

		private bool Validate()
		{

			var outValue = true;
			
			var outputDir = new DirectoryInfo(OutputPath);
			outValue = outputDir.Exists;
			if (!outValue) System.Console.WriteLine($"Output folder '{outputDir.FullName}' does not exist");

			if (outValue) {
				outValue = new DirectoryInfo("themes").Exists;
				if (!outValue) System.Console.WriteLine("themes folder is missing");
			}

			if (outValue) {
				outValue = new FileInfo("config.json").Exists;
				if (!outValue) System.Console.WriteLine($"config.json file is missing");
			}

			if (outValue)	outValue = ValidateConfig();

			return outValue;

		}

		private bool ValidateConfig()
		{

			try {
				
				var rdr = File.OpenRead("config.json");
				var json = new StreamReader(rdr).ReadToEnd();
				this.Config = JsonSerializer.Deserialize<Config>(json);
			} catch (Exception ex) {
				System.Console.WriteLine($"Error while reading config: {ex.Message}");
				return false;
			}

			return true;

		}
	}

	public class Config {



	}

}
