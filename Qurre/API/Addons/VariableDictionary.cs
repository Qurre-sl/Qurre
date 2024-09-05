using System.Collections.Generic;

namespace Qurre.API.Addons
{
    public class VariableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public VariableDictionary() : base() { }

        public bool TryGetAndParse<T>(TKey key, out T value)
        {
            if (TryGetValue(key, out TValue pre))
            {
                if (pre is T res)
                {
                    value = res;
                    return true;
                }
            }

            value = default;
            return false;
        }

        new public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                return base.TryGetValue(key, out value);
            }
            catch
            {
                value = default;
                return false;
            }
        }

        new public bool ContainsKey(TKey key)
        {
            try
            {
                return base.ContainsKey(key);
            }
            catch
            {
                return false;
            }
        }

        new public bool Add(TKey key, TValue value)
        {
            try
            {
                base.Add(key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        new public bool Remove(TKey key)
        {
            try
            {
                return base.Remove(key);
            }
            catch
            {
                return false;
            }
        }

        new public TValue this[TKey key]
        {
            get
            {
                try
                {
                    return base[key];
                }
                catch
                {
                    return default;
                }
            }
            set
            {
                base[key] = value;
            }
        }
    }
}