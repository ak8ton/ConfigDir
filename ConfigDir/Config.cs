using ConfigDir.Data;
using ConfigDir.Readers;
using ConfigDir.Internal;
using System;
using System.Collections.Generic;


namespace ConfigDir
{
    /// <summary>
    /// Базовый класс привязки
    /// </summary>
    public abstract class Config
    {
        public Finder Data { get; private set; }

        internal void SetFinder(Finder finder)
        {
            if (Data != null) throw new Exception("Data != null");
            finder.OnValidate += Validate;
            Data = finder;
        }

        public static string BasePath { get; set; } = System.IO.Directory.GetCurrentDirectory();

        public static TConfig GetOrCreate<TConfig>(string path) where TConfig : class
        {
            return GetOrCreate<TConfig>(path, null);
        }

        public virtual void Validate(string key, object value) { }

        public static TConfig GetOrCreate<TConfig>(string path, Action<TConfig> init) where TConfig : class
        {
            path = GetAbsolutePath(path);

            if (cash.ContainsKey(path))
            {
                if (cash[path] is TConfig c)
                {
                    return c;
                }
                else
                {
                    throw new Exception();
                }
            }

            var key = Utils.GetRelativePath(BasePath, path);
            var config = (TConfig)TypeBinder.CreateDynamicInstance<TConfig>(key);
            (config as Config).Data.Extend(new DirSource(BasePath, key));
            init?.Invoke(config);
            cash[path] = config;
            return config;
        }

        public static void ResetAll()
        {
            cash.Clear();
        }

        // private

        private static readonly Dictionary<string, object> cash = new Dictionary<string, object>();

        private static string GetAbsolutePath(string path)
        {
            return System.IO.Path.Combine(BasePath ?? "", path ?? "");
        }
    }
}
