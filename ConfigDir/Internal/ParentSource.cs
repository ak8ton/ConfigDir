using System.Collections.Generic;
using ConfigDir.Data;
using ConfigDir.Exceptions;


namespace ConfigDir.Internal
{
    class ParentSource : IConfigSource, IArraySource
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
                    foreach (var obj in ((IConfigSource)value.Source).GetAllValues(key))
                    {
                        if (obj is ValueOrSource vos)
                        {
                            yield return vos;
                        }
                        else if (obj is IConfigSource src)
                        {
                            yield return ValueOrSource.MkSource(finder, src, new KeyOrIndex(key));
                        }
                        else
                        {
                            yield return ValueOrSource.MkValue(finder, value.Source, obj, new KeyOrIndex(key));
                        }
                    }
                }
                else
                {
                    throw new ValueTypeException("Config option has incorrect value. Subsonfig expected", value, typeof(IConfigSource));
                }
            }
        }

        public IEnumerable<string> GetKeys()
        {
            throw new System.NotImplementedException("ConfigDir.Internal.ParentSource.GetKeys()");
        }

        public IEnumerable<object> GetAllValues(int index)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }
    }
}