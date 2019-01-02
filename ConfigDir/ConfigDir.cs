using ConfigDir.Internal;
using ConfigDir.Readers;
using System;
using System.Collections.Generic;


namespace ConfigDir
{
    static public class Config
    {
        static public string BasePath = "";

        static private readonly Dictionary<string, object> cash = new Dictionary<string, object>();

        static public TConfig GetOrCreate<TConfig>(string path) where TConfig : IConfigBase
        {
            return GetOrCreate<TConfig>(path, null);
        }

        static public TConfig GetOrCreate<TConfig>(string path, Action<TConfig> init) where TConfig : IConfigBase
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

            var finder = new Finder(); //(System.IO.Path.GetFileNameWithoutExtension(path));
            finder.Extend(new DirSource(path));
            var config = TypeBinder.CreateDynamicInstance<TConfig>(finder);
            init?.Invoke(config);
            cash[path] = config;
            return config;
        }

        static private string GetAbsolutePath(string path)
        {
            return System.IO.Path.Combine(BasePath ?? "", path ?? "");
        }
    }
}
