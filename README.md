BundleBuilder.UrlResolver
=========================

Converts relative resource references in CSS files to absolute paths so they resolve after bundling and minification.

Example Usage
-------------

Build your bundles (as usual) but append the `.WithRelativePathResolution()` helper to your bundle. Example:

    bundles.Add(new StyleBundle("~/css/site").Include(
        "~/Content/themes/base/jquery-ui.css",
        "~/Content/bootstrap.css",
        "~/Content/bootstrap-responsive.css",
        "~/content/site.css"
    ).WithRelativePathResolution());
    
When MVC goes to bundle the library it will replace all relative resource paths with their absolute path facsimile.

Versions
--------

* v1.0.1
    * Fixed extension method to work on `Bundle` type (instead of specifically the `StyleBundle` type), however an exception will be thrown if the bundle is not a `StyleBundle`.
* v1.0.0
    * Initial Release
