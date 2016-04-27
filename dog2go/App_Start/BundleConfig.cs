using System.Web.Optimization;

namespace dog2go
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/require").Include(
                        "~/Frontend/Library/requirejs/require.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Frontend/Library/JQuery/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Frontend/Library/JQuery/jquery-ui-{version}-dnd-only.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Frontend/Library/JQuery/jquery.validate.min.js",
                        "~/Frontend/Library/JQuery/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Frontend/Library/SignalR/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Frontend/Library/Bootstrap/bootstrap.js",
                      "~/Frontend/Library/Bootstrap/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Frontend/Styles/site.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Frontend/Styles/bootstrap/bootstrap.css"));

        }
    }
}
