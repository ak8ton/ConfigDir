namespace ConfigDir.Internal
{
    static class Utils
    {
        public static string GetRelativePath(string basePath, string path)
        {
            basePath = System.IO.Path.GetFullPath(basePath);
            path = System.IO.Path.GetFullPath(path);

            if (path.StartsWith(basePath))
            {
                return path.Substring(basePath.Length).Trim(System.IO.Path.DirectorySeparatorChar);
            }

            return path;
        }
    }
}
