using ConfigDir.Data;

namespace ConfigDir.Internal
{
    struct ValueOrSource
    {
        public bool IsSource { get; }
        public ISource Source { get; }
        public object Value { get; }

        public Finder Finder { get; }
        public KeyOrIndex KeyOrIndex { get; }
        // public string Path => finder.GetPath(key);

        private ValueOrSource(Finder finder, bool isSource, ISource source, object value, KeyOrIndex keyOrIndex)
        {
            Finder = finder;
            KeyOrIndex = keyOrIndex;

            IsSource = isSource;
            Source = source;
            Value = value;
        }

        static public ValueOrSource MkValue(Finder finder, ISource source, object value, KeyOrIndex keyOrIndex)
        {
            return new ValueOrSource(finder, false, source, value, keyOrIndex);
        }

        static public ValueOrSource MkSource(Finder finder, ISource source, KeyOrIndex keyOrIndex)
        {
            return new ValueOrSource(finder, true, source, null, keyOrIndex);
        }
    }
}
