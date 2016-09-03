using System;
using System.Collections.Generic;
using System.Linq;
using GriefClientPro.Utils;
using UnityEngine;

namespace GriefClientPro
{
    public class KillAllPlayers
    {
        public HashSet<GameObject> Players { get; } = new HashSet<GameObject>();

        public HashSet<string> PermaKillPlayers { get; } = new HashSet<string>();

        public KillAllPlayers(GriefClientPro instance)
        {
            // Get the current players
            var components = UnityEngine.Object.FindObjectsOfType<BoltPlayerSetup>().Where(coopPlayer => coopPlayer.gameObject.GetComponent<CoopPlayerRemoteSetup>() != null).ToArray();
            if (components.Length > 0)
            {
                foreach (var player in components.Where(player => player.entity.GetState<IPlayerState>().name != GriefClientPro.PlayerName))
                {
                    Players.Add(player.gameObject);
                }
            }

            // Listen to required events
            instance.OnTick += OnTick;
        }

        private void OnTick(object sender, EventArgs args)
        {
            // Only in Multiplayer
            if (!BoltNetwork.isRunning)
            {
                return;
            }

            var currentPlayers = BoltNetwork.entities.Where(current => current.StateIs<IPlayerState>() && current.GetState<IPlayerState>().name != GriefClientPro.PlayerName).ToArray();
            if (currentPlayers.Length > 0)
            {
                try
                {
                    // Remove invalid players
                    foreach (var coopPlayer in Players.ToArray())
                    {
                        try
                        {
                            if (coopPlayer == null ||
                                !coopPlayer.activeSelf ||
                                !coopPlayer.activeInHierarchy ||
                                coopPlayer.GetComponent<BoltPlayerSetup>() == null ||
                                coopPlayer.GetComponent<BoltPlayerSetup>().entity == null ||
                                currentPlayers.All(o => o.GetState<IPlayerState>().name != coopPlayer.GetComponent<BoltPlayerSetup>().entity.GetState<IPlayerState>().name))
                            {
                                Players.Remove(coopPlayer);
                            }
                        }
                        catch (Exception)
                        {
                            Players.Remove(coopPlayer);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception("Exception while removing invalid players", e);
                }

                try
                {
                    // Add missing players
                    if (currentPlayers.Any(player => Players.All(o => o.GetComponent<BoltPlayerSetup>().entity.GetState<IPlayerState>().name != player.GetState<IPlayerState>().name)))
                    {
                        // Clear stored players
                        Players.Clear();

                        // Add all current players
                        foreach (var coopPlayer in UnityEngine.Object.FindObjectsOfType<BoltPlayerSetup>().Where(coopPlayer => coopPlayer.gameObject.GetComponent<CoopPlayerRemoteSetup>() != null))
                        {
                            Players.Add(coopPlayer.gameObject);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception("Exception while refreshing players:", e);

                    // Clear stored players
                    Players.Clear();

                    // Add all current players
                    foreach (var coopPlayer in UnityEngine.Object.FindObjectsOfType<BoltPlayerSetup>().Where(coopPlayer => coopPlayer.gameObject.GetComponent<CoopPlayerRemoteSetup>() != null))
                    {
                        Players.Add(coopPlayer.gameObject);
                    }
                }
            }
            else
            {
                Players.Clear();
                PermaKillPlayers.Clear();
            }

            try
            {
                // Execute PermaKill
                if (Players.Count > 0 && PermaKillPlayers.Count > 0)
                {
                    foreach (var playerName in PermaKillPlayers)
                    {
                        foreach (var coopPlayer in Players.ToArray())
                        {
                            try
                            {
                                if (coopPlayer.GetComponent<BoltPlayerSetup>().entity.GetState<IPlayerState>().name == playerName)
                                {
                                    KillSinglePlayer(coopPlayer.GetComponent<CoopPlayerRemoteSetup>());
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                Players.Remove(coopPlayer);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Exception("Exception while executing perma kill:", e);
            }

            // Debug current players
            //Logger.Info("Players ({1}): {0}", string.Join(", ", Players.Select(o => o.GetComponent<BoltPlayerSetup>().entity.GetState<IPlayerState>().name).ToArray()), Players.Count);
        }

        public void Execute()
        {
            foreach (var player in Players)
            {
                KillSinglePlayer(player.GetComponent<CoopPlayerRemoteSetup>());
            }
        }

        public static void KillSinglePlayer(CoopPlayerRemoteSetup player)
        {
            try
            {
                player.hitFromEnemy(1000000);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
