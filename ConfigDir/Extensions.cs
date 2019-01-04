using System.Collections.Generic;

namespace ConfigDir
{
    public static class Extensions
    {
        public static string GetPath(this Config config)
        {
            return string.Join("/", GetPathItems(config));
        }

        static string[] GetPathItems(this Config config)
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
