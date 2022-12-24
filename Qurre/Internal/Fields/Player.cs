using System.Collections.Generic;
using UnityEngine;
namespace Qurre.Internal.Fields
{
	static internal class Player
	{
		static internal readonly Dictionary<GameObject, API.Player> Dictionary = new();
		static internal readonly Dictionary<int, API.Player> IdPlayers = new();
		static internal readonly Dictionary<string, API.Player> UserIDPlayers = new();
		static internal readonly Dictionary<string, API.Player> ArgsPlayers = new();
	}
}