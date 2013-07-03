BundleBuilder.UrlResolver
=========================

No Longer Necessary! (Here's Why)
---------------------------------

It appears that this issue has been [resolved](http://aspnetoptimization.codeplex.com/workitem/30) natively. You should now use the `CssRewriteUrlTransform` transformer when you build the bundle. e.g.

    bundles.Add(new ScriptBundle("~/css/site")
        .Include("~/Content/themes/base/jquery-ui.css", new CssRewriteUrlTransform())
        .Include("~/Content/bootstrap.css", new CssRewriteUrlTransform())
        .Include("~/Content/bootstrap-responsive.css", new CssRewriteUrlTransform())
        .Include("~/Content/site.css", new CssRewriteUrlTransform())
    );
    
Personally I think they should have made this a built-in default, but to each his/her own. However, you _can_ make the task easier using the following extension method:

    /// <summary>
    /// Includes the specified <paramref name="virtualPaths"/> within the bundle and attached the
    /// <see cref="System.Web.Optimization.CssRewriteUrlTransform"/> item transformer to each item
    /// automatically.
    /// </summary>
    /// <param name="bundle">The bundle.</param>
    /// <param name="virtualPaths">The virtual paths.</param>
    /// <returns>Bundle.</returns>
    /// <exception cref="System.ArgumentException">Only available to StyleBundle;bundle</exception>
    /// <exception cref="System.ArgumentNullException">virtualPaths;Cannot be null or empty</exception>
    public static Bundle IncludeWithCssRewriteTransform(this Bundle bundle, params String[] virtualPaths)
    {
        if (!(bundle is StyleBundle))
        {
            throw new ArgumentException("Only available to StyleBundle", "bundle");
        }
        if (virtualPaths == null || virtualPaths.Length == 0)
        {
            throw new ArgumentNullException("virtualPaths", "Cannot be null or empty");
        }
        IItemTransform itemTransform = new CssRewriteUrlTransform();
        foreach (String virtualPath in virtualPaths)
        {
            if (!String.IsNullOrWhiteSpace(virtualPath))
            {
                bundle.Include(virtualPath, itemTransform);
            }
        }
        return bundle;
    }
    
Which in turn makes the original code a little more concise:

    bundles.Add(new ScritpBundle("~/css/site").IncludeWithCssRewriteTransform(
        "~/Content/themes/base/jquery-ui.css",
        "~/Content/bootstrap.css",
        "~/Content/bootstrap-responsive.css",
        "~/Content/site.css"
    ));


Summary
-------

Converts relative resource references in CSS files to absolute paths so they resolve after bundling and minification.

Example Usage
-------------

Build your bundles (as usual) but append the `.WithRelativePathResolution()` helper to your bundle. Example:

    using BundleBuilder.UrlResolver;

    bundles.Add(new StyleBundle("~/css/site").Include(
        "~/Content/themes/base/jquery-ui.css",
        "~/Content/bootstrap.css",
        "~/Content/bootstrap-responsive.css",
        "~/Content/site.css"
    ).WithRelativePathResolution());
    
When MVC goes to bundle the library it will replace all relative resource paths with their absolute path facsimile.

Versions
--------

* v1.0.1
    * Fixed extension method to work on `Bundle` type (instead of specifically the `StyleBundle` type), however an exception will be thrown if the bundle is not a `StyleBundle`.
* v1.0.0
    * Initial Release

Support
-------

Want to help? You can promote the following items for me. ;-)

* [Enable global ability to change the default BundleBuilder](https://aspnetoptimization.codeplex.com/workitem/53)
* [Add support for versioning files in debug mode automatically](https://aspnetoptimization.codeplex.com/workitem/40)
