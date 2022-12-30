using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Controllers.Structs
{
    public class CassieList
    {
        readonly List<Cassie> _list = new();
        public IReadOnlyList<Cassie> List => _list.AsReadOnly();

        public IEnumerator GetEnumerator() => _list.GetEnumerator();
        public Cassie this[int index] => _list[index];
        public int Count => _list.Count;

        public void Add(Cassie cassie, bool instant = false)
        {
            if (cassie == null) return;
            if (instant && _list.Count > 0)
            {
                _list.Insert(1, cassie);
                cassie.Send();
            }
            else
            {
                _list.Add(cassie);
                if (!_list[0].Active) _list[0].Send();
            }
        }
        public void Remove(Cassie bc)
        {
            if (_list.Any(x => x == bc))
                _list.Remove(bc);
        }
        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(Cassie bc) => _list.Contains(bc);
        public bool Any(Func<Cassie, bool> func) => _list.Any(func);
    }
}