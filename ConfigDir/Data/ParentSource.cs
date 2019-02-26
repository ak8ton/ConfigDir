using System.Collections.Generic;
using ConfigDir.Internal;
using ConfigDir.Exceptions;


namespace ConfigDir.Data
{
    public partial class Finder
    {
        private class ParentSource : ISource
        {
            private readonly Finder finder;

            public ParentSource(Finder config)
            {
                finder = config;
            }

            public IEnumerable<object> GetAllValues(string key)
            {
                foreach (var value in finder.Parent.FindAllValues(finder.Key))
                {
                    if (value.IsSource)
                    {
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
                    }
                    else
                    {
                        throw new ValueTypeException("Config option has incorrect value. Subsonfig expected", value, typeof(ISource));
                    }
                }
            }
        }
    }
}