namespace ConfigDir.Readers
{
    struct KeyPrior
    {
        public string Path { get; }
        public int Prior { get; }
        public string Key { get; }

        public KeyPrior(string path, int prior, string key)
        {
            Path = path;
            Prior = prior;
            Key = key;
        }
    }
}
