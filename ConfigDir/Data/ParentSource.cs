﻿using System.Collections.Generic;
using ConfigDir.Internal;


namespace ConfigDir.Data
{
    public partial class Finder
    {
        private class ParentSource : ISource
        {
            private readonly Finder thisConfig;
            private readonly Finder parentConfig;
            private readonly string parentKey;
            public string Description => parentConfig.Description;

            public ParentSource(Finder config, Finder parent, string key)
            {
                thisConfig = config;
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
                                    yield return ValueOrSource.MkSource(thisConfig, src, key);
                                }
                                else
                                {
                                    yield return ValueOrSource.MkValue(thisConfig, value.Source, obj, key);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}