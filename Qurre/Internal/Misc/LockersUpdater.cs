using System.Collections.Generic;
using MapGeneration.Distributors;
using MEC;
using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Misc;

internal class LockersUpdater : MonoBehaviour
{
    private const float Interval = 0.1f;

    private Vector3 _cachedPosition = Vector3.zero;
    private Quaternion _cachedRotation = Quaternion.identity;
    private Vector3 _cachedScale = Vector3.zero;
    private CoroutineHandle? _coroutine;

    internal Locker? Locker;

    private void OnDestroy()
    {
        if (_coroutine is null)
            return;

        Timing.KillCoroutines(_coroutine.Value);
    }

    internal void Init()
    {
        _coroutine = Timing.RunCoroutine(Coroutine());
    }

    private IEnumerator<float> Coroutine()
    {
        while (Locker != null)
        {
            yield return Timing.WaitForSeconds(Interval);

            Transform trans = Locker.netIdentity.gameObject.transform;

            if (_cachedPosition == trans.position &&
                _cachedRotation == trans.rotation &&
                _cachedScale == trans.lossyScale)
                continue;

            _cachedPosition = trans.position;
            _cachedRotation = trans.rotation;
            _cachedScale = trans.lossyScale;

            try
            {
                Locker.netIdentity.UpdateData();
            }
            catch
            {
                // ignored
            } // end try-catch
        } // end while
    } // end Coroutine
}