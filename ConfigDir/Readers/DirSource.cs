using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConfigDir.Readers
{
    class DirSource : ISource
    {
        public string Description { get; }
        public string BasePath { get; }
        public string DirName { get; }

        private Dictionary<string, List<ISource>> s = null;
        private Dictionary<string, List<ISource>> SourcesByKey => s ?? (s = ReadDir());
        private readonly Regex pattern = new Regex(@"^(.*-)?(?<KEY>@?_?\p{L}\w{0,20})(?<PRIOR>-\d{1,4})?$", RegexOptions.IgnoreCase);

        private string dirPath;

        public DirSource(string basePath, string dirName)
        {
            basePath = Path.GetFullPath(basePath);
            BasePath = basePath;
            DirName = dirName;
            dirPath = Path.GetFullPath(Path.Combine(basePath, dirName));
            Description = "Папка: " + dirName;
        }

        public IEnumerable<object> GetAllValues(string key)
        {
            if (SourcesByKey.ContainsKey(key))
            {
                foreach (var value in SourcesByKey[key])
                {
                    yield return value;
                }
            }
        }

        public override string ToString()
        {
            return Description;
        }

        private Dictionary<string, List<ISource>> ReadDir()
        {
            var dict = new Dictionary<string, List<ISource>>();

            var groups = Directory.EnumerateFileSystemEntries(dirPath)
                .Select(f => GetKeyPrior(f))
                .GroupBy(kp => kp.Key);

            foreach (var group in groups)
            {
                dict[group.Key] = group
                    .OrderBy(f => f.Prior)
                    .Select(f => GetSource(f.Path))
                    .ToList();
            }

            dirPath = null;
            return dict;
        }

        private KeyPrior GetKeyPrior(string filename)
        {
            int prior = -1;
            var name = Path.GetFileNameWithoutExtension(filename);
            var m = pattern.Match(name);
            if (m.Success)
            {
                if (m.Groups["PRIOR"].Success)
                {
                    prior = int.Parse(m.Groups["PRIOR"].Value);
                }

                if (m.Groups["KEY"].Success)
                {
                    return new KeyPrior(filename, prior, m.Groups["KEY"].Value);
                }
            }

            throw new Exception("Bad filename " + name);
        }

        private ISource GetSource(string path)
        {
            if (Directory.Exists(path))
            {
                return new DirSource(BasePath, Path.GetRelativePath(BasePath, path));
            }

            if (File.Exists(path))
            {
                return new XSource(BasePath, Path.GetRelativePath(BasePath, path));
            }

            throw new Exception("Path not exists: " + path);
        }
    }
}
