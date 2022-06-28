using System;
using YamlDotNet.Serialization;

namespace Fritz.StaticBlog
{

    public class Frontmatter
    {

        [YamlMember(Alias = "title")]
        public string Title { get; set; }

        [YamlMember(Alias = "draft")]
        public bool Draft { get; set; }

        [YamlMember(Alias = "publishdate")]
        public DateTime PublishDate { get; set; }

        internal string Format(string sampleText)
        {

            var outText = sampleText.Clone().ToString();
            outText = outText.Replace("{{ PublishDate }}", PublishDate.ToString());
            outText = outText.Replace("{{ Title }}", Title);

            return outText;

        }
    }

}