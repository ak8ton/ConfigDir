using ConfigDir.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Type of value by key
        /// </summary>
        /// <param name="finder"></param>
        /// <param name="key">Key</param>
        /// <returns>Type</returns>
        public static Type GetValueType(this Finder finder, string key)
        {
            return finder?.ConfigType?.GetProperty(key)?.PropertyType;
        }

        /// <summary>
        /// Summary of value by key
        /// </summary>
        /// <param name="finder"></param>
        /// <param name="key">Key</param>
        /// <returns>Summary</returns>
        public static string GetSummary(this Finder finder, string key)
        {
            SummaryAttribute attribute = (SummaryAttribute)finder
                ?.ConfigType
                ?.GetProperty(key)
                ?.GetCustomAttributes(typeof(SummaryAttribute), false)
                ?.FirstOrDefault();

            return attribute?.Summary;
        }

        internal static IEnumerable<string> GetAllSummaries(this Finder finder, string key)
        {
            while (finder != null)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    break;
                }

                var summary = GetSummary(finder, key);
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    yield return $"[{key}] {summary}";
                }

                key = finder.Key;
                finder = finder.Parent;
            }
        }
    }
}
