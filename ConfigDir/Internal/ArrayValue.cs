using System.Collections;
using System.Collections.Generic;


namespace ConfigDir.Internal
{
    class ArrayValue<T> : IList<T>
    {
        List<T> cache = new List<T>();
        IEnumerator<object> source;

        private void ReadAllItems()
        {
            if (source == null) return;

            while (source.MoveNext())
            {
                var newItem = (T)source.Current;
                cache.Add(newItem);
            }

            source = null;
        }

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in cache)
            {
                yield return item;
            }

            if (source == null) yield break;

            while (source.MoveNext())
            {
                var newItem = (T)source.Current;
                cache.Add(newItem);
                yield return newItem;
            }

            source = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection<T>

        public int Count
        {
            get
            {
                ReadAllItems();
                return cache.Count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            ReadAllItems();
            cache.Add(item);
        }

        public void Clear()
        {
            source = null;
            cache.Clear();
        }

        public bool Contains(T item)
        {
            // Можно оптимизировать
            ReadAllItems();
            return cache.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Можно оптимизировать
            ReadAllItems();
            cache.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            ReadAllItems();
            return cache.Remove(item);
        }

        #endregion

        #region IList<T>

        public T this[int index]
        {
            get
            {
                ReadAllItems();
                return cache[index];
            }
            set
            {
                ReadAllItems();
                cache[index] = value;
            }
        }

        public int IndexOf(T item)
        {
            ReadAllItems();
            return cache.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ReadAllItems();
            cache.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ReadAllItems();
            cache.RemoveAt(index);
        }

        #endregion

    }
}