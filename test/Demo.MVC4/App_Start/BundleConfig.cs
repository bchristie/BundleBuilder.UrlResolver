namespace Demo.MVC4
{
    using System.Web.Optimization;
    using BundleBuilder.UrlResolver;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            //
            // jQuery & jQuery-UI
            //
            bundles.Add(new StyleBundle("~/css/jquery").Include(
                "~/Content/themes/base/jquery-ui.css"
            ).WithRelativePathResolution());
            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js"
            ));

            //
            // Twitter Bootstrap
            //
            bundles.Add(new StyleBundle("~/css/bootstrap").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css"
            ).WithRelativePathResolution());
            bundles.Add(new ScriptBundle("~/js/bootstrap").Include(
                "~/Scripts/bootstrap.js"
            ));

            //
            // Font Awesome
            //
            bundles.Add(new StyleBundle("~/css/font-awesome").Include(
                "~/Content/font-awesome.css"
            ).WithRelativePathResolution());
            bundles.Add(new StyleBundle("~/css/font-awesome/ie7").Include(
                "~/Content/font-awesome-ie7.css"
            ).WithRelativePathResolution());

            //
            // HTML5 shiv
            //
            bundles.Add(new ScriptBundle("~/js/html5shiv").Include(
                "~/Scripts/html5shiv.js"
            ));
        }
    }
}