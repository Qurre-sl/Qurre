using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using MEC;
using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Misc;

internal class DoorsUpdater : MonoBehaviour
{
    private const float Interval = 0.1f;

    private Vector3 _cachedPosition = Vector3.zero;
    private Quaternion _cachedRotation = Quaternion.identity;
    private Vector3 _cachedScale = Vector3.zero;
    private CoroutineHandle? _coroutine;

    internal DoorVariant? Door;

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
        while (Door != null)
        {
            yield return Timing.WaitForSeconds(Interval);

            Transform trans = Door.netIdentity.gameObject.transform;

            if (_cachedPosition == trans.position &&
                _cachedRotation == trans.rotation &&
                _cachedScale == trans.lossyScale)
                continue;

            _cachedPosition = trans.position;
            _cachedRotation = trans.rotation;
            _cachedScale = trans.lossyScale;

            try
            {
                Door.netIdentity.UpdateData();
            }
            catch
            {
                // ignored
            } // end try-catch
        } // end while
    } // end Coroutine
}