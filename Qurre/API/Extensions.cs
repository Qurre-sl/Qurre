using NorthwoodLib;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Qurre.API
{
	static public class Extensions
	{
		static public bool TryFind<TSource>(this IEnumerable<TSource> source, out TSource found, Func<TSource, bool> predicate)
		{
			foreach (TSource t in source)
			{
				if (predicate(t))
				{
					found = t;
					return true;
				}
			}
			found = default;
			return false;
		}

		#region Player
		public static IEnumerable<Player> GetPlayer(this Team team) => Player.List.Where(player => player.RoleInfomation.Team == team);
		public static IEnumerable<Player> GetPlayer(this RoleTypeId role) => Player.List.Where(player => player.RoleInfomation.Role == role);

		public static Player GetPlayer(this CommandSender sender) => sender is null ? null : GetPlayer(sender.SenderId);
		public static Player GetPlayer(this ReferenceHub referenceHub) { try { return referenceHub is null ? null : GetPlayer(referenceHub.gameObject); } catch { return null; } }
		public static Player GetPlayer(this uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? GetPlayer(hub) : null;

		public static Player GetPlayer(this GameObject gameObject)
		{
			if (gameObject is null) return null;
			Internal.Fields.Player.Dictionary.TryGetValue(gameObject, out Player player);
			return player;
		}

		public static Player GetPlayer(this int playerId)
		{
			if (Internal.Fields.Player.IDs.TryGetValue(playerId, out var _pl)) return _pl;

			foreach (Player pl in Player.List.Where(x => x.UserInfomation.Id == playerId))
			{
				Internal.Fields.Player.IDs.Add(playerId, pl);
				return pl;
			}

			return null;
		}

		public static Player GetPlayer(this string args)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(args))
					return null;

				if (Internal.Fields.Player.Args.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub is not null)
					return playerFound;

				if (int.TryParse(args, out int id))
					return GetPlayer(id);

				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
				{
					foreach (Player player in Internal.Fields.Player.Dictionary.Values)
					{
						if (player.UserInfomation.UserId == args)
						{
							playerFound = player;
							break;
						}
					}
				}
				else
				{
					int lastnameDifference = 31;
					string firstString = args.ToLower();
					foreach (Player player in Internal.Fields.Player.Dictionary.Values)
					{
						if (player.UserInfomation.NickName is null) continue;
						if (!player.UserInfomation.NickName.Contains(args, StringComparison.OrdinalIgnoreCase))
							continue;
						string secondString = player.UserInfomation.NickName;
						int nameDifference = secondString.Length - firstString.Length;
						if (nameDifference < lastnameDifference)
						{
							lastnameDifference = nameDifference;
							playerFound = player;
						}
					}
				}

				if (playerFound is not null)
					Internal.Fields.Player.Args[args] = playerFound;

				return playerFound;
			}
			catch (Exception ex)
			{
				Log.Error($"[API.Player.Get(string)] umm, error: {ex}");
				return null;
			}
		}
		#endregion
	}
}