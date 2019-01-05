using System.Collections.Generic;
using System.Linq;
using ConfigDir.Data;

namespace ConfigDir
{
    public static class Extensions
    {
        public static string GetPath(this Finder config, params string[] lastKays)
        {
            return string.Join("/", GetPathItems(config).Concat(lastKays));
        }

        static string[] GetPathItems(Finder config)
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
            return path.ToArray();
        }
    }
}
