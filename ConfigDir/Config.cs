﻿using ConfigDir.Data;
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
        public Finder Finder { get; private set; }

        internal void SetFinder(Finder finder)
        {
            if (Finder != null) throw new Exception("Data != null");
            finder.OnValidate += Validate;
            Finder = finder;
        }

        public virtual void Validate(string key, object value) { }

        // static

        public static string BasePath { get; set; } = System.IO.Directory.GetCurrentDirectory();

        public static TConfig GetOrCreate<TConfig>(string path) where TConfig : class
        {
            return GetOrCreate<TConfig>(path, null);
        }

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
            var config = (TConfig)TypeBinder.CreateDynamicInstance(key, typeof(TConfig), null);
            (config as Config).Finder.Extend(new DirSource(BasePath, key));
            init?.Invoke(config);
            cache[path] = config;
            return config;
        }

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
