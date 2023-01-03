using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Fritz.StaticBlog.adminweb.Helpers;

[HtmlTargetElement(Attributes = "is-active-page")]
public class ActiveTagHelper: AnchorTagHelper
{
  public ActiveTagHelper(IHtmlGenerator generator) : base(generator) { }
        
  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    
    if(Page == null || ViewContext.RouteData.Values["page"] == null)
      return;

		if (string.Equals(Page, ViewContext.RouteData.Values["page"].ToString(),
          StringComparison.InvariantCultureIgnoreCase) || (Page == "/Posts" && ViewContext.RouteData.Values["page"].ToString() == "/EditPost"))
    {
      var existingClasses = output.Attributes["class"].Value.ToString();
      if (output.Attributes["class"] != null)
      {
        output.Attributes.Remove(output.Attributes["class"]);
      }

      output.Attributes.Add("class", $"{existingClasses} active");
    } 
    
  }
}