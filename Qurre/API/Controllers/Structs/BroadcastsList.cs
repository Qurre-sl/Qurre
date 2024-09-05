using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Controllers.Structs
{
    public class BroadcastsList
    {
        readonly List<Broadcast> _list = new();
        public IReadOnlyList<Broadcast> List => _list.AsReadOnly();

        public IEnumerator GetEnumerator() => _list.GetEnumerator();
        public Broadcast this[int index] => _list[index];

        public void Add(Broadcast bc, bool instant = false)
        {
            if (bc == null) return;
            if (instant && _list.Count > 0)
            {
                var currentbc = _list.FirstOrDefault();
                currentbc.SmallEnd();
                _list[0] = bc;
                bc.Start();
            }
            else
            {
                _list.Add(bc);
                if (!_list[0].Active) _list[0].Start();
            }
        }

        public void Remove(Broadcast bc)
        {
            if (_list.Any(x => x == bc))
            {
                _list.Remove(bc);
                if (bc.Active) bc.End();
            }
        }

        public void Clear()
        {
            if (_list.Count < 1) return;
            try { _list[0].End(); } catch { }
            _list.Clear();
        }

        public bool Contains(Broadcast bc) => _list.Contains(bc);
        public bool Any(Func<Broadcast, bool> func) => _list.Any(func);
        public Broadcast First() => _list.First();
        public Broadcast FirstOrDefault() => _list.FirstOrDefault();
        public Broadcast FirstOrDefault(Func<Broadcast, bool> func) => _list.FirstOrDefault(func);
    }
}