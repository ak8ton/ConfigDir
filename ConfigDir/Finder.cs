using System.Collections.Generic;
using ConfigDir.Internal;


namespace ConfigDir
{
    public abstract partial class Config 
    {
        private readonly List<ISource> deck = new List<ISource>();

        public Config Parent { get; private set; }
        public string Key { get; private set; }

        private void SetParent(Config parent, string key)
        {
            Parent = Parent;
            Key = key;
            deck.Add(new ParentSource(parent, key));
        }

        public Config Update(ISource source)
        {
            deck.Insert(0, source);
            return this;
        }

        public Config Extend(ISource source)
        {
            deck.Add(source);
            return this;
        }

        private IEnumerable<ValueOrSource> FindAllValues(string key)
        {
            foreach (var source in deck)
            {
                foreach (var value in source.GetAllValues(key))
                {
                    if (value is ValueOrSource vos)
                    {
                        yield return vos;
                    }
                    else if (value is ISource src)
                    {
                        yield return ValueOrSource.MkSource(src);
                    }
                    else
                    {
                        yield return ValueOrSource.MkValue(source, value);
                    }
                }
            }
        }
    }
}
