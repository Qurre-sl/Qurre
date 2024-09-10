using System.Collections.Generic;
using UnityEngine;

namespace Qurre.Internal.Fields;

internal static class Player
{
    internal static readonly Dictionary<GameObject, API.Player> Dictionary = [];
    internal static readonly Dictionary<ReferenceHub, API.Player> Hubs = [];
    internal static readonly Dictionary<int, API.Player> Ids = [];
    internal static readonly Dictionary<string, API.Player> Args = [];
}