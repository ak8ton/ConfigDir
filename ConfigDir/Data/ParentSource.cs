using System.Collections.Generic;
using ConfigDir.Internal;


namespace ConfigDir.Data
{
    public partial class Finder
    {
        private class ParentSource : ISource
        {
            private readonly Finder finder;
            public string Description => finder.Parent.Description;

            public ParentSource(Finder config)
            {
                finder = config;
            }

            public IEnumerable<object> GetAllValues(string key)
            {
                foreach (var value in finder.Parent.FindAllValues(finder.Key))
                {
                    switch (value.Type)
                    {
                        case ValueOrSourceType.stop:
                            yield return value;
                            break;

                        case ValueOrSourceType.value:
                            yield return ValueOrSource.MkStop_TypeError(value);
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
                                    yield return ValueOrSource.MkSource(finder, src, key);
                                }
                                else
                                {
                                    yield return ValueOrSource.MkValue(finder, value.Source, obj, key);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}