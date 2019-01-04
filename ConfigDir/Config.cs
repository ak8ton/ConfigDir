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
    public abstract class Config : IConfig
    {
        public Finder Data { get; private set; }

        internal void SetFinder(Finder finder)
        {
            if (Data != null) throw new Exception("Data != null");
            finder.OnValidate += Validate;
            Data = finder;
        }

        public static string BasePath { get; set; } = "";

        public static TConfig GetOrCreate<TConfig>(string path) where TConfig : IConfig
        {
            return GetOrCreate<TConfig>(path, null);
        }

        public virtual void Validate(string key, object value) { }

        public static TConfig GetOrCreate<TConfig>(string path, Action<TConfig> init) where TConfig : IConfig
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

            var config = (TConfig)TypeBinder.CreateDynamicInstance<TConfig>();
            config.Data.Extend(new DirSource(path));
            init?.Invoke(config);
            cash[path] = config;
            return config;
        }

        // private

        private static readonly Dictionary<string, object> cash = new Dictionary<string, object>();

        private static string GetAbsolutePath(string path)
        {
            return System.IO.Path.Combine(BasePath ?? "", path ?? "");
        }
    }
}
