using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;

namespace Fritz.StaticBlog
{

	// From Khalid Abuhakmeh's blog at:
	// https://khalidabuhakmeh.com/parse-markdown-front-matter-with-csharp

	public static class FrontmatterExtensions {


private static readonly IDeserializer YamlDeserializer = 
        new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .Build();
    
    private static readonly MarkdownPipeline Pipeline 
        = new MarkdownPipelineBuilder()
        .UseYamlFrontMatter()
        .Build();

    public static T GetFrontMatter<T>(this string markdown)
    {
        var document = Markdown.Parse(markdown, Pipeline);
        var block = document
            .Descendants<YamlFrontMatterBlock>()
            .FirstOrDefault();

        if (block == null) 
            return default;

        var yaml =
            block
            // this is not a mistake
            // we have to call .Lines 2x
            .Lines // StringLineGroup[]
            .Lines // StringLine[]
            .OrderByDescending(x => x.Line)
            .Select(x => $"{x}\n")
            .ToList()
            .Select(x => x.Replace("---", string.Empty))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Aggregate((s, agg) => agg + s);

        return YamlDeserializer.Deserialize<T>(yaml);
    }

    public static string Serialize(this Frontmatter frontmatter)
		{

      var serializer = new SerializerBuilder()
        .IncludeNonPublicProperties()
        .Build();

      return string.Concat(
        "---", Environment.NewLine, 
        serializer.Serialize(frontmatter), 
        "---", Environment.NewLine, Environment.NewLine);

		}

	}

}