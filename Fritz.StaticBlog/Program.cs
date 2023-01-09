global using Fritz.StaticBlog;
global using Fritz.StaticBlog.Data;

using CommandLine;

if (args.Length == 0)
{

  await LocalWeb.StartAdminWeb();

}
else
{

  var arguments = Parser.Default.ParseArguments<ActionBuild, ActionCreate, ActionRun>(args).MapResult(
    (ActionBuild options) => options.Execute(),
    (ActionCreate options) => options.Execute(),
    (ActionRun options) => options.Execute(), 
    errors => 1
  );

}
