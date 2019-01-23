using System.Collections.Generic;
using System.Linq;
using ConfigDir.Data;

namespace ConfigDir
{
    /// <summary>
    /// Finder extensions methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get path string
        /// </summary>
        /// <param name="config"></param>
        /// <param name="lastKay"></param>
        /// <returns></returns>
        public static string GetPath(this Finder config, string lastKay)
        {
            return string.Join("/", GetPathItems(config, lastKay));
        }

        static string[] GetPathItems(Finder config, string lastKay)
        {
            var path = new List<string>();

            if(config == null) return path.ToArray();

            do
            {
                var key = config.Key;
                if (key != null) path.Add(key);
                config = config?.Parent;

            } while (config != null);

            path.Reverse();
            if (!string.IsNullOrWhiteSpace(lastKay))
            {
                path.Add(lastKay);
            }
            return path.ToArray();
        }
    }
}
