using System.Collections.Generic;

namespace ConfigDir.Internal
{
    class ParentSource : ISource
    {
        private readonly IFinder finder;
        private readonly string parentKey;
        public string Description { get; }

        public ParentSource(IFinder parent, string key)
        {
            Description = "Parent finder: " + key;
            finder = parent;
            parentKey = key;
        }


        public IEnumerable<object> GetAllValues(string key)
        {
            foreach (var value in finder.GetAllValues(parentKey))
            {
                switch (value.Type)
                {
                    case ValueOrSourceType.stop:
                        yield return value;
                        break;

                    case ValueOrSourceType.value:
                        yield return ValueOrSource.MkStop(value.Source, value.Value);
                        break;

                    case ValueOrSourceType.source:
                        foreach (var obj in value.Source.GetAllValues(key))
                        {
                            if (obj is ValueOrSource vos)
                            {
                                yield return vos;
                            }
                            else if (obj is ISource src)
                            {
                                yield return ValueOrSource.MkSource(src);
                            }
                            else
                            {
                                yield return ValueOrSource.MkValue(value.Source, obj);
                            }
                        }
                        break;
                }
            }
        }
    }
}
