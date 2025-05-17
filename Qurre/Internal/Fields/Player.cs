using System.Collections.Generic;
using UnityEngine;

namespace Qurre.Internal.Fields;

internal static class Player
{
    internal static readonly Dictionary<GameObject, API.Controllers.Player> Dictionary = [];
    internal static readonly Dictionary<ReferenceHub, API.Controllers.Player> Hubs = [];
    internal static readonly Dictionary<int, API.Controllers.Player> Ids = [];
    internal static readonly Dictionary<string, API.Controllers.Player> Args = [];
}