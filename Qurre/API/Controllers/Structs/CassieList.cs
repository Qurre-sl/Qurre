using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Qurre.API.Controllers.Structs;

[PublicAPI]
public class CassieList
{
    private readonly List<Cassie> _list = [];
    public IReadOnlyList<Cassie> List => _list.AsReadOnly();
    public Cassie this[int index] => _list[index];
    public int Count => _list.Count;

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void Add(Cassie cassie, bool instant = false)
    {
        if (instant && _list.Count != 0)
        {
            _list.Insert(1, cassie);
            cassie.Send();
        }
        else
        {
            _list.Add(cassie);
            if (!_list[0].Active)
                _list[0].Send();
        }
    }

    public bool Remove(Cassie bc)
    {
        return _list.Remove(bc);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(Cassie bc)
    {
        return _list.Contains(bc);
    }

    public bool Any()
    {
        return _list.Any();
    }

    public bool Any(Func<Cassie, bool> func)
    {
        return _list.Any(func);
    }

    public Cassie First()
    {
        return _list.First();
    }

    public Cassie? FirstOrDefault()
    {
        return _list.FirstOrDefault();
    }

    public Cassie? FirstOrDefault(Func<Cassie, bool> func)
    {
        return _list.FirstOrDefault(func);
    }
}