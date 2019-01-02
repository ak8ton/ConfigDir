using System.Collections.Generic;

namespace ConfigDir.Internal
{
    static class Extensions
    {
        public static string GetPath(this IFinder finder)
        {
            return string.Join("/", GetPathItems(finder));
        }

        public static string[] GetPathItems(this IFinder finder)
        {
            var path = new List<string>();

            if(finder == null) return path.ToArray();

            do
            {
                var key = finder.Key;
                if (key != null) path.Add(key);
                finder = finder?.Parent;

            } while (finder != null);

            path.Reverse();
            return path.ToArray();
        }
    }
}
