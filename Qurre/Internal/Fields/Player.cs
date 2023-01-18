using System.Collections.Generic;
using UnityEngine;

namespace Qurre.Internal.Fields
{
    internal static class Player
    {
        internal static readonly Dictionary<GameObject, API.Player> Dictionary = new ();
        internal static readonly Dictionary<int, API.Player> IDs = new ();
        internal static readonly Dictionary<string, API.Player> UserIDs = new ();
        internal static readonly Dictionary<string, API.Player> Args = new ();
    }
}