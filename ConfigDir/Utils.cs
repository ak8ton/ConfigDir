using ConfigDir.Readers;
using System;
using System.Collections.Generic;


namespace ConfigDir
{
    public abstract partial class Config
    {
        static public string BasePath = "";

        static private readonly Dictionary<string, object> pathDict = new Dictionary<string, object>();

        static public TConfig GetOrCreate<TConfig>(string path) where TConfig : IConfig, new()
        {
            return GetOrCreate<TConfig>(path, null);
        }

        static public TConfig GetOrCreate<TConfig>(string path, Action<TConfig> init) where TConfig : IConfig, new()
        {
            path = GetAbsolutePath(path);

            if (pathDict.ContainsKey(path))
            {
                if (pathDict[path] is TConfig c)
                {
                    return c;
                }
                else
                {
                    throw new Exception();
                }
            }

            var config = (TConfig)CreateDynamicInstance<TConfig>();
            config.Extend(new DirSource(path));
            init?.Invoke(config);
            pathDict[path] = config;
            return config;
        }

        static private string GetAbsolutePath(string path)
        {
            return System.IO.Path.Combine(BasePath ?? "", path ?? "");
        }
    }
}
