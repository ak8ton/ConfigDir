using ConfigDir.Data;
using ConfigDir.Readers;
using ConfigDir.Internal;
using System;
using System.Collections.Generic;


namespace ConfigDir
{
    /// <summary>
    /// Binding base class
    /// </summary>
    public abstract class Config : IConfig
    {
        /// <summary>
        /// Finder object
        /// </summary>
        public Finder Finder { get; private set; }

        internal void SetFinder(Finder finder)
        {
            if (Finder != null) throw new Exception("Data != null");
            finder.OnValidate += Validate;
            Finder = finder;
        }

        /// <summary>
        /// Custom validation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Validate(string key, object value) { }

        // static

        /// <summary>
        /// Default base path of config directory
        /// </summary>
        public static string BasePath { get; set; } = System.IO.Directory.GetCurrentDirectory();

        /// <summary>
        /// Bind directory to configuration object
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TConfig GetOrCreate<TConfig>(string path) where TConfig : class
        {
            return GetOrCreate<TConfig>(path, null);
        }

        /// <summary>
        /// Bind directory to configuration object
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="path"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public static TConfig GetOrCreate<TConfig>(string path, Action<TConfig> init) where TConfig : class
        {
            path = GetAbsolutePath(path);

            if (cache.ContainsKey(path))
            {
                if (cache[path] is TConfig c)
                {
                    return c;
                }
                else
                {
                    throw new Exception();
                }
            }

            var key = Utils.GetRelativePath(BasePath, path);
            var config = (TConfig)TypeBinder.CreateDynamicInstance(new KeyOrIndex(key), typeof(TConfig), null);
            (config as Config).Finder.Extend(new DirSource(BasePath, key));
            init?.Invoke(config);
            cache[path] = config;
            return config;
        }

        /// <summary>
        /// Clear main cache
        /// </summary>
        public static void ResetAll()
        {
            cache.Clear();
        }

        // private

        private static readonly Dictionary<string, object> cache = new Dictionary<string, object>();

        private static string GetAbsolutePath(string path)
        {
            return System.IO.Path.Combine(BasePath ?? "", path ?? "");
        }
    }
}
