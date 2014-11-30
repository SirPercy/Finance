using System.Web.Optimization;

namespace Finance.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/content/script").Include(
                        "~/scripts/jquery-2.0.2.min.js",
                        "~/scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
            "~/Content/bootstrap.min.css",
            "~/Content/Justified.css"));
        }
    }
}