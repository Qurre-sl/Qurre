using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Misc
{
    internal class DoorsUpdater : MonoBehaviour
    {
        readonly float _interval = 0.1f;
        float _nextCycle = 0f;

        internal DoorVariant Door;

        Vector3 _cachedPosition = Vector3.zero;
        Vector3 _cachedScale = Vector3.zero;
        Quaternion _cachedRotation = Quaternion.identity;

        private void Start()
        {
            _nextCycle = Time.time;
        }
        internal void Update()
        {
            if (Door is null)
                return;

            if (Time.time < _nextCycle)
                return;

            _nextCycle += _interval;

            Transform transform = Door.netIdentity.gameObject.transform;
            if (_cachedPosition == transform.position &&
                _cachedRotation == transform.rotation &&
                _cachedScale == transform.lossyScale)
                return;

            _cachedPosition = transform.position;
            _cachedRotation = transform.rotation;
            _cachedScale = transform.lossyScale;
            try { Door.netIdentity.UpdateData(); } catch { }
        }
    }
}