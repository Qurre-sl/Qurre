using System.Collections.Generic;
using JetBrains.Annotations;

namespace Qurre.API.Addons;

[PublicAPI]
public class VariableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    public new TValue this[TKey key]
    {
        get
        {
            try
            {
                return base[key];
            }
            catch
            {
                return default!;
            }
        }
        set => base[key] = value;
    }

    public bool TryGetAndParse<T>(TKey key, out T value)
    {
        if (TryGetValue(key, out TValue pre))
            if (pre is T res)
            {
                value = res;
                return true;
            }

        value = default!;
        return false;
    }

    public new bool TryGetValue(TKey key, out TValue value)
    {
        try
        {
            return base.TryGetValue(key, out value);
        }
        catch
        {
            value = default!;
            return false;
        }
    }

    public new bool ContainsKey(TKey key)
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

    public new bool Add(TKey key, TValue value)
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

    public new bool Remove(TKey key)
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
}