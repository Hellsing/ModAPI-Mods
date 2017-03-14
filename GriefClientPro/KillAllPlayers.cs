using System;
using System.Collections.Generic;
using System.Linq;

namespace GriefClientPro
{
    public class KillAllPlayers
    {
        public HashSet<ulong> PermaKillPlayers { get; } = new HashSet<ulong>();

        public KillAllPlayers(GriefClientPro instance)
        {
            // Listen to required events
            instance.OnTick += OnTick;

            // Trigger OnTick
            OnTick(null, EventArgs.Empty);
        }

        public void AddPlayerToPermaKill(Player player)
        {
            PermaKillPlayers.Add(player.SteamId);
        }

        public void RemovePlayerToPermaKill(Player player)
        {
            PermaKillPlayers.Remove(player.SteamId);
        }

        private void OnTick(object sender, EventArgs args)
        {
            // Only in Multiplayer
            if (!BoltNetwork.isRunning)
            {
                return;
            }

            // Remove missing players to kill
            foreach (var steamId in PermaKillPlayers.ToArray().Where(steamId => GriefClientPro.PlayerManager.GetPlayerBySteamId(steamId) == null))
            {
                PermaKillPlayers.Remove(steamId);
            }

            // Execute PermaKill
            if (PermaKillPlayers.Count > 0)
            {
                foreach (var steamId in PermaKillPlayers)
                {
                    try
                    {
                        var player = GriefClientPro.PlayerManager.GetPlayerBySteamId(steamId);
                        if (player != null)
                        {
                            // Kill the player
                            KillSinglePlayer(player.CoopPlayer);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        public static void KillSinglePlayer(CoopPlayerRemoteSetup player)
        {
            player.hitFromEnemy(1000000);
        }
    }
}
