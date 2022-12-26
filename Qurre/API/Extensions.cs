using NorthwoodLib;
using PlayerRoles;
using PlayerStatsSystem;
using Qurre.API.Objects;
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

		#region Player.Get
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

		#region Player
		static public Player GetAttacker(this DamageHandlerBase handler)
		{
			var atc = _getHandler();
			if (atc is null) return null;
			return atc.Attacker.Hub.GetPlayer();
			AttackerDamageHandler _getHandler() => handler switch
			{
				AttackerDamageHandler adh2 => adh2,
				_ => null,
			};
		}
		#endregion

		#region Damages

		static public LiteDamageTypes GetLiteDamageTypes(this DamageHandlerBase handler) => handler switch
		{
			CustomReasonDamageHandler _ => LiteDamageTypes.Custom,
			DisruptorDamageHandler _ => LiteDamageTypes.Disruptor,
			ExplosionDamageHandler _ => LiteDamageTypes.Explosion,
			FirearmDamageHandler _ => LiteDamageTypes.Gun,
			MicroHidDamageHandler _ => LiteDamageTypes.MicroHid,
			RecontainmentDamageHandler _ => LiteDamageTypes.Recontainment,
			Scp018DamageHandler _ => LiteDamageTypes.Scp018,
			Scp049DamageHandler _ => LiteDamageTypes.Scp049,
			Scp096DamageHandler _ => LiteDamageTypes.Scp096,
			ScpDamageHandler _ => LiteDamageTypes.ScpDamage,
			UniversalDamageHandler _ => LiteDamageTypes.Universal,
			WarheadDamageHandler _ => LiteDamageTypes.Warhead,
			_ => LiteDamageTypes.Unknow,
		};

		static internal readonly Dictionary<DamageHandlerBase, DamageTypes> DamagesCached = new();
		static public DamageTypes GetDamageType(this DamageHandlerBase handler)
		{
			if (DamagesCached.TryGetValue(handler, out var damageType)) return damageType;

			var _type = _get();
			DamagesCached.Add(handler, _type);
			return _type;

			DamageTypes _get() => handler switch
			{
				CustomReasonDamageHandler _ => DamageTypes.Custom,
				DisruptorDamageHandler _ => DamageTypes.Disruptor,
				ExplosionDamageHandler _ => DamageTypes.Explosion,

				FirearmDamageHandler fr => fr.WeaponType switch
				{
					ItemType.GunCOM15 => DamageTypes.Com15,
					ItemType.GunCOM18 => DamageTypes.Com18,
					ItemType.GunCom45 => DamageTypes.Com45,
					ItemType.GunRevolver => DamageTypes.Revolver,

					ItemType.GunFSP9 => DamageTypes.FSP9,
					ItemType.GunCrossvec => DamageTypes.CrossVec,

					ItemType.GunAK => DamageTypes.AK,
					ItemType.GunE11SR => DamageTypes.E11SR,
					ItemType.GunLogicer => DamageTypes.Logicer,
					ItemType.GunShotgun => DamageTypes.Shotgun,

					ItemType.ParticleDisruptor => DamageTypes.Disruptor,

					_ => DamageTypes.Unknow,
				},

				MicroHidDamageHandler _ => DamageTypes.MicroHid,
				RecontainmentDamageHandler _ => DamageTypes.Recontainment,
				Scp018DamageHandler _ => DamageTypes.Scp018,
				Scp049DamageHandler _ => DamageTypes.Scp049,
				Scp096DamageHandler _ => DamageTypes.Scp096,
				ScpDamageHandler sr => parseTranslation(sr._translationId),
				UniversalDamageHandler tr => parseTranslation(tr.TranslationId),
				WarheadDamageHandler _ => DamageTypes.Warhead,
				_ => DamageTypes.Unknow,
			};

			DamageTypes parseTranslation(byte _translationId) => _translationId switch
			{
				0 => DamageTypes.Recontainment,
				1 => DamageTypes.Warhead,
				2 => DamageTypes.Scp049,
				4 => DamageTypes.Asphyxiation,
				5 => DamageTypes.Bleeding,
				6 => DamageTypes.Falldown,
				7 => DamageTypes.Pocket,
				8 => DamageTypes.Decontamination,
				9 => DamageTypes.Poison,
				10 => DamageTypes.Scp207,
				11 => DamageTypes.SeveredHands,
				12 => DamageTypes.MicroHid,
				13 => DamageTypes.Tesla,
				14 => DamageTypes.Explosion,
				15 => DamageTypes.Scp096,
				16 => DamageTypes.Scp173,
				17 => DamageTypes.Scp939Lunge,
				18 => DamageTypes.Zombie,
				20 => DamageTypes.Crushed,
				22 => DamageTypes.FriendlyFireDetector,
				23 => DamageTypes.Hypothermia,
				24 => DamageTypes.CardiacArrest,
				25 => DamageTypes.Scp939,
				_ => DamageTypes.Unknow,
			};
		}
		#endregion
	}
}