using System.Collections.Generic;
using ConfigDir.Internal;

namespace ConfigDir
{
    public abstract partial class Config
    {
        private class ParentSource : ISource
        {
            private readonly Config parentConfig;
            private readonly string parentKey;
            public string Description => parentConfig.Description;

            public ParentSource(Config parent, string key)
            {
                parentConfig = parent;
                parentKey = key;
            }

            public IEnumerable<object> GetAllValues(string key)
            {
                foreach (var value in parentConfig.FindAllValues(parentKey))
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
}