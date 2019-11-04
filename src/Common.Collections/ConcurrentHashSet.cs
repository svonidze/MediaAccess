namespace Common.Collections
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class ConcurrentHashSet<T> : IEnumerable<T>
    {
        private readonly ConcurrentDictionary<T, bool> dictionary;

        public ConcurrentHashSet() : this(Enumerable.Empty<T>())
        {
        }

        public ConcurrentHashSet(IEnumerable<T> enumerable)
        {
            this.dictionary = new ConcurrentDictionary<T, bool>(enumerable.ToDictionary(x => x, x => false));
        }

        public int Count => this.dictionary.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return this.dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(T item)
        {
            this.dictionary.TryAdd(item, false);
        }

        public void AddRange(IEnumerable<T> items)
        {
            items.ToList().ForEach(this.Add);
        }

        public bool Remove(T item)
        {
            return this.dictionary.TryRemove(item, out _);
        }

        public bool Contains(T item)
        {
            return this.dictionary.ContainsKey(item);
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }
    }
}