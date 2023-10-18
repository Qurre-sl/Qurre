using CustomPlayerEffects;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using MapGeneration;
using MapGeneration.Distributors;
using Mirror;
using NorthwoodLib;
using PlayerRoles;
using PlayerStatsSystem;
using Qurre.API.Addons;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using SinkHole = CustomPlayerEffects.Sinkhole;

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

		static public void ForEach<T>(this IReadOnlyList<T> list, Action<T> action)
		{
			if (action is null) throw new ArgumentNullException("Action is null");

			for (int i = 0; i < list.Count; i++)
			{
				action(list[i]);
			}
		}

		static public void Shuffle<T>(this IList<T> list)
		{
			RNGCryptoServiceProvider provider = new();
			int n = list.Count;
			while (n > 1)
			{
				byte[] box = new byte[1];
				do provider.GetBytes(box);
				while (!(box[0] < n * (byte.MaxValue / n)));
				int k = (box[0] % n);
				n--;
				(list[n], list[k]) = (list[k], list[n]);
			}
		}

		static public float Difference(this float first, float second)
			=> Math.Abs(first - second);

		#region Player.Get
		static public IEnumerable<Player> GetPlayer(this Team team) => Player.List.Where(player => player.RoleInfomation.Team == team);
		static public IEnumerable<Player> GetPlayer(this RoleTypeId role) => Player.List.Where(player => player.RoleInfomation.Role == role);

		static public Player GetPlayer(this CommandSender sender) => sender is null ? null : GetPlayer(sender.SenderId);
		static public Player GetPlayer(this ReferenceHub referenceHub) { try { return referenceHub is null ? null : GetPlayer(referenceHub.gameObject); } catch { return null; } }
		static public Player GetPlayer(this uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? GetPlayer(hub) : null;

		static public Player GetPlayer(this GameObject gameObject)
		{
			if (gameObject is null) return null;
			Internal.Fields.Player.Dictionary.TryGetValue(gameObject, out Player player);
			return player;
		}

		static public Player GetPlayer(this int playerId)
		{
			if (Internal.Fields.Player.IDs.TryGetValue(playerId, out var _pl)) return _pl;

			foreach (Player pl in Player.List.Where(x => x.UserInfomation.Id == playerId))
			{
				Internal.Fields.Player.IDs.Add(playerId, pl);
				return pl;
			}

			return null;
		}

		static public Player GetPlayer(this string args)
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
						if (player.UserInfomation.Nickname is null) continue;
						if (!player.UserInfomation.Nickname.Contains(args, StringComparison.OrdinalIgnoreCase))
							continue;
						string secondString = player.UserInfomation.Nickname;
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

		static public float DistanceTo(this Player source, Player player)
			=> Vector3.Distance(source.MovementState.Position, player.MovementState.Position);
		static public float DistanceTo(this Player source, Vector3 position)
			=> Vector3.Distance(source.MovementState.Position, position);
		static public float DistanceTo(this Player source, GameObject Object)
			=> Vector3.Distance(source.MovementState.Position, Object.transform.position);
		#endregion

		#region Damages

		static public LiteDamageTypes GetLiteDamageTypes(this DamageHandlerBase handler) => handler switch
		{
			CustomReasonDamageHandler _ => LiteDamageTypes.Custom,
			DisruptorDamageHandler _ => LiteDamageTypes.Disruptor,
			ExplosionDamageHandler _ => LiteDamageTypes.Explosion,
			FirearmDamageHandler _ => LiteDamageTypes.Gun,
			JailbirdDamageHandler _ => LiteDamageTypes.Jailbird,
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
                JailbirdDamageHandler _ => DamageTypes.Jailbird,
                MicroHidDamageHandler _ => DamageTypes.MicroHid,

                FirearmDamageHandler fr => fr.WeaponType switch
				{
					ItemType.GunCOM15 => DamageTypes.Com15,
					ItemType.GunCOM18 => DamageTypes.Com18,
					ItemType.GunCom45 => DamageTypes.Com45,
					ItemType.GunRevolver => DamageTypes.Revolver,

					ItemType.GunFSP9 => DamageTypes.FSP9,
					ItemType.GunCrossvec => DamageTypes.CrossVec,

					ItemType.GunE11SR => DamageTypes.E11SR,
					ItemType.GunFRMG0 => DamageTypes.FRMG0,

					ItemType.GunAK => DamageTypes.AK,
					ItemType.GunLogicer => DamageTypes.Logicer,
					ItemType.GunShotgun => DamageTypes.Shotgun,
					ItemType.GunA7 => DamageTypes.A7,

					ItemType.ParticleDisruptor => DamageTypes.Disruptor,
					ItemType.Jailbird => DamageTypes.Jailbird,

					_ => DamageTypes.Unknow,
				},

				RecontainmentDamageHandler _ => DamageTypes.Recontainment,
				Scp018DamageHandler _ => DamageTypes.Scp018,
				Scp049DamageHandler _ => DamageTypes.Scp049,
				Scp096DamageHandler _ => DamageTypes.Scp096,
                WarheadDamageHandler _ => DamageTypes.Warhead,
                ScpDamageHandler sr => parseTranslation(sr._translationId),
				UniversalDamageHandler tr => parseTranslation(tr.TranslationId),
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

		#region Prefabs
		static public BreakableDoor GetPrefab(this DoorPrefabs prefab)
		{
			if (Prefabs.Doors.TryGetValue(prefab, out var door)) return door;
			return Prefabs.Doors.First().Value;
		}
		static public GameObject GetPrefab(this TargetPrefabs prefab)
		{
			if (Prefabs.Targets.TryGetValue(prefab, out var target)) return target;
			return Prefabs.Targets.First().Value;
		}
		static public MapGeneration.Distributors.Locker GetPrefab(this LockerPrefabs prefab)
		{
			if (prefab is LockerPrefabs.Pedestal)
			{
				prefab = UnityEngine.Random.Range(0, 100) switch
				{
					> 80 => LockerPrefabs.Pedestal268,
					> 60 => LockerPrefabs.Pedestal207,
					> 40 => LockerPrefabs.Pedestal500,
					> 20 => LockerPrefabs.Pedestal018,
					_ => LockerPrefabs.Pedestal2176,
				};
			}
			if (Prefabs.Lockers.TryGetValue(prefab, out var locker)) return locker;
			return Prefabs.Lockers.First().Value;
		}
		#endregion

		#region Items
		static public ItemCategory GetCategory(this ItemType itemType)
		{
			return itemType switch
			{
				ItemType.KeycardJanitor or ItemType.KeycardScientist or ItemType.KeycardResearchCoordinator or
				ItemType.KeycardZoneManager or ItemType.KeycardGuard or ItemType.KeycardContainmentEngineer or
				ItemType.KeycardMTFPrivate or ItemType.KeycardMTFOperative or ItemType.KeycardMTFCaptain or
				ItemType.KeycardFacilityManager or ItemType.KeycardChaosInsurgency or ItemType.KeycardO5 => ItemCategory.Keycard,

				ItemType.Radio => ItemCategory.Radio,
				ItemType.MicroHID => ItemCategory.MicroHID,

				ItemType.Medkit or ItemType.Adrenaline or ItemType.Painkillers => ItemCategory.Medical,

				ItemType.GunCOM15 or ItemType.GunE11SR or ItemType.GunCrossvec or ItemType.GunFSP9 or
				ItemType.GunLogicer or ItemType.GunCOM18 or ItemType.GunRevolver or ItemType.GunAK or
				ItemType.GunShotgun or ItemType.ParticleDisruptor or ItemType.GunCom45 or ItemType.Jailbird
				or ItemType.GunFRMG0 or ItemType.GunA7 => ItemCategory.Firearm,

				ItemType.GrenadeHE or ItemType.GrenadeFlash => ItemCategory.Grenade,

				ItemType.SCP500 or ItemType.SCP207 or ItemType.SCP018 or ItemType.SCP268 or ItemType.SCP330 or
				ItemType.SCP2176 or ItemType.SCP244a or ItemType.SCP244b or ItemType.SCP1853 or ItemType.SCP1576 or
				ItemType.AntiSCP207 => ItemCategory.SCPItem,

				ItemType.Ammo12gauge or ItemType.Ammo556x45 or ItemType.Ammo44cal or ItemType.Ammo762x39 or ItemType.Ammo9x19 => ItemCategory.Ammo,

				ItemType.ArmorLight or ItemType.ArmorCombat or ItemType.ArmorHeavy => ItemCategory.Armor,

				_ => ItemCategory.None
			};
		}

		static internal AmmoType GetAmmoType(this ItemType itemType)
		{
			return itemType switch
			{
				ItemType.Ammo556x45 => AmmoType.Ammo556,
				ItemType.Ammo762x39 => AmmoType.Ammo762,
				ItemType.Ammo9x19 => AmmoType.Ammo9,
				ItemType.Ammo12gauge => AmmoType.Ammo12Gauge,
				ItemType.Ammo44cal => AmmoType.Ammo44Cal,
				_ => AmmoType.None
			};
		}

		static internal ItemType GetItemType(this AmmoType ammoType)
		{
			return ammoType switch
			{
				AmmoType.Ammo556 => ItemType.Ammo556x45,
				AmmoType.Ammo762 => ItemType.Ammo762x39,
				AmmoType.Ammo9 => ItemType.Ammo9x19,
				AmmoType.Ammo12Gauge => ItemType.Ammo12gauge,
				AmmoType.Ammo44Cal => ItemType.Ammo44cal,
				_ => ItemType.None,
			};
		}

		static internal ItemBase CreateItemInstance(this ItemType type, Player owner = null)
		{
			ItemIdentifier identifier = new(type, ItemSerialGenerator.GenerateNext());

			if (owner != null)
				owner.Inventory.Base.CreateItemInstance(identifier, false);

			return Server.Host.Inventory.Base.CreateItemInstance(identifier, false);
		}
		#endregion

		#region GameObjects
		static public void NetworkRespawn(this GameObject gameObject)
		{
			NetworkServer.UnSpawn(gameObject);
			NetworkServer.Spawn(gameObject);
		}
		#endregion

		#region Effects
		static public Type Type(this EffectType effect) => effect switch
		{
			EffectType.AmnesiaItems => typeof(AmnesiaItems),
			EffectType.AmnesiaVision => typeof(AmnesiaVision),
			EffectType.Asphyxiated => typeof(Asphyxiated),
			EffectType.Bleeding => typeof(Bleeding),
			EffectType.Blinded => typeof(Blinded),
			EffectType.BodyshotReduction => typeof(BodyshotReduction),
			EffectType.Burned => typeof(Burned),
			EffectType.CardiacArrest => typeof(CardiacArrest),
			EffectType.Concussed => typeof(Concussed),
			EffectType.Corroding => typeof(Corroding),
			EffectType.DamageReduction => typeof(DamageReduction),
			EffectType.Deafened => typeof(Deafened),
			EffectType.Decontaminating => typeof(Decontaminating),
			EffectType.Disabled => typeof(Disabled),
			EffectType.Ensnared => typeof(Ensnared),
			EffectType.Exhausted => typeof(Exhausted),
			EffectType.Flashed => typeof(Flashed),
			EffectType.Hemorrhage => typeof(Hemorrhage),
			EffectType.Hypothermia => typeof(Hypothermia),
			EffectType.InsufficientLighting => typeof(InsufficientLighting),
			EffectType.Invigorated => typeof(Invigorated),
			EffectType.Invisible => typeof(Invisible),
			EffectType.MovementBoost => typeof(MovementBoost),
			EffectType.Poisoned => typeof(Poisoned),
			EffectType.RainbowTaste => typeof(RainbowTaste),
			EffectType.Scp1853 => typeof(Scp1853),
			EffectType.Scp207 => typeof(Scp207),
			EffectType.SeveredHands => typeof(SeveredHands),
			EffectType.Sinkhole => typeof(SinkHole),
			EffectType.SoundtrackMute => typeof(SoundtrackMute),
			EffectType.SpawnProtected => typeof(SpawnProtected),
			EffectType.Stained => typeof(Stained),
			EffectType.Traumatized => typeof(Traumatized),
			EffectType.Vitality => typeof(Vitality),
			EffectType.PocketCorroding => typeof(PocketCorroding),
			_ => throw new InvalidOperationException("Invalid effect enum provided"),
		};
		static public EffectType GetEffectType(this StatusEffectBase ef) => ef switch
		{
			AmnesiaItems => EffectType.AmnesiaItems,
			AmnesiaVision => EffectType.AmnesiaVision,
			Asphyxiated => EffectType.Asphyxiated,
			Bleeding => EffectType.Bleeding,
			Blinded => EffectType.Blinded,
			BodyshotReduction => EffectType.BodyshotReduction,
			Burned => EffectType.Burned,
			CardiacArrest => EffectType.CardiacArrest,
			Concussed => EffectType.Concussed,
			Corroding => EffectType.Corroding,
			DamageReduction => EffectType.DamageReduction,
			Deafened => EffectType.Deafened,
			Decontaminating => EffectType.Decontaminating,
			Disabled => EffectType.Disabled,
			Ensnared => EffectType.Ensnared,
			Exhausted => EffectType.Exhausted,
			Flashed => EffectType.Flashed,
			Hemorrhage => EffectType.Hemorrhage,
			Hypothermia => EffectType.Hypothermia,
			InsufficientLighting => EffectType.InsufficientLighting,
			Invigorated => EffectType.Invigorated,
			Invisible => EffectType.Invisible,
			MovementBoost => EffectType.MovementBoost,
			Poisoned => EffectType.Poisoned,
			RainbowTaste => EffectType.RainbowTaste,
			Scp1853 => EffectType.Scp1853,
			Scp207 => EffectType.Scp207,
			SeveredHands => EffectType.SeveredHands,
			SinkHole => EffectType.Sinkhole,
			SoundtrackMute => EffectType.SoundtrackMute,
			SpawnProtected => EffectType.SpawnProtected,
			Stained => EffectType.Stained,
			Traumatized => EffectType.Traumatized,
			Vitality => EffectType.Vitality,
			PocketCorroding => EffectType.PocketCorroding,
			_ => EffectType.None,
		};
		#endregion

		#region GetRoom
		static public Room GetRoom(this RoomName type) => Map.Rooms.FirstOrDefault(x => x.RoomName == type);
		static public Room GetRoom(this RoomType type) => Map.Rooms.FirstOrDefault(x => x.Type == type);
		static public Room GetRoom(this RoomIdentifier identifier) => Map.Rooms.FirstOrDefault(x => x.Identifier == identifier);
		#endregion

		#region GetTesla
		static public Tesla GetTesla(this TeslaGate teslaGate) => Map.Teslas.FirstOrDefault(x => x.GameObject == teslaGate.gameObject);
		static public Tesla GetTesla(this GameObject gameObject) => Map.Teslas.FirstOrDefault(x => x.GameObject == gameObject);
		#endregion

		#region GetGenerator
		static public Generator GetGenerator(this GameObject gameObject) => Map.Generators.FirstOrDefault(x => x.GameObject == gameObject);
		static public Generator GetGenerator(this Scp079Generator generator079) => Map.Generators.FirstOrDefault(x => x.GameObject == generator079.gameObject);
		#endregion

		#region GetLift
		static public Lift GetLift(this ElevatorChamber elevator) => Map.Lifts.FirstOrDefault(x => x.Elevator == elevator);
		#endregion

		#region GetLocker
		static public Controllers.Locker GetLocker(this MapGeneration.Distributors.Locker locker) => Map.Lockers.FirstOrDefault(x => x._locker == locker);
		#endregion

		#region GetDoor
		static public Door GetDoor(this DoorVariant variant) => Map.Doors.FirstOrDefault(x => x.DoorVariant == variant);
		static public Door GetDoor(this DoorType type) => Map.Doors.FirstOrDefault(x => x.Type == type);
		#endregion

		#region GetRagdoll
		static public Ragdoll GetRagdoll(this BasicRagdoll basic) => Map.Ragdolls.FirstOrDefault(x => x.ragdoll == basic);
		#endregion
	}
}