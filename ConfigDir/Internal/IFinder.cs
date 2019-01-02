using System.Collections.Generic;


namespace ConfigDir.Internal
{
    interface IFinder
    {
        IFinder Parent { get; }
        string Key { get; }
        IFinder Query(string key);
        void Extend(ISource source);
        void Update(ISource source);
        IEnumerable<ValueOrSource> GetAllValues(string key);
    }
}
