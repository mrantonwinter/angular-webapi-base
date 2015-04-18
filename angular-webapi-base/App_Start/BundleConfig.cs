using System.Web;
using System.Web.Optimization;

namespace angular_webapi_base
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/app").IncludeDirectory("~/Angular/JS", "*.js", true));

            bundles.Add(new StyleBundle("~/css").IncludeDirectory("~/Content", "*.css"));


            // Set EnableOptimizations to false for debugging. For more information,
            BundleTable.EnableOptimizations = false;
        }
    }
}
