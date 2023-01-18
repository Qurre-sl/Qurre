using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using UnityEngine;

namespace Qurre.Internal.Misc
{
    internal class DoorsUpdater : MonoBehaviour
    {
        internal DoorVariant Door;

        internal void Update()
        {
            if (Door is null)
            {
                return;
            }

            try
            {
                Door.netIdentity.UpdateData();
            }
            catch { }
        }
    }
}