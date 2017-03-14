using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Steamworks;
using TheForest.Networking;
using TheForest.Utils;
using UnityEngine;

namespace GriefClientPro
{
    public class PlayerManager
    {
        public List<Player> Players { get; } = new List<Player>();

        public PlayerManager(GriefClientPro instance)
        {
            // Listen to events
            instance.OnTick += OnTick;

            // Trigger OnTick once
            OnTick(null, EventArgs.Empty);
        }

        private void OnTick(object sender, EventArgs args)
        {
            // Only in Multiplayer
            if (!BoltNetwork.isRunning || Scene.SceneTracker == null || Scene.SceneTracker.allPlayerEntities == null)
            {
                return;
            }

            // Refresh players
            Players.Clear();
            Players.AddRange(Scene.SceneTracker.allPlayerEntities
                .Where(o => o.isAttached &&
                            o.StateIs<IPlayerState>() &&
                            LocalPlayer.Entity != o &&
                            //o.GetState<IPlayerState>().name != GriefClientPro.PlayerName &&
                            o.gameObject.activeSelf &&
                            o.gameObject.activeInHierarchy &&
                            o.GetComponent<BoltPlayerSetup>() != null)
                .OrderBy(o => o.GetState<IPlayerState>().name)
                .Select(o => new Player(o)));
        }

        public Player GetPlayerBySteamId(ulong steamId)
        {
            return Players.FirstOrDefault(o => o.SteamId == steamId);
        }
    }

    public class Player
    {
        private static readonly Dictionary<string, ulong> CachedIds = new Dictionary<string, ulong>();

        public BoltEntity Entity { get; }
        public ulong SteamId =>
            CachedIds.ContainsKey(Name)
                ? CachedIds[Name]
                : (CachedIds[Name] = CoopLobby.Instance.AllMembers.FirstOrDefault(o => SteamFriends.GetFriendPersonaName(o) == Name).m_SteamID);

        public string Name => Entity.GetState<IPlayerState>().name;
        public BoltPlayerSetup PlayerSetup => Entity.GetComponent<BoltPlayerSetup>();
        public CoopPlayerRemoteSetup CoopPlayer => Entity.GetComponent<CoopPlayerRemoteSetup>();
        public GameObject DeadTriggerObject
        {
            get
            {
                var playerSetup = PlayerSetup;
                if (playerSetup != null)
                {
                    var fieldInfo = typeof (BoltPlayerSetup).GetField("RespawnDeadTrigger", BindingFlags.NonPublic | BindingFlags.Instance);
                    var gameObject = fieldInfo?.GetValue(playerSetup) as GameObject;
                    if (gameObject != null)
                    {
                        return gameObject;
                    }
                }
                return null;
            }
        }
        public RespawnDeadTrigger DeadTrigger => DeadTriggerObject.GetComponent<RespawnDeadTrigger>();
        public Transform Transform => Entity.transform;
        public Vector3 Position => Transform.position;

        public Player(BoltEntity player)
        {
            // Apply properties
            Entity = player;
        }
    }
}
