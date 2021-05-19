using System;
using System.Collections.Generic;
using CommandLine;

namespace Fritz.StaticBlog
{
  class Program
	{

		static void Main(string[] args)
		{
			var arguments = Parser.Default.ParseArguments<ActionBuild>(args).MapResult(
				(ActionBuild options) => options.Execute(),
				errors => 1
			);

		}

	}

}
