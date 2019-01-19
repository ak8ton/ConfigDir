using System.Collections;
using System.Collections.Generic;


namespace ConfigDir.Readers
{
    class ArrayValue : IArray
    {
        private readonly List<object> list;

        public ArrayValue(object v1, object v2)
        {
            list = new List<object> { v1, v2 };
        }

        public void Add(object value)
        {
            list.Add(value);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
