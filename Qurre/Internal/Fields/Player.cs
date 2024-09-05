using System.Collections.Generic;
using UnityEngine;

namespace Qurre.Internal.Fields
{
	static internal class Player
	{
		static internal readonly Dictionary<GameObject, API.Player> Dictionary = new();
		static internal readonly Dictionary<ReferenceHub, API.Player> Hubs = new();
		static internal readonly Dictionary<int, API.Player> IDs = new();
		static internal readonly Dictionary<string, API.Player> UserIDs = new();
		static internal readonly Dictionary<string, API.Player> Args = new();
	}
}