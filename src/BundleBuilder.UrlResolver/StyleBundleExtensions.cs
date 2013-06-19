using System;
using System.Web.Optimization;

namespace BundleBuilder.UrlResolver
{
    /// <summary>
    /// Extends the <see cref="System.Web.Optimization.StyleBundle"/> object allowing for fluent extensions.
    /// </summary>
    public static class StyleBundleExtensions
    {
        /// <summary>
        /// Adds <see cref="UrlResolvingBundleBuilder"/> as the bundler so relative resource paths are resolved
        /// and replaced.
        /// </summary>
        /// <param name="bundle">Original bundle</param>
        /// <returns>Original bundle with the <see cref="UrlResolvingBundleBuilder"/> added.</returns>
        public static Bundle WithRelativePathResolution(this Bundle bundle)
        {
            if (!(bundle is StyleBundle))
            {
                throw new ArgumentException("Expecting StyleBundle type", "bundle");
            }
            bundle.Builder = new UrlResolvingBundleBuilder();
            return bundle;
        }
    }
}
