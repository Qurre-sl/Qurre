using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Qurre.API.Controllers.Structs;

[PublicAPI]
public class BroadcastsList
{
    private readonly List<Broadcast> _list = [];
    public IReadOnlyList<Broadcast> List => _list.AsReadOnly();
    public Broadcast this[int index] => _list[index];

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void Add(Broadcast bc, bool instant = false)
    {
        if (instant && _list.Count != 0)
        {
            _list[0].SilentEnd();
            _list[0] = bc;
            bc.Start();
        }
        else
        {
            _list.Add(bc);
            if (!_list[0].Active)
                _list[0].Start();
        }
    }

    public bool Remove(Broadcast bc)
    {
        if (!_list.Remove(bc))
            return false;

        if (bc.Active)
            bc.End();

        return true;
    }

    public void Clear()
    {
        if (_list.Count == 0)
            return;

        _list.First().SilentEnd();
        _list.Clear();
    }

    public bool Contains(Broadcast bc)
    {
        return _list.Contains(bc);
    }

    public bool Any()
    {
        return _list.Any();
    }

    public bool Any(Func<Broadcast, bool> func)
    {
        return _list.Any(func);
    }

    public Broadcast First()
    {
        return _list.First();
    }

    public Broadcast? FirstOrDefault()
    {
        return _list.FirstOrDefault();
    }

    public Broadcast? FirstOrDefault(Func<Broadcast, bool> func)
    {
        return _list.FirstOrDefault(func);
    }
}