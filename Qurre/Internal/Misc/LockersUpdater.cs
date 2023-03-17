using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Misc
{
    internal class LockersUpdater : MonoBehaviour
    {
        readonly float _interval = 0.1f;
        float _nextCycle = 0f;

        internal MapGeneration.Distributors.Locker Locker;

        private void Start()
        {
            _nextCycle = Time.time;
        }
        internal void Update()
        {
            if (Locker is null)
                return;

            if (Time.time < _nextCycle)
                return;

            _nextCycle += _interval;

            try { Locker.netIdentity.UpdateData(); } catch { }
        }
    }
}