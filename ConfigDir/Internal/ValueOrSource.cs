using ConfigDir.Data;

namespace ConfigDir.Internal
{
    struct ValueOrSource
    {
        public bool IsSource { get; }
        public ISource Source { get; }
        public object Value { get; }

        public Finder Finder { get; }
        public string Key { get; }
        // public string Path => finder.GetPath(key);

        private ValueOrSource(Finder finder, bool isSource, ISource source, object value, string key)
        {
            Finder = finder;
            Key = key;

            IsSource = isSource;
            Source = source;
            Value = value;
        }

        static public ValueOrSource MkValue(Finder finder, ISource source, object value, string key)
        {
            return new ValueOrSource(finder, false, source, value, key);
        }

        static public ValueOrSource MkSource(Finder finder, ISource source, string key)
        {
            return new ValueOrSource(finder, true, source, null, key);
        }
    }
}
