using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;


namespace UnitTests
{
    static class Utils
    {
        public static void ListDir(string path, string indent = "|- ")
        {
            foreach (var d in Directory.EnumerateDirectories(path))
            {
                Console.WriteLine(indent + Path.GetFileName(d));
                ListDir(d, indent.Replace('-', ' ') + "|- ");
            }

            foreach (var f in Directory.EnumerateFiles(path))
            {
                Console.WriteLine(indent + Path.GetFileName(f));
            }
        }

        public static void PrintConfig(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .OrderBy(f => GetPrior(f));

            foreach (var file in files)
            {
                Console.WriteLine();
                Console.WriteLine(" # ");
                Console.WriteLine(" # " + file);
                Console.WriteLine(" # ");
                Console.WriteLine();
                Console.WriteLine(rootElement.Replace(File.ReadAllText(file), ""));
            }
        }
        static Regex rootElement = new Regex(@"(^<.+\n)|(<.+$)");

        private static int GetPrior(string path)
        {
            var m = prior.Match(path);
            if (m.Success)
            {
                return int.Parse(m.Value);
            }
            return 0;
        }
        static Regex prior = new Regex(@"\d+");
    }
}
