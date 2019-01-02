using System;
using System.Collections.Generic;

namespace ConfigDir.Internal
{
    class Finder : IFinder
    {
        private readonly List<ISource> deck = new List<ISource>(); 

        public IFinder Parent { get; }
        public string Key { get; }
        public string Description { get; set; } = "";

        private Finder(IFinder parent, string key)
        {
            Parent = parent;
            Key = key;
            if (parent != null)
            {
                deck.Add(new ParentSource(parent, key));
            }
        }

        public Finder() : this(null, "" ) {}

        public IFinder Query(string key)
        {
            return new Finder(this, key);
        }

        public void Update(ISource source)
        {
            deck.Insert(0, source);
        }

        public void Extend(ISource source)
        {
            deck.Add(source);
        }

        public IEnumerable<ValueOrSource> GetAllValues(string key)
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
