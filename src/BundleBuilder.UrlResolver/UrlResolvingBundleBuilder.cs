namespace BundleBuilder.UrlResolver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Optimization;

    /// <summary>
    /// Bundle builder that searched for (and replaces) relative resources paths with their absolute path counterpart.
    /// </summary>
    public class UrlResolvingBundleBuilder : IBundleBuilder
    {
        #region Properties
        
        #region Constants

        /// <summary>
        /// The single quoted path pattern
        /// </summary>
        private const String SingleQuotedPathPattern = @"'(?!http)(?:\\\\'|[^\r\n'])+'";

        /// <summary>
        /// The double quoted path pattern
        /// </summary>
        private const String DoubleQuotedPathPattern = @"""(?!http)(?:\\\\""|[^""\n\r])+""";

        /// <summary>
        /// The unquoted path pattern
        /// </summary>
        private const String UnquotedPathPattern = @"(?:\\\)|[^)])+";

        #endregion

        #region Static

        /// <summary>
        /// The URL pattern
        /// </summary>
        private static String UrlPattern = String.Format(@"url\(\s*({0}|{1}|{2})\s*\)", SingleQuotedPathPattern, DoubleQuotedPathPattern, UnquotedPathPattern);

        /// <summary>
        /// The pattern import
        /// </summary>
        private static String ImportPattern = String.Format(@"@import\s{0}", UrlPattern);

        #endregion

        /// <summary>
        /// The root path of the website
        /// </summary>
        private DirectoryInfo rootPath;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativePathResolverBundler"/> class.
        /// </summary>
        public UrlResolvingBundleBuilder()
        {
            String currentRoot = AppDomain.CurrentDomain.BaseDirectory;
            if (!currentRoot.EndsWith("\\"))
            {
                currentRoot += "\\";
            }
            this.rootPath = new DirectoryInfo(currentRoot);
        }

        #region IBundleBuilder Implementation

        /// <summary>
        /// Builds the content of the bundle.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        /// <param name="context">The context.</param>
        /// <param name="files">The files.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string BuildBundleContent(Bundle bundle, BundleContext context, IEnumerable<System.IO.FileInfo> files)
        {
            StringBuilder result = new StringBuilder();
            IBundleBuilder defaultBuilder = new DefaultBundleBuilder();
            Regex reUrl = new Regex(UrlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (FileInfo file in files)
            {
                // use normal parser to bundle it
                String contents = defaultBuilder.BuildBundleContent(bundle, context, new[] { file });

                // Now, if it's a CSS file we need to look through it
                if (String.Compare(file.Extension, ".css", true) == 0)
                {
                    contents = reUrl.Replace(contents, (match) =>
                    {
                        if (match.Success)
                        {
                            String path = match.Groups[1].Value;
                            return EscapePath(ResolvePath(rootPath, file, path));
                        }
                        return match.Value;
                    });
                }

                // Some simple cleanup then append result
                contents = contents.Trim().TrimEnd(new[] { '\r', '\n' });
                if (result.Length > 0)
                {
                    result.AppendLine();
                }
                result.Append(contents);
            }

            return result.ToString();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Escapes the path (taking care of special characters) and returns the result surrounded by the url(...) wrapper.
        /// </summary>
        /// <param name="originalPath"></param>
        /// <returns></returns>
        private static String EscapePath(String originalPath)
        {
            String result = String.Empty;
            if (!String.IsNullOrWhiteSpace(originalPath))
            {
                originalPath = originalPath.Trim();
                result = String.Format("url(\"{0}\")", originalPath.Replace("\"", "\\\""));
            }
            return result;
        }

        /// <summary>
        /// Determines wheter the path is already using an absolute reference.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns></returns>
        private static Boolean IsAbsolutePath(String path)
        {
            return String.Compare(path ?? String.Empty, 0, "/", 0, 1) == 0;
        }

        /// <summary>
        /// Determines whether the path points to an external website.
        /// </summary>
        /// <param name="path">The path.</param>
        private static Boolean IsExternalPath(String path)
        {
            return String.Compare(path ?? String.Empty, 0, "http", 0, 4) == 0;
        }

        private static String ResolvePath(DirectoryInfo basePath, FileInfo fileOrigin, String path)
        {
            String result = String.Empty;

            path = UnescapePath(path);
            if (IsExternalPath(path))
            {
                // This is an external reference, we need not worry about correcting it.
                result = path;
            }
            else
            {
                // Check for query parameters (?foo=bar&baz=foo)
                String queryParameters = String.Empty;
                Int32 questionMarkLocation = path.IndexOf('?');
                Int32 hashLocation = path.IndexOf('#');
                if (questionMarkLocation != -1 || hashLocation != -1)
                {
                    Int32 location = questionMarkLocation != -1
                        ? (hashLocation != -1 && hashLocation < questionMarkLocation ? hashLocation : questionMarkLocation)
                        : hashLocation;
                    queryParameters = path.Substring(location);
                    path = path.Substring(0, location);
                }

                if (IsAbsolutePath(path))
                {
                    // path is absolute (references directly from site root)
                    result = Path.GetFullPath(Path.Combine(basePath.FullName, path.Substring(1)));
                }
                else
                {
                    // path is relative, let's get an asbolute path from it
                    try { result = Path.GetFullPath(Path.Combine(fileOrigin.Directory.FullName, path)); }
                    catch { result = path; }
                }

                // clean up and re-add query parameters
                result = result.Substring(basePath.FullName.Length - 1).Replace('\\', '/');
                result += queryParameters;
            }

            return result;
        }

        /// <summary>
        /// Removes any surrounding quotes or escaped characters from the path
        /// </summary>
        /// <param name="originalPath"></param>
        /// <returns></returns>
        private static String UnescapePath(String originalPath)
        {
            String result = String.Empty;
            if (!String.IsNullOrWhiteSpace(originalPath))
            {
                originalPath = originalPath.Trim();
                if ((originalPath.StartsWith("'") && originalPath.EndsWith("'")) || (originalPath.StartsWith("\"") && originalPath.EndsWith("\"")))
                {
                    result = originalPath.Substring(1, originalPath.Length - 2);
                }
                else
                {
                    result = originalPath;
                }
            }
            return result;
        }

        #endregion
    }
}
