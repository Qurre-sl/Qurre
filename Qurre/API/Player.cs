using NorthwoodLib;
using Qurre.API.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Field = Qurre.Internal.Fields.Player;
namespace Qurre.API
{
	public class Player
	{
		private readonly ReferenceHub rh;
		private readonly GameObject go;
		private string ui = "";
		private readonly string _nick = "";
		private string _tag = "";
		internal List<KillElement> _kills = new();

		static public IEnumerable<Player> List => Field.Dictionary.Values.Where(x => !x.Bot);

		internal Player(GameObject gameObject) => new Player(ReferenceHub.GetHub(gameObject));
		internal Player(ReferenceHub _rh)
		{
			rh = _rh;
			go = _rh.gameObject;
			ui = _rh.characterClassManager.UserId;
			try { _nick = _rh.nicknameSync.Network_myNickSync; } catch { }

		}

		public ReferenceHub ReferenceHub => rh;
		public CharacterClassManager ClassManager => rh.characterClassManager;

		public string Tag
		{
			get => _tag;
			set
			{
				if (value is null) return;
				_tag = value;
			}
		}
		public int Id
		{
			get => rh.queryProcessor.NetworkPlayerId;//need to find new field
			set => rh.queryProcessor.NetworkPlayerId = value;
		}
		public string UserId
		{
			get
			{
				if (Bot)
				{
					if (ui is null) ui = $"7{UnityEngine.Random.Range(0, 99999999)}{UnityEngine.Random.Range(0, 99999999)}@bot";
					return ui;
				}
				try
				{
					string _ = ClassManager.UserId;
					if (_.Contains("@"))
					{
						ui = _;
						return _;
					}
					return ui;
				}
				catch { return ui; }
			}
			set => ClassManager.NetworkSyncedUserId = value;
		}
		public string CustomUserId
		{
			get => ClassManager.UserId2;
			set => ClassManager.UserId2 = value;
		}
		public string DisplayNickname
		{
			get => rh.nicknameSync.Network_displayName;
			set => rh.nicknameSync.Network_displayName = value;
		}
		public string Nickname
		{
			get
			{
				try { return rh.nicknameSync.Network_myNickSync; }
				catch { return _nick; }
			}
			internal set => rh.nicknameSync.Network_myNickSync = value;
		}
		public bool DoNotTrack => ServerRoles.DoNotTrack;//*
		public bool RemoteAdminAccess => ServerRoles.RemoteAdmin;//*
		public bool Overwatch//*
		{
			get => ServerRoles.OverwatchEnabled;
			set => ServerRoles.SetOverwatchStatus(value);
		}



		public bool Bot { get; internal set; } = false;






		public static IEnumerable<Player> Get(Team team) => List.Where(player => player.Team == team);
		static public IEnumerable<Player> Get(RoleType role) => List.Where(player => player.Role == role);
		static public Player Get(CommandSender sender) => sender is null ? null : Get(sender.SenderId);
		static public Player Get(ReferenceHub rh) { try { return rh is null ? null : Get(rh.gameObject); } catch { return null; } }
		static public Player Get(GameObject gameObject)
		{
			if (gameObject is null) return null;
			Field.Dictionary.TryGetValue(gameObject, out Player player);
			return player;
		}
		static public Player Get(uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;
		static public Player Get(int playerId)
		{
			if (Field.IdPlayers.ContainsKey(playerId)) return Field.IdPlayers[playerId];
			foreach (Player pl in List.Where(x => x.Id == playerId))
			{
				Field.IdPlayers.Add(playerId, pl);
				return pl;
			}
			return null;
		}
		static public Player Get(string args)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(args))
					return null;
				if (Field.ArgsPlayers.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub is not null)
					return playerFound;
				if (int.TryParse(args, out int id))
					return Get(id);
				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
				{
					foreach (Player player in Field.Dictionary.Values)
					{
						if (player.UserId == args)
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
					foreach (Player player in Field.Dictionary.Values)
					{
						if (player.Nickname is null) continue;
						if (!player.Nickname.Contains(args, StringComparison.OrdinalIgnoreCase))
							continue;
						string secondString = player.Nickname;
						int nameDifference = secondString.Length - firstString.Length;
						if (nameDifference < lastnameDifference)
						{
							lastnameDifference = nameDifference;
							playerFound = player;
						}
					}
				}
				if (playerFound is not null)
					Field.ArgsPlayers[args] = playerFound;
				return playerFound;
			}
			catch (Exception ex)
			{
				Log.Error($"[API.Player.Get(string)] umm, error: {ex}");
				return null;
			}
		}
	}
}